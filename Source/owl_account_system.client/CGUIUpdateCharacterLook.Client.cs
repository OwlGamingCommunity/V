internal class CGUIUpdateCharacterLook : CEFCore
{
	public CGUIUpdateCharacterLook(OnGUILoadedDelegate callbackOnLoad) : base("owl_account_system.client/update-character-look.html", EGUIID.UpdateCharacterLook, callbackOnLoad)
	{

	}

	public override void OnLoad()
	{

	}

	public void SetData(string charactername, int height,
		int weight, string physicalAppearance, string scars, string tattoos, string makeup, int createdAt,
		int updatedAt)
	{
		Execute("SetData", charactername, height, weight, physicalAppearance, scars, tattoos, makeup, createdAt, updatedAt);
	}
}