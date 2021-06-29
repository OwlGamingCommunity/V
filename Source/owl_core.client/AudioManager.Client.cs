using System;
using System.Collections.Generic;

public delegate void AudioFinishedPlayingDelegate(WeakReference<AudioInstance> audioInst);

public static class AudioDefinitions
{
	public class AudioDefinition
	{
		public string ResourceName { get; }
		public string AudioPrefixName { get; }
		public uint NumSegments { get; }

		public AudioDefinition(string strResourceName, string audioPrefixName, uint numSegments)
		{
			ResourceName = strResourceName;
			AudioPrefixName = audioPrefixName;
			NumSegments = numSegments;
		}
	}

	public static Dictionary<EAudioIDs, AudioDefinition> g_DictAudioDefinitions = new Dictionary<EAudioIDs, AudioDefinition>()
	{
		{ EAudioIDs.MenuMusic, new AudioDefinition("owl_account_system.client", "login", 24)  },
		{ EAudioIDs.Uganda, new AudioDefinition("owl_achievements.client", "uganda", 3)  },
		{ EAudioIDs.Mexico, new AudioDefinition("owl_achievements.client", "mex", 11)  },
		{ EAudioIDs.Country, new AudioDefinition("owl_account_system.client", "country", 108)  },
		{ EAudioIDs.Christmas, new AudioDefinition("owl_account_system.client", "december", 36)  },
		{ EAudioIDs.Seatbelt_Buckle, new AudioDefinition("owl_vehicles.client", "unbuckle", 1)  },
		{ EAudioIDs.Seatbelt_Unbuckle, new AudioDefinition("owl_vehicles.client", "buckle", 3)  },
		{ EAudioIDs.FourthOfJulyCountdown, new AudioDefinition("owl_admin.client", "countdown", 141) },
		{ EAudioIDs.Handbrake_Up, new AudioDefinition("owl_vehicles.client", "hb_on", 1) },
		{ EAudioIDs.Handbrake_Down, new AudioDefinition("owl_vehicles.client", "hb_off", 1) },
		{ EAudioIDs.Halloween, new AudioDefinition("owl_account_system.client", "halloween", 39)  },
	};
}

public static class AudioManager
{
	private const uint g_AudioPoolSize = 25;
	private static CGUIAudioPlayer m_AudioPlayer;

	private static AudioInstance[] arrAudioInstances = new AudioInstance[g_AudioPoolSize];

	static AudioManager()
	{

	}

	public static void Init()
	{
		for (uint i = 0; i < g_AudioPoolSize; ++i)
		{
			arrAudioInstances[i] = null;
		}

		m_AudioPlayer = new CGUIAudioPlayer(() => { });
		m_AudioPlayer.SetVisible(true, false, false);

		m_AudioPlayer.Initialize(g_AudioPoolSize);

		NetworkEvents.PlayCustomAudio += (EAudioIDs audioID, bool bStopAllOtherAudio) =>
		{
			PlayAudio(audioID, bStopAllOtherAudio);
		};
	}

	public static bool StopAudio(WeakReference<AudioInstance> audioInst)
	{
		AudioInstance inst = audioInst.Instance();
		if (inst != null)
		{
			m_AudioPlayer.StopAudio(inst.PoolID);
			// TODO_CSHARP: Indicate if it was stopped manually or not
			OnPlayingFinished(inst.PoolID);
			return true;
		}

		return false;
	}

	public static void StopAllAudio()
	{
		for (uint i = 0; i < g_AudioPoolSize; ++i)
		{
			if (arrAudioInstances[i] == null)
			{
				StopAudio(new WeakReference<AudioInstance>(arrAudioInstances[i]));
			}
		}
	}

	public static bool FadeOutAudio(WeakReference<AudioInstance> audioInst)
	{
		AudioInstance inst = audioInst.Instance();
		if (inst != null)
		{
			m_AudioPlayer.FadeOutAudio(inst.PoolID);
			// TODO_CSHARP: Indicate if it was stopped manually or not
			return true;
		}

		return false;
	}

