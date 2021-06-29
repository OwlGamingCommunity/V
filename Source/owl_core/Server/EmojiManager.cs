using System.Collections.Generic;
using System.Text;

public static class EmojiManager
{
	public static string TryAndParseEmoji(string text)
	{
		StringBuilder sb = new StringBuilder();
		string[] textArray = text.Split(' ');

		foreach (var word in textArray)
		{
			sb.Append(g_dictEmojis.TryGetValue(word.ToLower(), out string emojiText) ? emojiText : word);
			sb.Append(" ");
		}
		return sb.ToString();
	}

	private static Dictionary<string, string> g_dictEmojis = new Dictionary<string, string>()
	{
		{":eyes:", "&#128064;" },
		{":fire:", "&#128293;" },
		{":100:", "&#128175;" },
		{":ok:", "&#127383;" },
		{":jer:", "&#129492;" },
		{":joy:", "&#128514;" },
		{":wink:", "&#128521;" },
		{":hearteyes:", "&#128525;" },
		{":pensive:", "&#128532;" },
		{":rollingeyes:", "&#128580;" },
		{":unamused:", "&#128530;" },
		{":hugging:", "&#129303;" },
		{":mask:", "&#128567;" },
		{":sleeping:", "&#128564;" },
		{":drool:", "&#129316;" },
		{":moneymouth:", "&#129297;" },
		{":sleepy:", "&#129322;" },
		{":flushed:", "&#128563;" },
		{":sob:", "&#128557;" },
		{":scream:", "&#128561;" },
		{":weary:", "&#128553;" },
		{":tired:", "&#128555;" },
		{":triumph:", "&#128548;" },
		{":rage:", "&#128545;" },
		{":blueheart:", "&#128153;" },
		{":smile:", "&#128578;" },
		{":updownsmile:", "&#128579;" },
		{":grin:", "&#128516;" },
		{":rofl:", "&#129315;" },
		{":relieved:", "&#128524;" },
		{":lying:", "&#129317;" },
		{":grimacing:", "&#128556;" },
		{":vomiting:", "&#129326;" },
		{":woozy:", "&#129396;" },
		{":sneezing:", "&#129319;" },
		{":explodinghead:", "&#129327;" },
		{":sunglasses:", "&#128526;" },
		{":partying:", "&#129395;" },
		{":hushed:", "&#128559;" },
		{":astonished:", "&#128562;" },
		{":anxious:", "&#128560;" },
		{":cryingloud:", "&#128557;" },
		{":downcastface:", "&#128531;" },
		{":yawning:", "&#129393;" },
		{":cursing:", "&#129324;" },
		{":angry:", "&#128544;" },
		{":poop:", "&#128169;" },
		{":ghost:", "&#128123;" },
		{":joyfulcat:", "&#128569;" },
		{":monkeynosee:", "&#128584;" },
		{":kiss:", "&#128139;" },
		{":heart:", "&#10084;&#65039;" },
		{":brokenheart:", "&#128148;" },
		{":twohearts:", "&#128149;" },
		{":hundred:", "&#128175;" },
		{":bomb:", "&#128163;" },
		{":zzz:", "&#128164;" },
		{":wavinghand:", "&#128075;" },
		{":raisedhand:", "&#9995;" },
		{":vulcansalute:", "&#128406;" },
		{":okhand:", "&#128076;" },
		{":pinchinghand:", "&#129295;" },
		{":victoryhand:", "&#9996;&#65039;" },
		{":crossedfingers:", "&#129310;" },
		{":thumbsup:", "&#128077;" },
		{":thumbsdown:", "&#128078;" },
		{":smirk:", "&#128527;" },
		{":pleading:", "&#129402;" },
		// Start custom emotes
		{":4weird:", "<img src='https://cdn.discordapp.com/emojis/673591686785662992.png?v=1' style='align-items: center; object-fit: contain;' height='35'>" },
		{":peepoowl:", "<img src='https://cdn.discordapp.com/emojis/716087641787072572.png?v=1' style='align-items: center; object-fit: contain;' height='35'>" },
		{":chefkiss:", "<img src='https://cdn.discordapp.com/emojis/703369938446057523.png?v=1' style='align-items: center; object-fit: contain;' height='35'>" },
		{":peeposmile:", "<img src='https://cdn.discordapp.com/emojis/735890951888699533.png?v=1' style='align-items: center; object-fit: contain;' height='35'>" },
		{":lul:", "<img src='https://cdn.discordapp.com/emojis/332086892235456514.png?v=1' style='align-items: center; object-fit: contain;' height='35'>" },
		{":pepetux:", "<img src='https://cdn.discordapp.com/emojis/690775603695058946.png?v=1' style='align-items: center; object-fit: contain;' height='35'>" },
		{":yay:", "<img src='https://cdn.discordapp.com/emojis/682137423022784512.gif?v=1' style='align-items: center; object-fit: contain;' height='35'>" },
		{":alienpls:", "<img src='https://cdn.discordapp.com/emojis/740294128650420376.gif?v=1' style='align-items: center; object-fit: contain;' height='35'>" },
		{":pikawha:", "<img src='https://cdn.discordapp.com/emojis/735311551711543357.png?v=1' style='align-items: center; object-fit: contain;' height='35'>" },
		{":skypemooning:", "<img src='https://cdn.discordapp.com/emojis/690775793978179645.gif?v=1' style='align-items: center; object-fit: contain;' height='35'>" },
		{":partyblob:", "<img src='https://cdn.discordapp.com/emojis/599687667839664198.gif?v=1' style='align-items: center; object-fit: contain;' height='35'>" },
	};
}
