public class ChatBubbles
{
	public ChatBubbles()
	{
		RageEvents.RAGE_OnRender += OnRender;

		ClientTimerPool.CreateTimer(UpdateChatBubbleEffectState, 120);

		if (!RAGE.Game.Graphics.HasStreamedTextureDictLoaded(g_strChatBubbleDictName))
		{
			RAGE.Game.Graphics.RequestStreamedTextureDict(g_strChatBubbleDictName, true);
		}
	}

	private void OnRender()
	{
		RAGE.Vector3 vecLocalPlayerPos = RAGE.Elements.Player.LocalPlayer.Position;

		foreach (var player in RAGE.Elements.Entities.Players.Streamed)
		{
			if (player != RAGE.Elements.Player.LocalPlayer)
			{
				// Are they nearby?
				float fDistScale = 0.0f;
				Vector2 vecScreenPos = null;
				if (Nametags.GetNametagPositionForPlayer(vecLocalPlayerPos, player, out vecScreenPos, out fDistScale))
				{
					bool bPlayerIsChatting = player.IsTypingInTextChat;
					bool bReconOn = DataHelper.GetEntityData<bool>(player, EDataNames.RECON);
					bool bDisappearOn = DataHelper.GetEntityData<bool>(player, EDataNames.DISAPPEAR);
					bool bNametagsHidden = DataHelper.GetLocalPlayerEntityData<bool>(EDataNames.NAMETAGS);

					if (bPlayerIsChatting && !bReconOn && !bDisappearOn && !bNametagsHidden)
					{

						if (RAGE.Game.Graphics.HasStreamedTextureDictLoaded(g_strChatBubbleDictName))
						{

							// calc scale
							Vector2 vecResolution = GraphicsHelper.GetScreenResolution();
							float scaleX = (32 / vecResolution.X) * fDistScale;
							float scaleY = (32 / vecResolution.Y) * fDistScale;

							// subtract height of sprite + buffer from nametag pos
							vecScreenPos.Y += scaleY * 2.0f;

							RAGE.RGBA col1 = new RAGE.RGBA(255, 255, 255, 100);
							RAGE.RGBA col2 = new RAGE.RGBA(255, 255, 255, 100);
							RAGE.RGBA col3 = new RAGE.RGBA(255, 255, 255, 100);

							if (m_ChatBubbleEffectState == 0)
							{
								col1.Alpha = 255;
							}
							else if (m_ChatBubbleEffectState == 1)
							{
								col2.Alpha = 255;
							}
							else if (m_ChatBubbleEffectState == 2)
							{
								col3.Alpha = 255;
							}
							else if (m_ChatBubbleEffectState == 3)
							{
								// nothing to do for 3, its neutral
							}

							RAGE.Game.Graphics.DrawSprite(g_strChatBubbleDictName, "medaldot_32", vecScreenPos.X - (scaleX), vecScreenPos.Y, scaleX, scaleY, 0, (int)col1.Red, (int)col1.Green, (int)col1.Blue, (int)col1.Alpha, 0);
							RAGE.Game.Graphics.DrawSprite(g_strChatBubbleDictName, "medaldot_32", vecScreenPos.X, vecScreenPos.Y, scaleX, scaleY, 0, (int)col2.Red, (int)col2.Green, (int)col2.Blue, (int)col2.Alpha, 0);
							RAGE.Game.Graphics.DrawSprite(g_strChatBubbleDictName, "medaldot_32", vecScreenPos.X + (scaleX), vecScreenPos.Y, scaleX, scaleY, 0, (int)col3.Red, (int)col3.Green, (int)col3.Blue, (int)col3.Alpha, 0);
						}
					}
				}
			}

		}
	}

	// TODO: Do this on render instead... will need to calculate frame times. Making timers all the time can't be good
	private void UpdateChatBubbleEffectState(object[] parameters)
	{
		if (m_ChatBubbleEffectState > 3)
		{
			m_ChatBubbleEffectState = 0;
		}
		else
		{
			m_ChatBubbleEffectState++;
		}
	}

	private int m_ChatBubbleEffectState = 0;
	private const string g_strChatBubbleDictName = "srange_gen";
}