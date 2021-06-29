using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class Job_Unarchive : JobBase
{
	private string m_strArchive;
	public Job_Unarchive(string strArchive) : base()
	{
		m_strArchive = strArchive;
	}

	public override bool Execute()
	{
		// If it's a platform file, only unarchive it if this is the correct platform. cross platform is always unarchived
		bool IsLinuxArchive = m_strArchive.ToLower().Contains("_linux");
		bool IsWindowsArchive = m_strArchive.ToLower().Contains("_windows");

		// Is this a platform archive?
		if (IsWindowsArchive || IsLinuxArchive)
		{
			// Are we on the correct platform?
			if (IsWindowsArchive && !CookerSettings.IsWindows())
			{
				Console.WriteLine("Ignoring {0} as the package is not for this OS.", Path.GetFileNameWithoutExtension(m_strArchive));

				// false is an error, so lets pretend everything is fine
				return true;
			}

			if (IsLinuxArchive && !CookerSettings.IsLinux())
			{
				Console.WriteLine("Ignoring {0} as the package is not for this OS.", Path.GetFileNameWithoutExtension(m_strArchive));

				// false is an error, so lets pretend everything is fine
				return true;
			}
		}

		List<string> g_lstThisResourceIndexAdditions = new List<string>();
		bool bWroteClientJSFile = false;
		try
		{
			byte[] bytes = File.ReadAllBytes(m_strArchive);

			// check version
			byte Version = bytes[0];
			if (Version != CookerSettings.ArchiveVersion)
			{
				// TODO_COOKER: log error
				return false;
			}

			// we now have the true length:
			int bytesProcessed = 0;
			int bytesTotal = bytes.Length - 1; // ignore versioning
			int fileOffset = 1; // 1 to skip version

			// now the file is raw
			while (bytesProcessed < bytesTotal)
			{
				List<byte> outBytes = new List<byte>();

				// Get filename length
				byte byteFileNameLength = bytes[fileOffset];
				int dataLen = Convert.ToInt32(byteFileNameLength);
				byte[] bytesFileName = bytes.Skip(fileOffset + 1).Take(dataLen).ToArray();

				CookerTypes.EAssetType assetType = (CookerTypes.EAssetType)bytes.Skip(dataLen + fileOffset + 1).Take(sizeof(uint)).ToArray()[0];

				// Get file length
				int fileLen = BitConverter.ToInt32(bytes.Skip(dataLen + fileOffset + 1 + sizeof(uint)).Take(sizeof(int)).ToArray(), 0);

				int individualBytesOffset = sizeof(uint) + sizeof(int);
				int startPos = dataLen + fileOffset + 1 + individualBytesOffset;

				outBytes = bytes.Skip(startPos).Take(fileLen).ToList();

				bool bArchiveNameContainsFolderName = CookerHelpers.GetString(bytesFileName).Contains(Path.GetFileNameWithoutExtension(m_strArchive) + @"\");

				string strOutput = Path.Combine(CookerHelpers.GetMountPathForArchive(bArchiveNameContainsFolderName ? "" : Path.GetFileNameWithoutExtension(m_strArchive), assetType, CookerHelpers.GetString(bytesFileName)), CookerHelpers.GetString(bytesFileName));

				// Do not write dependencies or overrides
				if (assetType != CookerTypes.EAssetType.ServerDependency && assetType != CookerTypes.EAssetType.RageOverrideDLL)
				{
					Directory.CreateDirectory(Path.GetDirectoryName(strOutput));
					try { File.Delete(strOutput); } catch { };
					File.WriteAllBytes(strOutput, outBytes.ToArray());

					// If clientside, strip comments
#if RELEASE
						if (assetType == CookerTypes.EAssetType.ClientScript || (assetType == CookerTypes.EAssetType.ClientAsset && CookerHelpers.GetString(bytesFileName).EndsWith(".html")))
						{
							// Don't check itemdata.json, its safe for comments as its just JSON
							if (!CookerHelpers.GetString(bytesFileName).Equals("ItemData.cs"))
							{
								bool bReplaced = false;
								string[] strContents = File.ReadAllLines(strOutput);
								for (int i = 0; i < strContents.Length; ++i)
								{
									string strLine = strContents[i].ToLower();
									int startIndex = strLine.IndexOf("// todo");

									if (startIndex != -1)
									{
										// replace the comment part
										strContents[i] = strContents[i].Remove(startIndex, strLine.Length - startIndex);
										bReplaced = true;
									}

									if (strLine.Contains("console.log"))
									{
										strContents[i] = "";
										bReplaced = true;
									}
								}

								if (bReplaced)
								{
									File.WriteAllLines(strOutput, strContents.ToArray());
								}
							}
						}
#endif
				}

				bool bServerSide = CookerHelpers.IsFileServerside(CookerHelpers.GetString(bytesFileName), assetType);
				// Is it a server DLL or .config? Copy it to runtime too for sanity...
				if (bServerSide && ((assetType == CookerTypes.EAssetType.ServerDependency || assetType == CookerTypes.EAssetType.RageOverrideDLL)))
				{
					string strRuntimeOutput = Path.Combine(CookerHelpers.GetRuntimePath(), CookerHelpers.GetString(bytesFileName));
					try { File.Delete(strRuntimeOutput); } catch { };
					File.WriteAllBytes(strRuntimeOutput, outBytes.ToArray());
				}

				// Add to index
				if (!bServerSide && assetType == CookerTypes.EAssetType.ClientScript)
				{
					if (!CookerHelpers.GetString(bytesFileName).EndsWith(".cs"))
					{
						g_lstThisResourceIndexAdditions.Add(String.Format("require('./{0}/{1}');", Path.GetFileNameWithoutExtension(m_strArchive), CookerHelpers.GetString(bytesFileName)));
						bWroteClientJSFile = true;
					}
				}

				bytesProcessed += dataLen + fileLen + 1 + individualBytesOffset;
				fileOffset += dataLen + fileLen + 1 + individualBytesOffset;

				outBytes = null;
			}
		}
		catch
		{
			return false;
		}

		if (bWroteClientJSFile)
		{
			// Write index
			File.WriteAllLines(Path.Combine("client_packages", Path.GetFileNameWithoutExtension(m_strArchive), "index.js"), g_lstThisResourceIndexAdditions.ToArray());
		}

		return true;
	}

	public override string Describe()
	{
		return String.Format("[{0}] - {1}", this.GetType().Name, m_strArchive);
	}
}
