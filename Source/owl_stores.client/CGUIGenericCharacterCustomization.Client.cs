public class CGUIGenericCharacterCustomization : CEFCore
{
	public CGUIGenericCharacterCustomization(OnGUILoadedDelegate callbackOnLoad, EGUIID guiID) : base("owl_stores.client/genericcharactercustomization.html", guiID, callbackOnLoad)
	{

	}

	public override void OnLoad()
	{

	}

	public void Initialize(string strStoreName, UIEventID FinishEvent, UIEventID ExitEvent, UIEventID ToggleClothesEvent, UIEventID GotoBodyCam_NearEvent, UIEventID GotoBodyCam_FarEvent, UIEventID GotoHeadCamEvent, UIEventID StartRotationEvent, UIEventID StopRotationEvent, UIEventID ResetRotationEvent)
	{
		Execute("Initialize", strStoreName, FinishEvent.ToString(), ExitEvent.ToString(), ToggleClothesEvent.ToString(), GotoBodyCam_NearEvent.ToString(), GotoBodyCam_FarEvent.ToString(), GotoHeadCamEvent.ToString(), StartRotationEvent.ToString(), StopRotationEvent.ToString(), ResetRotationEvent.ToString());
	}

	public void AddHeritage(EGender gender, int value)
	{
		Execute("AddHeritage", gender, value);
	}

	public void AddTabContent_HeritageSelector(string strTitle, int callbackID, EGender gender, string strDescription, UIEventID onChangeEvent)
	{
		Execute("AddTabContent_HeritageSelector", strTitle, callbackID, gender, strDescription, onChangeEvent.ToString());
	}

	public void AddTabContent_Slider(string strTitle, string strDescription, string strLeftText, string strRightText, uint minVal, uint maxVal, uint defaultVal, UIEventID onChangeEventName, UIEventID onFinalizedChangeEventName)
	{
		// NOTE: onChangeEventName is called constantly, use it for updating locally, but not transmission/save etc
		Execute("AddTabContent_Slider", strTitle, strDescription, strLeftText, strRightText, minVal, maxVal, defaultVal, onChangeEventName.ToString(), onFinalizedChangeEventName.ToString());
	}

	public void AddTabContent_NumberSelector(string strTitle, string strDescription, uint minVal, uint maxVal, uint initialVal, UIEventID onChangeEvent)
	{
		Execute("AddTabContent_NumberSelector", strTitle, strDescription, minVal, maxVal, initialVal, onChangeEvent.ToString());
	}

	public void AddTabContent_GenericButton(string strCaption, UIEventID onPressEvent)
	{
		Execute("AddTabContent_GenericButton", strCaption, onPressEvent.ToString());
	}

	public void AddTabContent_ClothingSelector(string strTitle, string box1_name, int box1_min, int box1_max, int box1_default, UIEventID box1_event, string box2_name, int box2_min, int box2_max, int box2_default, UIEventID box2_event, UIEventID eventOnRootChanged)
	{
		if (box1_min < 0) { box1_min = 0; }
		if (box1_max < 0) { box1_max = 15; }
		if (box2_min < 0) { box2_min = 0; }
		if (box2_max < 0) { box2_max = 15; }

		Execute("AddTabContent_ClothingSelector", strTitle, box1_name, box1_min, box1_max, box1_default, box1_event.ToString(), box2_name, box2_min, box2_max, box2_default, box2_event.ToString(), eventOnRootChanged.ToString()); ;
	}


	public void AddPendingColor(uint colorID, string strHexCode)
	{
		Execute("AddPendingColor", colorID, strHexCode);
	}

	public void AddTabContent_ColorPicker(string strTitle, string strDescription, uint initialVal, UIEventID onChangeEvent)
	{
		Execute("AddTabContent_ColorPicker", strTitle, strDescription, initialVal, onChangeEvent.ToString());
	}

	public void Reset()
	{
		Execute("Reset");
	}

	public void SetConfirmButtonText(string strText)
	{
		Execute("SetConfirmButtonText", strText);
	}

	public void ResetConfirmButtonText()
	{
		Execute("ResetConfirmButtonText");
	}

	public void SetExitButtonText(string strText)
	{
		Execute("SetExitButtonText", strText);
	}

	public void ResetExitButtonText()
	{
		Execute("ResetExitButtonText");
	}

	public void AddTab(string strName, string strIcon, UIEventID onChangeEvent)
	{
		Execute("AddTab", strName, strIcon, onChangeEvent.ToString());
	}

	public void AddSeperator()
	{
		Execute("AddSeperator");
	}

	public void AddTabContent_Textbox(string strTitle, string strDescription, uint maxLength, bool bAddValidator, UIEventID onChangeEvent, string strDefault)
	{
		Execute("AddTabContent_Textbox", strTitle, strDescription, maxLength, bAddValidator, onChangeEvent.ToString(), strDefault);
	}

	public void AddTabContent_TwoRadioOptions(string strTitle, string strDescription, string strRadioButton1, string strRadioButton2, UIEventID onChangeEvent, int initialSelection)
	{
		Execute("AddTabContent_TwoRadioOptions", strTitle, strDescription, strRadioButton1, strRadioButton2, onChangeEvent.ToString(), initialSelection);
	}

	public void AddPendingDropdownItem(string strValue, string strName)
	{
		Execute("AddPendingDropdownItem", strValue, strName);
	}

	public void AddTabContent_Dropdown(string strTitle, string strDescription, string strInitialText, UIEventID onChangeEvent)
	{
		Execute("AddTabContent_Dropdown", strTitle, strDescription, strInitialText, onChangeEvent.ToString());
	}

	public void AddTattooListItem(int ID, string strDisplayName)
	{
		Execute("AddTattooListItem", ID, strDisplayName);
	}

	public void CommitTattooList()
	{
		Execute("CommitTattooList");
	}

	public void ShowTattooCreator()
	{
		Execute("ShowTattooCreator");
	}

	public void AddTabContent_GenericListItem(string strTitle, string strSubtitle, string strTinyText, UIEventID onClickEvent, UIEventID onMouseEnterEvent, UIEventID onMouseExitEvent, int callbackID)
	{
		Execute("AddTabContent_GenericListItem", strTitle, strSubtitle, strTinyText, onClickEvent.ToString(), onMouseEnterEvent.ToString(), onMouseExitEvent.ToString(), callbackID);
	}

	public void DeleteTabContent_GenericListItem(string strElementName)
	{
		Execute("DeleteTabContent_GenericListItem", strElementName);
	}

	public void SetName(string strName)
	{
		Execute("SetName", strName);
	}

	public void SetPriceString(string strPriceString)
	{
		Execute("SetPriceString", strPriceString);
	}

	public void SetMaxForElement(string strElementName, int newMax)
	{
		Execute("SetMaxForElement", strElementName, newMax);
	}
}