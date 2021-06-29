internal class CGUICustomMapLoader : CEFCore
{
	public CGUICustomMapLoader(OnGUILoadedDelegate callbackOnLoad) : base("owl_maps.client/maploader.html", EGUIID.MapLoader, callbackOnLoad)
	{
		UIEvents.CustomInterior_CloseWindow += () => { CustomMapSystem.GetCustomMapLoader().OnCloseCustomMapUI(); };
		UIEvents.CustomInterior_ProcessCustomInterior += (string mapData, string mapType, float markerX, float markerY, float markerZ) => { CustomMapSystem.GetCustomMapLoader().ProcessCustomInterior(mapData, mapType, markerX, markerY, markerZ); };
	}

	public override void OnLoad()
	{

	}
}