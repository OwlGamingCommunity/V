internal class CGUISprayCanPrompt : CEFCore
{
	public CGUISprayCanPrompt(OnGUILoadedDelegate callbackOnLoad) : base("owl_items.client/spraycanprompt.html", EGUIID.SprayCanPrompt, callbackOnLoad)
	{
		UIEvents.SprayCan_TagWall += () => { ItemSystem.GetGangTags()?.OnTagWall(); };
		UIEvents.SprayCan_EditTag += () => { ItemSystem.GetGangTags()?.OnEditTag(); };
		UIEvents.SprayCan_ShareTag += () => { ItemSystem.GetGangTags()?.OnShareTag(); };
		UIEvents.SprayCan_Exit += () => { ItemSystem.GetGangTags()?.OnExitSprayCan(); };
	}

	public override void OnLoad()
	{

	}
}