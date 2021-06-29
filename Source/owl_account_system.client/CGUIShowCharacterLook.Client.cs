internal class CGUIShowCharacterLook : CEFCore
{
	public CGUIShowCharacterLook(OnGUILoadedDelegate callbackOnLoad) : base("owl_account_system.client/show-character-look.html", EGUIID.ShowCharacterLook, callbackOnLoad)
	{

	}

	public override void OnLoad()
	{

	}

	public void SetData(string charactername, int age, int height,
		int weight, string physicalAppearance, string scars, string tattoos, string makeup, int createdAt,
		int updatedAt)
	{
		Execute("SetData", charactername, age, height, weight, physicalAppearance, scars, tattoos, makeup, createdAt, updatedAt);
	}
}