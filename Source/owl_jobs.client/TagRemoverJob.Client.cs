using System;
using System.Collections.Generic;
using EntityDatabaseID = System.Int64;

class TagRemoverJobInstance : BaseJob
{
	Dictionary<Int64, RAGE.Elements.Blip> m_dictBlips = new Dictionary<Int64, RAGE.Elements.Blip>();
	private Int64 m_iTagInProgressID = -1;
	private bool m_bIsCleaning = false;
	private bool m_bHasCleaningBeenApproved = false;
	private DateTime m_timeStartCleaning = DateTime.Now;
	private int timeBetweenTagUpdates = 100;

	public TagRemoverJobInstance() : base(EJobID.TagRemoverJob, "Graffiti Cleaner", "Find graffiti on the map and remove it", EVehicleType.None, EWorldPedType.TagRemoverJob, 0x48114518, new RAGE.Vector3(6.581469f, 6414.272f, 31.42529f), 232.0764f, 0,
		new RAGE.Vector3(120.7183f, -3099.603f, 6.010986f), 269.8574f, 0, 72)
	{
		RageEvents.RAGE_OnRender += OnRender;
		RageEvents.RAGE_OnTick_OncePerSecond += UpdateBlips;

		NetworkEvents.DestroyGangTag += OnDestroyGangTag;

		NetworkEvents.ChangeCharacterApproved += CleanupBlips;

		NetworkEvents.StartTagCleaningResponse += OnStartCleaningResponse;
		NetworkEvents.TagCleaningComplete += OnTagCleaningComplete;
	}

	private void OnDestroyGangTag(EntityDatabaseID a_ID)
	{
		GangTagStorage tag = GangTagPool.GetGangTag(a_ID);

		if (tag != null)
		{
			if (m_dictBlips.ContainsKey(tag.ID))
			{
				var blip = m_dictBlips[tag.ID];
				if (blip != null)
				{
					blip.Destroy();
				}

				m_dictBlips.Remove(tag.ID);
			}
		}
	}



	private void CleanTag(Int64 tagID)
	{
		m_iTagInProgressID = tagID;
		m_bIsCleaning = true;
		m_bHasCleaningBeenApproved = false;
		m_timeStartCleaning = DateTime.Now;
		NetworkEventSender.SendNetworkEvent_RequestTagCleaning(m_iTagInProgressID);
	}

	private void OnStartCleaningResponse(bool bApproved)
	{
		if (bApproved)
		{
			m_bHasCleaningBeenApproved = true;
		}
		else
		{
			m_iTagInProgressID = -1;
			m_bIsCleaning = false;
			m_bHasCleaningBeenApproved = false;
		}
	}

	private void OnTagCleaningComplete()
	{
		m_iTagInProgressID = -1;
		m_bIsCleaning = false;
		m_bHasCleaningBeenApproved = false;
	}

	const float g_fDistThreshold = 10.0f;
	private GangTagStorage GetNearestGangTag()
	{
		GangTagStorage nearestTag = null;

		float fSmallestDistance = 999999.0f;
		foreach (var kvPair in GangTagPool.GetAll())
		{
			GangTagStorage tag = kvPair.Value;
			if (RAGE.Elements.Player.LocalPlayer.Dimension == tag.Dimension)
			{
				RAGE.Vector3 vecTagPos = tag.Position;
				RAGE.Vector3 vecPlayerPos = RAGE.Elements.Player.LocalPlayer.Position;
				float fDistance = WorldHelper.GetDistance(vecPlayerPos, vecTagPos);

				if (fDistance <= g_fDistThreshold && fDistance < fSmallestDistance)
				{
					fSmallestDistance = fDistance;
					nearestTag = tag;
				}
			}
		}

		return nearestTag;
	}

	private GangTagStorage GetObjectForTagCleaning()
	{
		if (!m_bIsCleaning)
		{
			return null;
		}

		foreach (var kvPair in GangTagPool.GetAll())
		{
			if (kvPair.Value.ID == m_iTagInProgressID)
			{
				return kvPair.Value;
			}
		}

		return null;
	}

