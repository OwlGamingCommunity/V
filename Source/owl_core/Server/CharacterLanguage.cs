public class CCharacterLanguage
{
	public CCharacterLanguage(ECharacterLanguage a_eCharacterLanguage, bool a_bActive, float a_fProgress)
	{
		CharacterLanguage = a_eCharacterLanguage;
		Active = a_bActive;
		Progress = a_fProgress;
	}

	public ECharacterLanguage CharacterLanguage { get; set; }
	public bool Active { get; set; }
	public float Progress { get; set; }
}