using TwitchLib;
using TwitchLib.Models.Client;

static class TwitchBot
{
	public static TwitchClient twitchClient;
	private static readonly string[] _twitchWhitelist =
 	{
		"lookezbr",
		"betolixo",
		"player2player",
		"gglucascomap",
		"woronkoff"
	};

	public static bool IsWhitelisted(string nick)
	{
		foreach (var name in _twitchWhitelist)
		{
			if (name == nick.ToLower())
				return true;
		}

		return false;
	}

	public static void Start()
	{
		try
		{
			twitchClient = new TwitchClient(new ConnectionCredentials(Config.TwitchUsername, Config.TwitchAccessToken), Config.TwitchChannel);
			twitchClient.Connect();
		}
		catch
		{
			Logger.LogAll("ERRO! Falha ao conectar com a Twitch");
		}

		twitchClient.OnConnected += (s, e) => { Logger.LogAll("Conectado a Twitch \"Player2Player\""); };

		twitchClient.OnUserTimedout += (s, e) =>
		{ Logger.LogChat("[Twitch] " + e.Username + " -> Timed Out (" + e.TimeoutDuration.ToString() + " Segundos)"); };

		twitchClient.OnUserBanned += (s, e) =>
		{ Logger.LogChat("[Twitch] " + e.Username + " -> Banido"); };

		twitchClient.OnNewSubscriber += (s, e) =>
		{ Logger.LogChat("[Twitch] " + e.Subscriber.Name + " -> Se Inscreveu"); };

		twitchClient.OnReSubscriber += (s, e) =>
		{ Logger.LogChat("[Twitch] " + e.ReSubscriber.DisplayName + " -> Se Reinscreveu (" + e.ReSubscriber.Months.ToString() + " Meses)"); };

		twitchClient.OnHostingStarted += (s, e) =>
		{ Logger.LogChat("[Twitch] Player2Player -> Hosteando " + e.TargetChannel); };

		twitchClient.OnHostingStopped += (s, e) =>
		{ Logger.LogChat("[Twitch] Player2Player -> Parou de Hostear"); };

		twitchClient.OnMessageReceived += (s, e) =>
		{
			if (e.ChatMessage.Bits > 0)
			{
				Logger.LogChat("[Twitch] " + e.ChatMessage.DisplayName + " -> Cheer de " + e.ChatMessage.Bits.ToString() +
					" Bits ($" + e.ChatMessage.BitsInDollars.ToString() + " USD)");
			}

			#region Log Chat
			string logStr = " " + e.ChatMessage.DisplayName + ": " + e.ChatMessage.Message;

			if (e.ChatMessage.Subscriber)
				logStr = "[Sub]" + logStr;

			if (e.ChatMessage.IsModerator)
				logStr = "[Mod]" + logStr;

			if (e.ChatMessage.IsBroadcaster)
				logStr = "[Streamer]" + logStr;

			if (e.ChatMessage.UserType == TwitchLib.Enums.UserType.GlobalModerator)
				logStr = "[Global Mod]" + logStr;
			else if (e.ChatMessage.UserType == TwitchLib.Enums.UserType.Admin)
				logStr = "[Global Admin]" + logStr;
			else if (e.ChatMessage.UserType == TwitchLib.Enums.UserType.Staff)
				logStr = "[Twitch Staff]" + logStr;

			Logger.LogChat(logStr);
			#endregion Log Chat

			string channelMessage = e.ChatMessage.Message.ToLower();

			#region Commands
			if (channelMessage.StartsWith(">>")) // >> Find and print tag <any>
				TwitchBotCommands.Tags(e, ">>");

			else if (channelMessage.StartsWith("tags>>")) // >> Run tag operations <add, del, list>
				TwitchBotCommands.Tags(e, channelMessage.Substring(6).Split(' ')[0]);

			else if (channelMessage.StartsWith("overwatch>>")) // >> Run overwatch requests <inspect>
				TwitchBotCommands.Overwatch(e, channelMessage.Substring(11).Split(' ')[0]);

			else return;
			#endregion Commands

			#region Log Command
			string cmdLogStr = " " + e.ChatMessage.DisplayName + " -> usou comando \"" + e.ChatMessage.Message + "\"";

			if (e.ChatMessage.Subscriber)
				cmdLogStr = "[Sub]" + cmdLogStr;

			if (e.ChatMessage.IsModerator)
				cmdLogStr = "[Mod]" + cmdLogStr;

			if (e.ChatMessage.IsBroadcaster)
				cmdLogStr = "[Streamer]" + cmdLogStr;

			if (e.ChatMessage.UserType == TwitchLib.Enums.UserType.GlobalModerator)
				cmdLogStr = "[Global Mod]" + cmdLogStr;
			else if (e.ChatMessage.UserType == TwitchLib.Enums.UserType.Admin)
				cmdLogStr = "[Global Admin]" + cmdLogStr;
			else if (e.ChatMessage.UserType == TwitchLib.Enums.UserType.Staff)
				cmdLogStr = "[Twitch Staff]" + cmdLogStr;

			Logger.LogCmd("[Twitch]" + cmdLogStr);
			#endregion Log Command
		};
	}
}