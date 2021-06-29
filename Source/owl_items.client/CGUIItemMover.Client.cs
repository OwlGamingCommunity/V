using System.Globalization;

internal class CGUIItemMover : CEFCore
{

	private RAGE.Elements.MapObject m_GenericItemToUpdate = null;
	private RAGE.Vector3 m_GenericItemOldPosition = new RAGE.Vector3(0, 0, 0);
	private RAGE.Vector3 m_GenericItemOldRotation = new RAGE.Vector3(0, 0, 0);
	public CGUIItemMover(OnGUILoadedDelegate callbackOnLoad) : base("owl_items.client/itemmover.html", EGUIID.ItemMover, callbackOnLoad)
	{
		UIEvents.Generic_CloseItemMoverUI += OnCloseItemMoverUI;
		UIEvents.Generic_UpdateGenericPreviewPosition += OnUpdateGenericPreview;
		UIEvents.Generic_UpdateGenericPosition += OnSaveObjectPosition;
	}

	public override void OnLoad()
	{

	}

	public void Initialize(RAGE.Elements.MapObject mapObject)
	{
		RAGE.Vector3 rotation = mapObject.GetRotation(0);
		Execute("loadItemData", mapObject.Position.X, mapObject.Position.Y, mapObject.Position.Z, rotation.X, rotation.Y, rotation.Z);

		m_GenericItemToUpdate = mapObject;
		m_GenericItemOldPosition = mapObject.Position;
		m_GenericItemOldRotation = mapObject.GetRotation(0);
	}
	public void OnCloseItemMoverUI()
	{
		SetVisible(false, false, false);
		Execute("Reset");

		m_GenericItemToUpdate.Position = m_GenericItemOldPosition;
		m_GenericItemToUpdate.SetRotation(m_GenericItemOldRotation.X, m_GenericItemOldRotation.Y, m_GenericItemOldRotation.Z, 0, false);
	}

	public void OnSaveObjectPosition(string posX, string posY, string posZ, string rotX, string rotY, string rotZ)
	{
		float objPosX = float.TryParse(posX, NumberStyles.Any, CultureInfo.InvariantCulture, out objPosX) ? objPosX : 0.0f;
		float objPosY = float.TryParse(posY, NumberStyles.Any, CultureInfo.InvariantCulture, out objPosY) ? objPosY : 0.0f;
		float objPosZ = float.TryParse(posZ, NumberStyles.Any, CultureInfo.InvariantCulture, out objPosZ) ? objPosZ : 0.0f;
		float objRotX = float.TryParse(rotX, NumberStyles.Any, CultureInfo.InvariantCulture, out objRotX) ? objRotX : 0.0f;
		float objRotY = float.TryParse(rotY, NumberStyles.Any, CultureInfo.InvariantCulture, out objRotY) ? objRotY : 0.0f;
		float objRotZ = float.TryParse(rotZ, NumberStyles.Any, CultureInfo.InvariantCulture, out objRotZ) ? objRotZ : 0.0f;

		NetworkEventSender.SendNetworkEvent_Generics_UpdateGenericPosition(objPosX, objPosY, objPosZ, objRotX, objRotY, objRotZ, m_GenericItemToUpdate);
	}

	public void OnUpdateGenericPreview(string posX, string posY, string posZ, string rotX, string rotY, string rotZ)
	{
		float objPosX = float.TryParse(posX, NumberStyles.Any, CultureInfo.InvariantCulture, out objPosX) ? objPosX : 0.0f;
		float objPosY = float.TryParse(posY, NumberStyles.Any, CultureInfo.InvariantCulture, out objPosY) ? objPosY : 0.0f;
		float objPosZ = float.TryParse(posZ, NumberStyles.Any, CultureInfo.InvariantCulture, out objPosZ) ? objPosZ : 0.0f;
		float objRotX = float.TryParse(rotX, NumberStyles.Any, CultureInfo.InvariantCulture, out objRotX) ? objRotX : 0.0f;
		float objRotY = float.TryParse(rotY, NumberStyles.Any, CultureInfo.InvariantCulture, out objRotY) ? objRotY : 0.0f;
		float objRotZ = float.TryParse(rotZ, NumberStyles.Any, CultureInfo.InvariantCulture, out objRotZ) ? objRotZ : 0.0f;
		m_GenericItemToUpdate.Position = new RAGE.Vector3(objPosX, objPosY, objPosZ);
		m_GenericItemToUpdate.SetRotation(objRotX, objRotY, objRotZ, 0, false);
	}
}