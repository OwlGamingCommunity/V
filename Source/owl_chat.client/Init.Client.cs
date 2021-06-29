class Init_ChatSystem : RAGE.Events.Script { Init_ChatSystem() { OwlScriptManager.RegisterScript<ChatSystem>(); } }

class ChatSystem : OwlScript
{
	public ChatSystem()
	{
		m_ChatBubbles = new ChatBubbles();
		m_Chatbox = new Chatbox();
		m_Cellphone = new Cellphone();
		m_Languages = new Languages();
		m_ChatVisualization = new ChatVisualization();
		m_CoinFlip = new CoinFlip();
	}

	public static ChatBubbles GetChatBubbles() { return m_ChatBubbles; }
	private static ChatBubbles m_ChatBubbles = null;

	public static Chatbox GetChatbox() { return m_Chatbox; }
	private static Chatbox m_Chatbox = null;

	public static Cellphone GetCellphone() { return m_Cellphone; }
	private static Cellphone m_Cellphone = null;

	public static Languages GetLanguages() { return m_Languages; }
	private static Languages m_Languages = null;

	public static CoinFlip GetCoinFlip() { return m_CoinFlip; }
	private static CoinFlip m_CoinFlip = null;

	private static ChatVisualization m_ChatVisualization = null;
}