	private void OnRender()
	{
		if (m_bIsCleaning)
		{
			if (m_bHasCleaningBeenApproved)
			{
				double timeSinceLastProc = (DateTime.Now - m_timeStartCleaning).TotalMilliseconds;

				GangTagStorage tagBeingCleaned = GetObjectForTagCleaning();
				if (tagBeingCleaned != null)
				{
					float fTagProgress = 100.0f - tagBeingCleaned.Progress;
					TextHelper.Draw2D(Helpers.FormatString("Cleaning in progress: {0:0}%", fTagProgress), 0.5f, 0.8f, 0.4f, 255, 255, 255, 255, RAGE.Game.Font.ChaletLondon, RAGE.NUI.UIResText.Alignment.Centered, true, true);

					if (timeSinceLastProc >= timeBetweenTagUpdates)
					{
						m_timeStartCleaning = DateTime.Now;
						NetworkEventSender.SendNetworkEvent_UpdateTagCleaning(m_iTagInProgressID);
					}

					if (fTagProgress >= 100.0f)
					{
						m_bIsCleaning = false;
						m_iTagInProgressID = -1;
					}
				}
			}
		}
		else
		{
			if (IsJobActive())
			{
				GangTagStorage nearestTag = GetNearestGangTag();
				if (nearestTag != null)
				{
					Int64 charID = DataHelper.GetEntityData<Int64>(RAGE.Elements.Player.LocalPlayer, EDataNames.CHARACTER_ID);

					if (nearestTag.OwnerCharacterID != charID)
					{
						WorldHintManager.DrawExclusiveWorldHint(ScriptControls.GetKeyBoundToControl(EScriptControlID.Interact), "Clean Tag", null, () => { CleanTag(nearestTag.ID); }, nearestTag.Position, nearestTag.Dimension, false, false, 3.0f);
					}
					else if (nearestTag.Progress >= 100.0f) // If less than 100, we don't show this message because we'll probably be offered to resume tagging
					{
						WorldHintManager.DrawExclusiveWorldHint(ConsoleKey.NoName, "You cannot clean your own tags", null, null, nearestTag.Position, nearestTag.Dimension, false, false, 3.0f);
					}
				}
			}
		}
	}

	private void UpdateBlips()
	{
		if (IsJobActive())
		{
			foreach (var kvPair in GangTagPool.GetAll())
			{
				try
				{
					if (!m_dictBlips.ContainsKey(kvPair.Value.ID))
					{
						// calculate a random position offset, but within the radius so the blip isn't directly on top of the tag
						const float fBlipRadius = 5;
						Random rng = new Random();
						RAGE.Vector3 vecBlipPos = kvPair.Value.Position.CopyVector();
						float fRot = rng.Next(0, 360);
						float fDist = fBlipRadius;
						float radians = (fRot + 90.0f) * (3.14f / 180.0f);
						vecBlipPos.X += (float)Math.Cos(radians) * fDist;
						vecBlipPos.Y += (float)Math.Sin(radians) * fDist;

						var blip = new RAGE.Elements.Blip(1, vecBlipPos, "Graffiti Cleanup Area", (int)fBlipRadius, 2, 150, 0, true, 0, kvPair.Value.Dimension);
						m_dictBlips.Add(kvPair.Value.ID, blip);
					}
				}
				catch
				{

				}
			}
		}
	}

	private void CleanupBlips()
	{
		foreach (var kvPair in m_dictBlips)
		{
			kvPair.Value.Destroy();
		}

		m_dictBlips.Clear();
	}

	public override void CleanupAll()
	{
		CleanupBlips();
	}

	public override void Reset()
	{
		CleanupBlips();
	}

	public override void OnEnteredJobVehicle()
	{

	}

	public override void OnExitedJobVehicle()
	{

	}

}