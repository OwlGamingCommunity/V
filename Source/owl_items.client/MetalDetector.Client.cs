public class MetalDetector
{
	public MetalDetector()
	{
		NetworkEvents.PlayMetalDetectorAlarm += OnPlayMetalDetectorAlarm;
	}

	private void OnPlayMetalDetectorAlarm(RAGE.Vector3 colshapePosition)
	{
		RAGE.Game.Audio.PlaySoundFromCoord(-1, "Metal_Detector_Big_Guns", colshapePosition.X, colshapePosition.Y, colshapePosition.Z, "dlc_ch_heist_finale_security_alarms_sounds", false, 0, false);
	}
}
