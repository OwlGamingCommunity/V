using EntityDatabaseID = System.Int64;

public class CFactionRank : CBaseEntity
{
	public CFactionRank(EntityDatabaseID a_DBID, string a_strName, float a_fSalary)
	{
		m_DatabaseID = a_DBID;
		Name = a_strName;
		Salary = a_fSalary;
	}

	public string Name { get; }
	public float Salary { get; }
}