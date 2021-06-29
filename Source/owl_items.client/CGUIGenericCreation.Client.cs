internal class CGUIGenericCreation : CEFCore
{
	public CGUIGenericCreation(OnGUILoadedDelegate callbackOnLoad) : base("owl_items.client/genericcreator.html", EGUIID.GenericCreator, callbackOnLoad)
	{
		UIEvents.Generic_SpawnGenerics += OnSpawnGenerics;
		UIEvents.Generic_CloseGenericsUI += () => { ItemSystem.GetGenericSystem().OnCloseGenericsUI(); };
	}

	public override void OnLoad()
	{

	}

	private void OnSpawnGenerics(string name, string model, string amount, string price)
	{
		int genericsAmount = int.TryParse(amount, out genericsAmount) ? genericsAmount : 1;
		float genericsPrice = float.TryParse(price, out genericsPrice) ? genericsPrice : 0.0f;
		string objectModel = RAGE.Game.Streaming.IsModelValid(HashHelper.GetHashUnsigned(model)) ? model : "hei_prop_drug_statue_box_big";

		NetworkEventSender.SendNetworkEvent_Generics_SpawnGenerics(name, objectModel, genericsAmount, genericsPrice);
	}

	public void Reset()
	{
		Execute("Reset");
	}
}