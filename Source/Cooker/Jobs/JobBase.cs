using System.Collections.Generic;

public abstract class JobBase
{
	private List<string> m_lstBufferedIO = new List<string>();
	private List<string> m_lstErrors = new List<string>();

	protected void BufferedWriteLine(string strFormat, params string[] strParams)
	{
		m_lstBufferedIO.Add(string.Format(strFormat, strParams));
	}

	public List<string> GetBufferedIO()
	{
		return m_lstBufferedIO;
	}

	public List<string> GetErrors()
	{
		return m_lstErrors;
	}

	protected void LogErrorMessage(string strFormat, params string[] strParams)
	{
		m_lstErrors.Add(string.Format(strFormat, strParams));
	}

	public abstract bool Execute();
	public abstract string Describe();
}
