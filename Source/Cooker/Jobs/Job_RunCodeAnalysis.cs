using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class Job_RunCodeAnalysis : JobBase
{
	private string m_strArchiveDirectory;

	public Job_RunCodeAnalysis(string strArchiveDirectory) : base()
	{
		m_strArchiveDirectory = strArchiveDirectory;
	}

	public override bool Execute()
	{
		bool bSuccess = true;

		// some directories (e.g. owl_maps) are descriptor only and dont have a source folder
		if (Directory.Exists(m_strArchiveDirectory))
		{
			// add sub dirs
			List<string> lstDirectories = Directory.GetDirectories(m_strArchiveDirectory).ToList();

			// add root dir
			lstDirectories.Add(m_strArchiveDirectory);

			foreach (string dirPath in lstDirectories)
			{
				foreach (string strFilePath in Directory.GetFiles(dirPath))
				{
					bool bIsCSharpFile = strFilePath.EndsWith(".cs");
					bool bIsJSFile = strFilePath.EndsWith(".js");
					bool bIsHTMLFile = strFilePath.EndsWith(".html");
					bool bIsSourceFile = bIsCSharpFile || bIsJSFile || bIsHTMLFile;

					if (bIsSourceFile)
					{
						if (bIsJSFile)
						{
							if (!CodeAnalysis.g_lstUIJavascriptFiles.Contains(Path.GetFileName(strFilePath).ToLower()))
							{
								CodeAnalysis.AddJS();
							}
						}

						// TODO_COOKER: Double IO here, could use bytes from the package function
						string[] strContents = File.ReadAllLines(strFilePath);

						CodeAnalysis.AddFile();
						CodeAnalysis.AddLines(strContents.Length);

						int htmlNumCoresFound = 0;
						const int htmlNumCoresExpected = 1;

						foreach (string strLine in strContents)
						{
							string strLower = strLine.ToLower();
							int startIndex = strLower.IndexOf("// todo"); // +3

							if (startIndex != -1)
							{
								int endIndex = strLower.IndexOf(':', startIndex); // -1

								if (endIndex != -1)
								{
									startIndex += 3;
									string todo = strLower.Substring(startIndex, endIndex - startIndex);
									CodeAnalysis.AddTodo(todo);
								}
							}

							// Do we have an exported?
							if (strLower.Contains("api.exported"))
							{
								CodeAnalysis.AddExported();
							}

							// legacy mysql
							int startIndexSQL = strLower.IndexOf("database.legacyfunctions.");
							if (startIndexSQL != -1)
							{
								string strFunctionName = strLower;
								int endIndexSQL = strLower.IndexOf('(', startIndexSQL);

								if (endIndexSQL != -1)
								{
									strFunctionName = strLine.Substring(startIndexSQL, endIndexSQL - startIndexSQL).Replace("\t", "");
								}

								CodeAnalysis.AddLegacySQL(strFunctionName);
							}

							// Are we using API rather than NAPI?
							if (strLower.Contains("api.") && !strLower.Contains("napi."))
							{
								CodeAnalysis.AddLegacy();
							}

							// HTML files must contain one package:// core.js, one for debug one for release
							if (bIsHTMLFile)
							{
								if (strLower.Contains("package://owl_client_shared//core.js"))
								{
									htmlNumCoresFound++;
								}
							}

							if (!strFilePath.Contains("Program.cs") && (bIsHTMLFile || bIsCSharpFile))
							{
								foreach (var strBannedWordKeyPair in CodeAnalysis.g_strBannedCodeAnalysisWords)
								{
									if (strLower.Contains(strBannedWordKeyPair.Key.ToLower()) && strLower.IndexOf("//") == -1)
									{
										// Is this file allowed?
										if (!strBannedWordKeyPair.Value.Contains(Path.GetFileName(strFilePath)))
										{
											LogErrorMessage(String.Format("File {0} has banned code analysis phrase {1}", strFilePath, Path.GetFileName(strBannedWordKeyPair.Key)));
											bSuccess = false;
										}
									}
								}
							}
						}

						if (bIsHTMLFile && htmlNumCoresFound != htmlNumCoresExpected)
						{
							if (!CodeAnalysis.g_strHTMLFilesAllowedNoCore.Contains(Path.GetFileName(strFilePath)))
							{
								LogErrorMessage(String.Format("File {0} does not have two includes of package://owl_client_shared//core.js", strFilePath));
								bSuccess = false;
							}
						}
					}
				}
			}
		}

		return bSuccess;
	}

	public override string Describe()
	{
		return String.Format("[{0}] - {1}", this.GetType().Name, m_strArchiveDirectory);
	}
}
