using System;

public class TagRemoverJobInstance : BaseJob, IDisposable
{
	public TagRemoverJobInstance(CPlayer a_Owner) : base(a_Owner, EJobID.TagRemoverJob, EAchievementID.TagRemoverJob, EAchievementID.None, "Graffiti Cleaner", EDrivingTestType.Car, EVehicleType.TagRemoverJob)
	{
		AddLevel(0, 25.0f);
	}

	public void Dispose()
	{
		Dispose(true);

		GC.SuppressFinalize(this);
	}

	protected virtual void Dispose(bool a_CleanupNativeAndManaged)
	{

	}

	public override void OnQuitJob()
	{

	}

	public override void OnStartJob(bool b_IsResume)
	{

	}

	public override int GetXP()
	{
		return 0;
	}
}