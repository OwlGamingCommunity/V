#define SUPPORT_PACKED_AUDIO_FILES
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Xml;

public class Job_BuildArchive : JobBase
{
	private string m_strDescriptorFilePath;

	public Job_BuildArchive(string strDescriptorFilePath) : base()
	{
		m_strDescriptorFilePath = strDescriptorFilePath;
	}

	public override bool Execute()
	{
		string archiveName = CookerHelpers.GetArchiveName(Path.GetFileName(m_strDescriptorFilePath));

		string data = File.ReadAllText(m_strDescriptorFilePath);
		CookerTypes.AssetDescriptor descriptor = JsonConvert.DeserializeObject<CookerTypes.AssetDescriptor>(data);

		CookerTypes.EArchiveResult result = Archive(archiveName, descriptor);
		return result != CookerTypes.EArchiveResult.Error;
	}

	[DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
	static extern bool CreateSymbolicLink(string lpSymlinkFileName, string lpTargetFileName, SymbolicLink dwFlags);

	enum SymbolicLink
	{
		File = 0,
		Directory = 1,
		SYMBOLIC_LINK_FLAG_ALLOW_UNPRIVILEGED_CREATE = 2,
	}

	private CookerTypes.EArchiveResult Archive(string strArchiveName, CookerTypes.AssetDescriptor Descriptor)
	{
		bool bIsFIM = CookerSettings.IsFastIterationMode();

		if (bIsFIM)
		{
			bool IsLinuxArchive = strArchiveName.ToLower().Contains("_linux");
			bool IsWindowsArchive = strArchiveName.ToLower().Contains("_windows");

			// Is this a platform archive?
			if (IsWindowsArchive || IsLinuxArchive)
			{
				// Are we on the correct platform?
				if (IsWindowsArchive && !CookerSettings.IsWindows())
				{
					BufferedWriteLine("FIM: Ignoring {0} as the package is not for this OS.", strArchiveName);

					// lets pretend everything is fine
					return CookerTypes.EArchiveResult.UpToDate;
				}

				if (IsLinuxArchive && !CookerSettings.IsLinux())
				{
					BufferedWriteLine("FIM: Ignoring {0} as the package is not for this OS.", strArchiveName);

					// lets pretend everything is fine
					return CookerTypes.EArchiveResult.UpToDate;
				}
			}

			// Meta.xml
			XmlWriterSettings xmlSettings = new XmlWriterSettings
			{
				Indent = true,
				IndentChars = "     ",
				NewLineOnAttributes = false,
				OmitXmlDeclaration = true
			};

			// always have to clear out the target folder for the archive in FIM because we don't compare hashes for individual files, so removed files would remain causing weird behavior
			if (CookerSettings.IsFastIterationMode())
			{
				bool bIsSharedResource = strArchiveName.EndsWith("_shared");
				bool bIsClientResource = strArchiveName.EndsWith(".client");
				if (!bIsClientResource || bIsSharedResource)
				{
					string strOutputFolderServer = Path.Combine(Directory.GetCurrentDirectory(), "..", "Output", CookerSettings.GetTargetName(), CookerHelpers.GetMountPathForArchive(strArchiveName, CookerTypes.EAssetType.ServerScript, "dummy.dll"));
					if (Directory.Exists(strOutputFolderServer))
					{
						Directory.Delete(strOutputFolderServer, true);
					}
				}

				if (bIsClientResource || bIsSharedResource)
				{
					// delete assets AND client scripts
					string strOutputFolderClientAssets = Path.Combine(Directory.GetCurrentDirectory(), "..", "Output", CookerSettings.GetTargetName(), CookerHelpers.GetMountPathForArchive(strArchiveName, CookerTypes.EAssetType.ClientAsset, "dummy.html"));
					if (Directory.Exists(strOutputFolderClientAssets))
					{
						Directory.Delete(strOutputFolderClientAssets, true);
					}

					string strOutputFolderClientScripts = Path.Combine(Directory.GetCurrentDirectory(), "..", "Output", CookerSettings.GetTargetName(), CookerHelpers.GetMountPathForArchive(strArchiveName, CookerTypes.EAssetType.ClientScript, "dummy.cs"));
					if (Directory.Exists(strOutputFolderClientScripts))
					{
						Directory.Delete(strOutputFolderClientScripts, true);
					}
				}
			}

			string strMetaFolder = Path.Combine("..", "Output", "Debug", "dotnet", "resources", strArchiveName);
			string strMetaPath = Path.Combine(strMetaFolder, "meta.xml");
			Directory.CreateDirectory(strMetaFolder);
			XmlWriter xmlWriter = XmlWriter.Create(strMetaPath, xmlSettings);

			xmlWriter.WriteStartDocument();
			xmlWriter.WriteStartElement("meta");
			xmlWriter.WriteStartElement("info");
			xmlWriter.WriteAttributeString("name", strArchiveName);
			xmlWriter.WriteAttributeString("author", "Owl Development Team");
			xmlWriter.WriteAttributeString("type", "script");
			xmlWriter.WriteEndElement();

			// Files
			bool bWroteClientJSFile = false;
			List<string> lstThisResourceIndexAdditions = new List<string>();
			foreach (CookerTypes.AssetFile fileDesc in Descriptor.Files)
			{
				// calculate source

				// we use Path.GetFileName here because folders (e.g. Assets/) etc are ignored for unarchiving
				string strFakeFileDir = Path.Combine(Directory.GetCurrentDirectory(), "..", "Output", CookerSettings.GetTargetName(), CookerHelpers.GetMountPathForArchive(strArchiveName, fileDesc.Type, Path.GetFileName(fileDesc.Name)));
				string strFakeFilePath = Path.Combine(strFakeFileDir, Path.GetFileName(fileDesc.Name));
				string strFileOnDiskPath = "";
				string strFileOnDiskDir = "";

				if (fileDesc.Type == CookerTypes.EAssetType.ServerScript)
				{
					strFileOnDiskDir = Path.Combine(Directory.GetCurrentDirectory(), strArchiveName, "bin", CookerSettings.GetTargetName(), CookerSettings.DotNetVersion);
					strFileOnDiskPath = Path.Combine(strFileOnDiskDir, fileDesc.Name);
				}
				else if (fileDesc.Type == CookerTypes.EAssetType.MapFile_Interior || fileDesc.Type == CookerTypes.EAssetType.MapFile_Persistent)
				{
					// NOTE: Nothing to do here, the folder is already sym linked
					continue;
				}
				else
				{
					strFileOnDiskDir = Path.Combine(Directory.GetCurrentDirectory(), strArchiveName);
					strFileOnDiskPath = Path.Combine(strFileOnDiskDir, fileDesc.Name);
				}

				if (!Directory.Exists(strFakeFileDir))
				{
					Directory.CreateDirectory(strFakeFileDir);
				}

#if SUPPORT_PACKED_AUDIO_FILES
				bool bIsPackedAudioFile = (fileDesc.Type == CookerTypes.EAssetType.Audio && fileDesc.Name.EndsWith("-XX.ogg"));
#else
				bool bIsPackedAudioFile = false;
#endif

				if (!File.Exists(strFileOnDiskPath) && !bIsPackedAudioFile)
				{
					string strErrorMsg = String.Format("File {0} does not exist on disk. ({1})", strFileOnDiskPath, strArchiveName);
					BufferedWriteLine("\t\t\tERROR: {0}", strErrorMsg);
					LogErrorMessage(strErrorMsg);
					return CookerTypes.EArchiveResult.Error;
				}
				else
				{
					if (fileDesc.Type != CookerTypes.EAssetType.Audio)
					{
						// JS? Copy and Add to index (no symlinks)
						if (fileDesc.Type == CookerTypes.EAssetType.ClientScript && !fileDesc.Name.EndsWith(".cs"))
						{
							// Copy unfortunately... JS doesnt respect symbolic links...
							lstThisResourceIndexAdditions.Add(String.Format("require('./{0}/{1}');", strArchiveName, fileDesc.Name));
							bWroteClientJSFile = true;

							File.Copy(strFileOnDiskPath, strFakeFilePath, true);
						}
						else
						{
							// TODO_COOKER: Remove when clientside symlinks work
							if ((fileDesc.Type == CookerTypes.EAssetType.ClientAsset || fileDesc.Type == CookerTypes.EAssetType.Audio || fileDesc.Type == CookerTypes.EAssetType.ClientScript))
							{
								File.Copy(strFileOnDiskPath, strFakeFilePath, true);
							}
							else
							{
								bool bCreated = CreateSymbolicLink(strFakeFilePath, strFileOnDiskPath, SymbolicLink.File | SymbolicLink.SYMBOLIC_LINK_FLAG_ALLOW_UNPRIVILEGED_CREATE);

								if (!bCreated)
								{
									LogErrorMessage("Failed to create symlink ({0}: {1} -> {2}) - this likely means you are not running your IDE as administrator!\nPress close and re-run as admin. This cook will fail.", fileDesc.Type.ToString(), strFakeFilePath, strFileOnDiskPath);
									return CookerTypes.EArchiveResult.Error;
								}
							}
						}
					}
					else
					{
						// unpack audio files
						// Is it a packed file list?
						if (bIsPackedAudioFile)
						{
							// Loop the files
							int index = 1;
							bool bAllFilesProcessed = false;
							while (!bAllFilesProcessed)
							{
								string strFileNameUnpacked = fileDesc.Name.Replace("XX", index.ToString());

								strFakeFileDir = Path.Combine(Directory.GetCurrentDirectory(), "..", "Output", CookerSettings.GetTargetName(), CookerHelpers.GetMountPathForArchive(strArchiveName, fileDesc.Type, Path.GetFileName(fileDesc.Name)));
								strFakeFilePath = Path.Combine(strFakeFileDir, Path.GetFileName(strFileNameUnpacked));
								strFileOnDiskDir = Path.Combine(Directory.GetCurrentDirectory(), strArchiveName);
								strFileOnDiskPath = Path.Combine(strFileOnDiskDir, strFileNameUnpacked);

								if (File.Exists(strFileOnDiskPath))
								{
									// TODO_COOKER: Remove when clientside symlinks work
									/*
									bool bCreated = CreateSymbolicLink(strFakeFilePath, strFileOnDiskPath, SymbolicLink.File | SymbolicLink.SYMBOLIC_LINK_FLAG_ALLOW_UNPRIVILEGED_CREATE);

									if (!bCreated)
									{
										LogErrorMessage("Failed to create symlink ({0}: {1} -> {2}) - this likely means you are not running your IDE as administrator!\nPress close and re-run as admin. This cook will fail.", fileDesc.Type.ToString(), strFakeFilePath, strFileOnDiskPath);
										return CookerTypes.EArchiveResult.Error;
									}
									*/

									File.Copy(strFileOnDiskPath, strFakeFilePath, true);
									++index;
								}
								else
								{
									bAllFilesProcessed = true;
								}
							}
						}
						else
						{
							// Add normally, its an individual file
							// TODO_COOKER: Remove when clientside symlinks work
							if ((fileDesc.Type == CookerTypes.EAssetType.ClientAsset || fileDesc.Type == CookerTypes.EAssetType.Audio || fileDesc.Type == CookerTypes.EAssetType.ClientScript))
							{
								File.Copy(strFileOnDiskPath, strFakeFilePath, true);
							}
							else
							{
								bool bCreated = CreateSymbolicLink(strFakeFilePath, strFileOnDiskPath, SymbolicLink.File | SymbolicLink.SYMBOLIC_LINK_FLAG_ALLOW_UNPRIVILEGED_CREATE);

								if (!bCreated)
								{
									LogErrorMessage("Failed to create symlink ({0}: {1} -> {2}) - this likely means you are not running your IDE as administrator!\nPress close and re-run as admin. This cook will fail.", fileDesc.Type.ToString(), strFakeFilePath, strFileOnDiskPath);
									return CookerTypes.EArchiveResult.Error;
								}
							}
						}
					}

					if (fileDesc.Type == CookerTypes.EAssetType.ServerScript)
					{
						xmlWriter.WriteStartElement("script");
						xmlWriter.WriteAttributeString("src", fileDesc.Name);
						xmlWriter.WriteAttributeString("type", "server");
						xmlWriter.WriteAttributeString("lang", "compiled");
						xmlWriter.WriteEndElement();
					}
					else if (fileDesc.Type == CookerTypes.EAssetType.ClientScript)
					{
						// NOTE: Nothing to do here for RAGE
					}
					else if (fileDesc.Type == CookerTypes.EAssetType.ClientAsset)
					{
						// NOTE: Nothing to do here for RAGE
					}
					else if (fileDesc.Type == CookerTypes.EAssetType.Audio)
					{
						// NOTE: Nothing to do here for RAGE
					}
					else if (fileDesc.Type == CookerTypes.EAssetType.ServerDependency)
					{
						// NOTE: Nothing to do here, it's just a dependency dll
					}
					else if (fileDesc.Type == CookerTypes.EAssetType.RageOverrideDLL)
					{
						// NOTE: Nothing to do here, it's just a rage override dll
					}
					else if (fileDesc.Type == CookerTypes.EAssetType.ServerAsset)
					{
						// NOTE: Nothing to do here, it's just a data file for copying
					}
					else if (fileDesc.Type == CookerTypes.EAssetType.DotNetConfigFile)
					{
						// NOTE: Nothing to do here, it's just a dot net config file
					}

				}
			}

			// Exported functions
			foreach (CookerTypes.AssetExportedFunctions funcDesc in Descriptor.ExportedFunctions)
			{
				string strOutputName = String.Format("{0}_{1}", funcDesc.Class, funcDesc.Function);
				//dictHashes.Add(strOutputName, CookerHelpers.HashString(strOutputName));

				xmlWriter.WriteStartElement("export");
				xmlWriter.WriteAttributeString("class", funcDesc.Class);
				xmlWriter.WriteAttributeString("function", funcDesc.Function);
				xmlWriter.WriteEndElement();
			}

			// Close meta
			xmlWriter.WriteEndDocument();
			xmlWriter.Close();

			// Clientside JS index
			if (bWroteClientJSFile)
			{
				// Write index
				File.WriteAllLines(Path.Combine(Directory.GetCurrentDirectory(), "..", "Output", CookerSettings.GetTargetName(), "client_packages", strArchiveName, "index.js"), lstThisResourceIndexAdditions.ToArray());
			}

			// write FIM cache
			string strDescriptorPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "Descriptors", m_strDescriptorFilePath);
			uint newHash = CookerHelpers.HashFile(m_strDescriptorFilePath);
			string strHashCachePath = Path.Combine(Directory.GetCurrentDirectory(), CookerSettings.HashCacheName, strArchiveName + CookerSettings.HashCacheExtensionFIM);
			File.WriteAllText(strHashCachePath, newHash.ToString());

			return CookerTypes.EArchiveResult.UpToDate;
		}
		else // NORMAL COOK
		{
			Dictionary<uint, string> dictDupeHashCheck = new Dictionary<uint, string>();
			Dictionary<string, string> dictDupeFiles = new Dictionary<string, string>();

			string outputFolder = Path.Combine(Directory.GetCurrentDirectory(), "..", "Output", CookerSettings.GetTargetName(), "Packages");

			BufferedWriteLine("\tArchiver: Building {0}", strArchiveName);

			List<byte> outBytes = new List<byte>();
			Dictionary<string, uint> dictHashes = new Dictionary<string, uint>();

			string strHashCachePath = Path.Combine(CookerSettings.HashCacheName, strArchiveName + CookerSettings.HashCacheExtension);

			// Check if we need to build or not?
			string Reason = string.Empty;
			bool bNeedsBuild = false;

			string strOutputFolder = Path.Combine(Directory.GetCurrentDirectory(), "..", "Output", CookerSettings.GetTargetName(), "Packages");
			string strOutputPath = Path.Combine(strOutputFolder, strArchiveName + CookerSettings.ArchiveExtension);

			if (File.Exists(strHashCachePath) && File.Exists(strOutputPath))
			{
				Dictionary<string, uint> oldDictHashes = JsonConvert.DeserializeObject<Dictionary<string, uint>>(File.ReadAllText(strHashCachePath));

				byte[] CurrentPackageBytes = File.ReadAllBytes(strOutputPath);

				int trueNumberOfTargetHashes = 0;
				foreach (CookerTypes.AssetFile fileDesc in Descriptor.Files)
				{
#if SUPPORT_PACKED_AUDIO_FILES
					bool bIsPackedAudioFile = (fileDesc.Type == CookerTypes.EAssetType.Audio && fileDesc.Name.EndsWith("-XX.ogg"));

					if (bIsPackedAudioFile)
					{
						// Loop the files
						int index = 1;
						bool bAllFilesProcessed = false;
						while (!bAllFilesProcessed)
						{
							string strFileNameUnpacked = fileDesc.Name.Replace("XX", index.ToString());
							string strSourcePathUnpacked = Path.Combine(strArchiveName, strFileNameUnpacked);
							if (File.Exists(strSourcePathUnpacked))
							{
								++index;
								++trueNumberOfTargetHashes;
							}
							else
							{
								bAllFilesProcessed = true;
							}
						}


					}
					else
#endif
					{
						trueNumberOfTargetHashes++;

						if (fileDesc.Type == CookerTypes.EAssetType.ServerScript)
						{
							// Increment by one more to account for the shadow packaged .config file
							trueNumberOfTargetHashes++;
						}
					}
				}

				// If our sizes do not match, we have either added or removed files meaning we need to rebuild!
				if (oldDictHashes.Count != (trueNumberOfTargetHashes + Descriptor.ExportedFunctions.Count))
				{
					Reason = "Archive now has less or more files.";
					bNeedsBuild = true;
				}
				else if (CurrentPackageBytes[0] != CookerSettings.ArchiveVersion) // archive was generated with a different version
				{
					Reason = "Archive version is different.";
					bNeedsBuild = true;
				}
				else
				{
					// Files
					foreach (CookerTypes.AssetFile fileDesc in Descriptor.Files)
					{
						// TODO_COOKER: Compare descriptors for file types too
						string strOutputName = fileDesc.Name;
						string strFilePath = fileDesc.Type == CookerTypes.EAssetType.ServerScript ? Path.Combine(strArchiveName, "bin", CookerSettings.GetTargetName(), CookerSettings.DotNetVersion, fileDesc.Name) : Path.Combine(strArchiveName, fileDesc.Name);

						// DUPE CHECK (only for clientside files to reduce download size, server side we don't really care)
						if ((fileDesc.Type == CookerTypes.EAssetType.ClientAsset || fileDesc.Type == CookerTypes.EAssetType.Audio || fileDesc.Type == CookerTypes.EAssetType.ClientScript) && File.Exists(strFilePath))
						{
							uint hash = CookerHelpers.HashFile(strFilePath);

							try
							{
								dictDupeHashCheck.Add(hash, strFilePath);
							}
							catch (ArgumentException)
							{
								string strConflictingFilePath = dictDupeHashCheck[hash];

								// we only really care if its in another resource for now... lots of achievements share an icon for example
								bool bShouldLogDupe = true;
								string[] strSplit = strFilePath.Split("\\");
								string[] strSplitConflict = strConflictingFilePath.Split("\\");
								if (strSplit.Length > 0 && strSplitConflict.Length > 0)
								{
									if (strSplit[0] == strSplitConflict[0])
									{
										bShouldLogDupe = false;
									}
								}

								if (bShouldLogDupe)
								{
									dictDupeFiles.Add(strFilePath, strConflictingFilePath);
								}
							}
						}

#if SUPPORT_PACKED_AUDIO_FILES
						bool bIsPackedAudioFile = (fileDesc.Type == CookerTypes.EAssetType.Audio && fileDesc.Name.EndsWith("-XX.ogg"));

						if (!bIsPackedAudioFile)
						{
#endif
							if (oldDictHashes.TryGetValue(fileDesc.Name, out uint oldHash))
							{
								if (!File.Exists(strFilePath) || CookerHelpers.HashFile(strFilePath) != oldHash)
								{
									Reason = "File Differs";
									bNeedsBuild = true;
									break;
								}
							}
							else
							{
								Reason = "No previous cook data for one or more files.";
								bNeedsBuild = true;
								break;
							}
#if SUPPORT_PACKED_AUDIO_FILES
						}
						else
						{
							// Loop the files
							int index = 1;
							bool bAllFilesProcessed = false;
							while (!bAllFilesProcessed)
							{
								string strFileNameUnpacked = fileDesc.Name.Replace("XX", index.ToString());
								string strSourcePathUnpacked = Path.Combine(strArchiveName, strFileNameUnpacked);

								if (!File.Exists(strSourcePathUnpacked))
								{
									bAllFilesProcessed = true;
								}
								else
								{
									if (oldDictHashes.TryGetValue(strFileNameUnpacked, out uint oldHash))
									{
										if (CookerHelpers.HashFile(strSourcePathUnpacked) != oldHash)
										{
											Reason = "File Differs";
											bNeedsBuild = true;
											break;
										}
									}
									else
									{
										Reason = "No previous cook data for one or more files.";
										bNeedsBuild = true;
										break;
									}

									++index;
								}
							}
						}
#endif




						// Server script? also check .config
						if (fileDesc.Type == CookerTypes.EAssetType.ServerScript)
						{
							string strConfigFilename = fileDesc.Name + ".config";
							string strFileConfigPath = Path.Combine(strArchiveName, "bin", CookerSettings.GetTargetName(), CookerSettings.DotNetVersion, strConfigFilename);
							if (oldDictHashes.TryGetValue(strConfigFilename, out uint oldHashConfig))
							{
								if (!File.Exists(strFileConfigPath) || CookerHelpers.HashFile(strFileConfigPath) != oldHashConfig)
								{
									Reason = "File Config Differs";
									bNeedsBuild = true;
									break;
								}
							}
							else
							{
								Reason = "No previous cook data for one or more files.";
								bNeedsBuild = true;
								break;
							}
						}
					}

					// TODO_COOKER: Remove any asset or function that WAs in the cache but isnt in the .json anymore


					// Functions
					foreach (CookerTypes.AssetExportedFunctions funcDesc in Descriptor.ExportedFunctions)
					{
						string strOutputName = String.Format("{0}_{1}", funcDesc.Class, funcDesc.Function);
						string strFilePath = Path.Combine(strArchiveName, strOutputName);

						if (oldDictHashes.TryGetValue(strOutputName, out uint oldHash))
						{
							// NOTE: Nothing to compare here, if its in the hash we are fine since it's just a string, no data
						}
						else
						{
							Reason = "No previous cook data for one or more exported functions.";
							bNeedsBuild = true;
							break;
						}
					}
				}
			}
			else
			{
				Reason = "No previous cook data.";
				bNeedsBuild = true;
			}

			if (!bNeedsBuild)
			{
				BufferedWriteLine("\t\tArchive has not changed!\n");

			}
			else
			{
				BufferedWriteLine("\t\tReason for Recook: {0}\n", Reason);

				// Meta.xml
				XmlWriterSettings xmlSettings = new XmlWriterSettings
				{
					Indent = true,
					IndentChars = "     ",
					NewLineOnAttributes = false,
					OmitXmlDeclaration = true
				};

				string strMetaFolder = Path.Combine(CookerSettings.TempFolder, strArchiveName);
				string strMetaPath = Path.Combine(strMetaFolder, "meta.xml");
				Directory.CreateDirectory(strMetaFolder);
				XmlWriter xmlWriter = XmlWriter.Create(strMetaPath, xmlSettings);

				xmlWriter.WriteStartDocument();
				xmlWriter.WriteStartElement("meta");
				xmlWriter.WriteStartElement("info");
				xmlWriter.WriteAttributeString("name", strArchiveName);
				xmlWriter.WriteAttributeString("author", "Owl Development Team");
				xmlWriter.WriteAttributeString("type", "script");
				xmlWriter.WriteEndElement();

				outBytes.Add(CookerSettings.ArchiveVersion);

				// Files
				foreach (CookerTypes.AssetFile fileDesc in Descriptor.Files)
				{
					// calculate source
					string strSourcePath = "";

					if (fileDesc.Type == CookerTypes.EAssetType.ServerScript)
					{
						strSourcePath = Path.Combine(strArchiveName, "bin", CookerSettings.GetTargetName(), CookerSettings.DotNetVersion, fileDesc.Name);
					}
					else if (fileDesc.Type == CookerTypes.EAssetType.MapFile_Interior || fileDesc.Type == CookerTypes.EAssetType.MapFile_Persistent)
					{
						strSourcePath = Path.Combine(Job_CookMap.GetMapDirectory(), fileDesc.Name);
					}
					else
					{
						strSourcePath = Path.Combine(strArchiveName, fileDesc.Name);
					}

#if SUPPORT_PACKED_AUDIO_FILES
					bool bIsPackedAudioFile = (fileDesc.Type == CookerTypes.EAssetType.Audio && fileDesc.Name.EndsWith("-XX.ogg"));
#else
				bool bIsPackedAudioFile = false;
#endif

					if (!File.Exists(strSourcePath) && !bIsPackedAudioFile)
					{
						string strErrorMsg = String.Format("File {0} does not exist on disk. ({1})", strSourcePath, strArchiveName);
						BufferedWriteLine("\t\t\tERROR: {0}", strErrorMsg);
						LogErrorMessage(strErrorMsg);
						return CookerTypes.EArchiveResult.Error;
					}
					else
					{
						if (fileDesc.Type != CookerTypes.EAssetType.Audio)
						{
							AppendFileToArchive(fileDesc.Name, Path.GetFileName(fileDesc.Name), fileDesc.Type, strSourcePath, ref dictHashes, ref outBytes);
						}
						else
						{
							// unpack audio files
							// Is it a packed file list?
							if (bIsPackedAudioFile)
							{
								// Loop the files
								int index = 1;
								bool bAllFilesProcessed = false;
								while (!bAllFilesProcessed)
								{
									string strFileNameUnpacked = fileDesc.Name.Replace("XX", index.ToString());
									string strSourcePathUnpacked = Path.Combine(strArchiveName, strFileNameUnpacked);
									if (File.Exists(strSourcePathUnpacked))
									{
										AppendFileToArchive(strFileNameUnpacked, Path.GetFileName(strFileNameUnpacked), fileDesc.Type, strSourcePathUnpacked, ref dictHashes, ref outBytes);
										++index;
									}
									else
									{
										bAllFilesProcessed = true;
									}
								}
							}
							else
							{
								// Add normally, its an individual file
								AppendFileToArchive(fileDesc.Name, Path.GetFileName(fileDesc.Name), fileDesc.Type, strSourcePath, ref dictHashes, ref outBytes);
							}
						}

						// Must also append .config for .dlls
						if (fileDesc.Type == CookerTypes.EAssetType.ServerScript)
						{
							string strConfigPath = Path.Combine(strArchiveName, "bin", CookerSettings.GetTargetName(), CookerSettings.DotNetVersion, fileDesc.Name + ".config");
							if (!File.Exists(strConfigPath))
							{
								string strErrorMsg = String.Format("File {0} does not exist on disk. ({1}). All .dlls must have a .config alongside it.", strConfigPath, strArchiveName);
								LogErrorMessage(strErrorMsg);
								return CookerTypes.EArchiveResult.Error;
							}
							else
							{
								AppendFileToArchive(fileDesc.Name + ".config", Path.GetFileName(fileDesc.Name) + ".config", CookerTypes.EAssetType.DotNetConfigFile, strConfigPath, ref dictHashes, ref outBytes);
							}
						}

						if (fileDesc.Type == CookerTypes.EAssetType.ServerScript)
						{
							xmlWriter.WriteStartElement("script");
							xmlWriter.WriteAttributeString("src", fileDesc.Name);
							xmlWriter.WriteAttributeString("type", "server");
							xmlWriter.WriteAttributeString("lang", "compiled");
							xmlWriter.WriteEndElement();
						}
						else if (fileDesc.Type == CookerTypes.EAssetType.ClientScript)
						{
							// NOTE: Nothing to do here for RAGE
						}
						else if (fileDesc.Type == CookerTypes.EAssetType.ClientAsset)
						{
							// NOTE: Nothing to do here for RAGE
						}
						else if (fileDesc.Type == CookerTypes.EAssetType.Audio)
						{
							// NOTE: Nothing to do here for RAGE
						}
						else if (fileDesc.Type == CookerTypes.EAssetType.ServerDependency)
						{
							// NOTE: Nothing to do here, it's just a dependency dll
						}
						else if (fileDesc.Type == CookerTypes.EAssetType.RageOverrideDLL)
						{
							// NOTE: Nothing to do here, it's just a rage override dll
						}
						else if (fileDesc.Type == CookerTypes.EAssetType.ServerAsset)
						{
							// NOTE: Nothing to do here, it's just a data file for copying
						}
						else if (fileDesc.Type == CookerTypes.EAssetType.DotNetConfigFile)
						{
							// NOTE: Nothing to do here, it's just a dot net config file
						}
					}
				}

				// Exported functions
				foreach (CookerTypes.AssetExportedFunctions funcDesc in Descriptor.ExportedFunctions)
				{
					string strOutputName = String.Format("{0}_{1}", funcDesc.Class, funcDesc.Function);
					dictHashes.Add(strOutputName, CookerHelpers.HashString(strOutputName));

					xmlWriter.WriteStartElement("export");
					xmlWriter.WriteAttributeString("class", funcDesc.Class);
					xmlWriter.WriteAttributeString("function", funcDesc.Function);
					xmlWriter.WriteEndElement();
				}

				// Close meta
				xmlWriter.WriteEndDocument();
				xmlWriter.Close();

				// append Meta.xml
				// No meta for platform archives
				bool IsPlatformArchive = strArchiveName.ToLower().Contains("_windows") || strArchiveName.ToLower().Contains("_linux") || strArchiveName.ToLower().Contains("_crossplatform");
				if (!IsPlatformArchive)
				{
					AppendFileToArchive("meta.xml", "meta.xml", CookerTypes.EAssetType.Meta, strMetaPath, ref dictHashes, ref outBytes, false);
				}

				File.WriteAllText(strHashCachePath, JsonConvert.SerializeObject(dictHashes));


				File.WriteAllBytes(strOutputPath, outBytes.ToArray());
				BufferedWriteLine("\t Finished!\n");
			}

			string outputFileName = strArchiveName + CookerSettings.ArchiveExtension;

			if (File.Exists(outputFolder + outputFileName))
			{
				File.Delete(outputFolder + outputFileName);
			}

			return bNeedsBuild ? CookerTypes.EArchiveResult.NeedsUpdate : CookerTypes.EArchiveResult.UpToDate;
		}
	}

