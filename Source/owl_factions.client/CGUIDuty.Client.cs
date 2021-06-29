internal class CGUIDuty : CEFCore
{
	public CGUIDuty(OnGUILoadedDelegate callbackOnLoad) : base("owl_factions.client/duty.html", EGUIID.Duty, callbackOnLoad)
	{
		UIEvents.GoOnDuty += () => { FactionSystem.GetDutySystem()?.GoOnDuty(); };
		UIEvents.CancelGoingOnDuty += () => { FactionSystem.GetDutySystem()?.CancelGoingOnDuty(); };
		UIEvents.Duty_GotoOutfits += () => { FactionSystem.GetDutySystem()?.GotoOutfits(); };
	}

	public override void OnLoad()
	{

	}

	public void ClearDropdown()
	{
		Execute("ClearDropdown");
	}

	public void AddDropdownItem(string name, string value)
	{
		Execute("AddDropdownItem", name, value);
	}

	public void SetDropdownText(string text)
	{
		Execute("SetDropdownText", text);
	}
}

