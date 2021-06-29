using System.Collections.Generic;

public static class ShardManager
{
	static ShardManager()
	{

	}

	public static void Init()
	{
		NetworkEvents.CharacterSelectionApproved += OnChangeCharacter;
	}

	public static void ShowShard(string strTitle, string strText1, string strText2 = "")
	{
		CShardInstance newShard = new CShardInstance(strTitle, strText1, strText2);
		m_queueShards.Enqueue(newShard);

		PopNextShard();
	}

	private static void OnChangeCharacter()
	{
		m_queueShards.Clear();
		m_ShardGUI.SetVisible(false, false, false);
	}

	public static void OnFadedOut()
	{
		m_bShowingShard = false;
		PopNextShard();
	}

	private static void PopNextShard()
	{
		if (!m_bShowingShard)
		{
			if (m_queueShards.Count > 0)
			{
				m_ShardGUI.SetVisible(true, false, false);
				m_bShowingShard = true;

				CShardInstance shardInst = m_queueShards.Dequeue();
				InternalShowShard(shardInst);
			}
			else
			{
				m_ShardGUI.SetVisible(false, false, false);
			}
		}
	}

	private static void InternalShowShard(CShardInstance shardInstance)
	{
		// TODO: Other sound
		RAGE.Game.Audio.PlaySoundFrontend(-1, "SELECT", "HUD_FREEMODE_SOUNDSET", true);
		m_ShardGUI.ShowShard(shardInstance.Title, shardInstance.Text1, shardInstance.Text2);
	}

	private class CShardInstance
	{
		public CShardInstance(string strTitle, string strText1, string strText2)
		{
			Title = strTitle;
			Text1 = strText1;
			Text2 = strText2;
		}

		public string Title { get; }
		public string Text1 { get; }
		public string Text2 { get; }
	}

	private static void OnUILoaded()
	{

	}

	public static CGUIShard m_ShardGUI = new CGUIShard(OnUILoaded);
	private static Queue<CShardInstance> m_queueShards = new Queue<CShardInstance>();
	private static bool m_bShowingShard = false;
}

public class CGUIShard : CEFCore
{
	public CGUIShard(OnGUILoadedDelegate callbackOnLoad) : base("owl_core.client/shard.html", EGUIID.ShardManager, callbackOnLoad)
	{
		UIEvents.Shard_OnFadedOut += ShardManager.OnFadedOut;
	}

	public override void OnLoad()
	{

	}

	public void ShowShard(string strTitle, string strText1, string strText2)
	{
		Execute("ShowShard", strTitle, strText1, strText2);
	}
}