using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;

public class Job_ClientCodeGen : JobBase
{
	private string m_strName;

	public Job_ClientCodeGen(string strName) : base()
	{
		m_strName = strName;
	}

	public override bool Execute()
	{
		string strInputFileName = String.Format("{0}.json", m_strName);
		string strOutputFileName = String.Format("{0}.cs", m_strName);

		string inputPath = Path.Combine(Directory.GetCurrentDirectory(), "owl_gamedata", strInputFileName);
		string sanitizedJsonData = String.Empty;

		// Parse and check for error
		try
		{
			var obj = JsonConvert.DeserializeObject(File.ReadAllText(inputPath));
			sanitizedJsonData = JsonConvert.SerializeObject(obj); // re-serialize it to sanitize comments etc.
		}
		catch (Exception ex)
		{
			BufferedWriteLine(String.Format("Error Parsing: {0} - {1}", strInputFileName, ex.Message));
			LogErrorMessage(String.Format("Error Parsing: {0} - {1}", strInputFileName, ex.Message));
			return false;
		}

		string itemData = sanitizedJsonData
		.Replace("\r\n", string.Empty)
		.Replace("\t", string.Empty)
		.Replace("\"", "\"\"");

		string strCodePath = Path.Combine(Directory.GetCurrentDirectory(), "owl_shared", strOutputFileName);
		int i = 0;
		string[] dataCodeFile = File.ReadAllLines(strCodePath);
		foreach (string line in dataCodeFile)
		{
			if (line.Contains(String.Format("public const string {0}", m_strName)))
			{
				dataCodeFile[i] = String.Format("\tpublic const string {0} = @\"{1}\";", m_strName, itemData);
				break;
			}
			++i;
		}

		File.WriteAllLines(strCodePath, dataCodeFile.ToArray());

		if (CookerSettings.IsFastIterationMode())
		{
			// write FIM cache
			uint newHash = CookerHelpers.HashFile(inputPath);
			string strHashCachePath = Path.Combine(Directory.GetCurrentDirectory(), CookerSettings.HashCacheName, String.Format("{0}{1}", m_strName, CookerSettings.HashCacheExtensionFIM));
			File.WriteAllText(strHashCachePath, newHash.ToString());
		}

		return true;
	}

	public override string Describe()
	{
		return String.Format("[{0}] - {1}", this.GetType().Name, m_strName);
	}
}