	private void AppendFileToArchive(string hashKey, string fileName, CookerTypes.EAssetType assetType, string strFilePath, ref Dictionary<string, uint> dictHashes, ref List<byte> outBytes, bool bWriteToHashCache = true)
	{
		string strOutputName = fileName;

		// Fill out hashcache
		if (bWriteToHashCache)
		{
			dictHashes.Add(hashKey, CookerHelpers.HashFile(strFilePath));
		}

		byte[] bytes = File.ReadAllBytes(strFilePath);
		BufferedWriteLine("\t\t > {0}", fileName);

		// write file name
		byte[] filenameBytes = CookerHelpers.GetBytes(strOutputName);
		outBytes.Add(Convert.ToByte(filenameBytes.Length));
		foreach (byte b in filenameBytes)
		{
			outBytes.Add(b);
		}

		// Write asset type
		byte[] bytesAssetType = BitConverter.GetBytes(Convert.ToUInt32(assetType));
		foreach (byte b in bytesAssetType)
		{
			outBytes.Add(b);
		}

		// write size
		byte[] bytesFileLen = BitConverter.GetBytes(bytes.ToArray().Length);
		foreach (byte b in bytesFileLen)
		{
			outBytes.Add(b);
		}

		// write data
		foreach (byte b in bytes)
		{
			outBytes.Add(b);
		}
	}

	public override string Describe()
	{
		return String.Format("[{0}] - {1}", this.GetType().Name, m_strDescriptorFilePath);
	}
}
