public class CGUIPlayerRadialMenu : CEFCore
{
	public CGUIPlayerRadialMenu(OnGUILoadedDelegate callbackOnLoad) : base("owl_entityinteractions.client/player_radialmenu.html", EGUIID.PlayerRadialMenu, callbackOnLoad)
	{

	}

	public override void OnLoad()
	{

	}

	public void Show(bool playerMenu = true, bool vehicleMenu = false, bool pedMenu = false)
	{
		Execute("Show", playerMenu, vehicleMenu, pedMenu);
	}

	public void Hide()
	{
		Execute("Hide");
	}
}