internal class CGUIRoadblockEditor : CEFCore
{
	public CGUIRoadblockEditor(OnGUILoadedDelegate callbackOnLoad) : base("owl_factions.client/roadblockeditor.html", EGUIID.RoadblockEditor, callbackOnLoad)
	{
		UIEvents.RoadblockEditor_Hide += () => { FactionSystem.GetRoadblockSystem()?.OnExit(); };
	}

	public override void OnLoad()
	{

	}
	public void AddRoadblockType(int index, string strDisplayName)
	{
		Execute("AddRoadblockType", index, strDisplayName);
	}

	public void CommitRoadblockTypes()
	{
		Execute("CommitRoadblockTypes");
	}
}