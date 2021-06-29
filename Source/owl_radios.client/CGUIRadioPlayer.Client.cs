internal class CGUIRadioPlayer : CEFCore
{
	public CGUIRadioPlayer(OnGUILoadedDelegate callbackOnLoad) : base("owl_radios.client/radioplayer.html", EGUIID.RadioPlayer, callbackOnLoad)
	{

	}

	public override void OnLoad()
	{

	}

	public void PlayRadioStation(string strURL)
	{
		Execute("PlayRadioStation", strURL);
	}

	public void StopRadioStation()
	{
		Execute("StopRadioStation");
	}

	public void SetVolume(float fVol)
	{
		Execute("SetVolume", fVol);
	}
}