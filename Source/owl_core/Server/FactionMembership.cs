// TODO: We could speed up faction management queries a lot by using DBID from here
public class CFactionMembership
{
	public CFactionMembership(CFaction a_Faction, bool a_bManager, int a_Rank)
	{
		Faction = a_Faction;
		Manager = a_bManager;
		Rank = a_Rank;
	}

	public CFaction Faction { get; set; }
	public bool Manager { get; set; }
	public int Rank { get; set; }
}