using System;
using System.IO;
using System.Xml;

public class Job_CookSettingsXML : JobBase
{
	public Job_CookSettingsXML() : base()
	{

	}

	public static string GetOutputPath()
	{
		string strSettingsDir = Path.Combine(Directory.GetCurrentDirectory(), "..", "Output", CookerSettings.GetTargetName(), "Packages");
		string strSettingsPath = Path.Combine(strSettingsDir, "settings.xml");

		// For FIM, we have to deploy straight into the target folder since no unarchive step is present
		if (CookerSettings.IsFastIterationMode())
		{
			strSettingsDir = Path.Combine(Directory.GetCurrentDirectory(), "..", "Output", CookerSettings.GetTargetName(), "dotnet");
			strSettingsPath = Path.Combine(strSettingsDir, "settings.xml");
		}

		if (!Directory.Exists(strSettingsDir))
		{
			Directory.CreateDirectory(strSettingsDir);
		}

		return strSettingsPath;
	}

	public override bool Execute()
	{
		XmlWriterSettings xmlSettings = new XmlWriterSettings
		{
			Indent = true,
			IndentChars = "     ",
			NewLineOnAttributes = false,
			OmitXmlDeclaration = false
		};

		string strSettingsPath = GetOutputPath();
		if (File.Exists(strSettingsPath))
		{
			File.Delete(strSettingsPath);
		}

		XmlWriter xmlWriter = XmlWriter.Create(strSettingsPath, xmlSettings);

		xmlWriter.WriteStartDocument();

		xmlWriter.WriteStartElement("config");

		xmlWriter.WriteStartElement("acl_enabled");
		xmlWriter.WriteValue(true);
		xmlWriter.WriteEndElement();

		xmlWriter.WriteStartElement("resource");
		xmlWriter.WriteAttributeString("src", "owl_script_loader");
		xmlWriter.WriteEndElement();

		xmlWriter.WriteEndElement();

		xmlWriter.WriteEndDocument();
		xmlWriter.Close();

		return true;
	}

	public override string Describe()
	{
		return String.Format("{0}", this.GetType().Name);
	}
}
