public class BinocularSystem
{
	public BinocularSystem()
	{

	}

	public void ToggleBinoculars(CPlayer SenderPlayer, EBinocularsType binocularsType)
	{
		if (!SenderPlayer.ToggledBinoculars)
		{
			SenderPlayer.ToggleBinoculars(true);
			SenderPlayer.SetBinocularsType(binocularsType);
			SenderPlayer.AddAnimationToQueue((int)(AnimationFlags.StopOnLastFrame | AnimationFlags.OnlyAnimateUpperBody | AnimationFlags.AllowPlayerControl), "amb@world_human_binoculars@male@enter", "enter", false, true, false, 2500, false);
			SenderPlayer.AddAnimationToQueue((int)(AnimationFlags.StopOnLastFrame | AnimationFlags.OnlyAnimateUpperBody | AnimationFlags.AllowPlayerControl), "amb@world_human_binoculars@male@idle_a", "idle_a", false, true, false, 1000 * 1000, false);
		}
		else
		{
			SenderPlayer.ToggleBinoculars(false);
			SenderPlayer.SetBinocularsType(EBinocularsType.None);
			SenderPlayer.AddAnimationToQueue((int)(AnimationFlags.StopOnLastFrame | AnimationFlags.OnlyAnimateUpperBody | AnimationFlags.AllowPlayerControl), "amb@world_human_binoculars@male@exit", "exit", false, true, false, 2500, false);
		}
	}
}

