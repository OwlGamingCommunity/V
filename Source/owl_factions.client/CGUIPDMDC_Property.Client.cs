internal class CGUIPDMDC_Property : CEFCore
{
	public CGUIPDMDC_Property(OnGUILoadedDelegate callbackOnLoad) : base("owl_factions.client/mdc_property.html", EGUIID.MDCProperty, callbackOnLoad)
	{

	}

	public override void OnLoad()
	{

	}

	public void ShowTerminal(CMdtProperty propertyInfo)
	{
		Execute("ShowTerminal", propertyInfo.id, propertyInfo.name, propertyInfo.owner, propertyInfo.owner_name, propertyInfo.renter, propertyInfo.renter_name, propertyInfo.entrance_x, propertyInfo.entrance_y, propertyInfo.entrance_z, propertyInfo.entrance_dimension, propertyInfo.buy_price, propertyInfo.rent_price);
	}
}