	public static WeakReference<AudioInstance> PlayAudio(EAudioIDs audioID, bool bStopAllOtherCurrentAudio, bool bLoopAudio = false, AudioFinishedPlayingDelegate callbackOnFinishPlaying = null)
	{
		if (bStopAllOtherCurrentAudio)
		{
			StopAllAudio();
		}

		AudioDefinitions.AudioDefinition audioDefinition = AudioDefinitions.g_DictAudioDefinitions[audioID];

		if (audioDefinition != null)
		{
			// Find a free pool space
			for (uint i = 0; i < g_AudioPoolSize; ++i)
			{
				if (arrAudioInstances[i] == null)
				{
					AudioInstance newAudio = new AudioInstance(i, audioDefinition.ResourceName, audioDefinition.AudioPrefixName, audioDefinition.NumSegments, callbackOnFinishPlaying);

					m_AudioPlayer.InitAudio(i, bLoopAudio);

					for (int segmentIndex = 1; segmentIndex <= audioDefinition.NumSegments; ++segmentIndex)
					{
						string strSegmentExtension = (audioDefinition.NumSegments == 1) ? "" : Helpers.FormatString("-{0}", segmentIndex);
						string strFilePath = Helpers.FormatString("package://{0}/{1}{2}.ogg", audioDefinition.ResourceName, audioDefinition.AudioPrefixName, strSegmentExtension);
						m_AudioPlayer.AddAudioTrack(i, strFilePath);
					}

					m_AudioPlayer.PlayAudio(i);

					arrAudioInstances[i] = newAudio;
					return new WeakReference<AudioInstance>(newAudio);
				}
			}
		}

		return null;
	}

	public static void OnPlayingFinished(uint poolIndex)
	{
		if (poolIndex < arrAudioInstances.Length)
		{
			AudioInstance audioInst = arrAudioInstances[poolIndex];

			if (audioInst != null)
			{
				arrAudioInstances[poolIndex].OnFinishedPlaying();
				arrAudioInstances[poolIndex] = null;
			}
		}
	}
}

public class AudioInstance
{
	public uint PoolID { get; }
	private string ResourceName { get; }
	private string FileName { get; }

	private AudioFinishedPlayingDelegate m_callbackOnFinishPlaying;

	public AudioInstance(uint poolID, string strResourceName, string strFileName, uint numSegments, AudioFinishedPlayingDelegate callbackOnFinishPlaying)
	{
		PoolID = poolID;
		ResourceName = strResourceName;
		FileName = strFileName;
		m_callbackOnFinishPlaying = callbackOnFinishPlaying;


	}

	public void OnFinishedPlaying()
	{
		if (m_callbackOnFinishPlaying != null)
		{
			m_callbackOnFinishPlaying(new WeakReference<AudioInstance>(this));
		}
	}
}

internal class CGUIAudioPlayer : CEFCore
{
	public CGUIAudioPlayer(OnGUILoadedDelegate callbackOnLoad) : base("owl_core.client/audioplayer.html", EGUIID.AudioManager, callbackOnLoad)
	{
		UIEvents.AudioPlayFinished += (uint poolIndex) => { AudioManager.OnPlayingFinished(poolIndex); };
	}

	public override void OnLoad()
	{

	}

	public void Initialize(uint poolSize)
	{
		Execute("InitAudioPool", poolSize);
	}

	public void InitAudio(uint poolIndex, bool bLoop)
	{
		Execute("InitAudio", poolIndex, bLoop);
	}
	public void AddAudioTrack(uint poolIndex, string strPath)
	{
		Execute("AddAudioTrack", poolIndex, strPath);
	}

	public void PlayAudio(uint poolIndex)
	{
		Execute("PlayAudio", poolIndex);
	}

	public void StopAudio(uint poolIndex)
	{
		Execute("StopAudio", poolIndex);
	}

	public void FadeOutAudio(uint poolIndex)
	{
		Execute("FadeOutAudio", poolIndex);
	}
}