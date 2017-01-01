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
		twitchClient = new TwitchClient(new ConnectionCredentials(Config.TwitchUsername, Config.TwitchAccessToken), Config.TwitchChannel);
		twitchClient.Connect();

		twitchClient.OnConnected += (s, e) => { Logger.LogAll("Conectado a Twitch \"Player2Player\""); };

		twitchClient.OnUserTimedout += (s, e) =>
		{ Logger.LogChat("[Twitch] " + e.Username + " -> timed out (" + e.TimeoutDuration.ToString() + " segundos)"); };

		twitchClient.OnUserBanned += (s, e) =>
		{ Logger.LogChat("[Twitch] " + e.Username + " -> banido (" + e.BanReason + ")"); };

		twitchClient.OnNewSubscriber += (s, e) =>
		{ Logger.LogChat("[Twitch] " + e.Subscriber.Name + " -> se inscreveu"); };

		twitchClient.OnReSubscriber += (s, e) =>
		{ Logger.LogChat("[Twitch] " + e.ReSubscriber.DisplayName + " -> se reinscreveu (" + e.ReSubscriber.Months.ToString() + " meses)"); };

		twitchClient.OnHostingStarted += (s, e) =>
		{ Logger.LogChat("[Twitch] Player2Player -> hosteando " + e.TargetChannel); };

		twitchClient.OnHostingStopped += (s, e) =>
		{ Logger.LogChat("[Twitch] Player2Player -> parou de hostear " + e.HostingChannel); };

		twitchClient.OnMessageReceived += (s, e) =>
		{
			#region Log Chat
			string logStr = " " + e.ChatMessage.DisplayName + ": " + e.ChatMessage.Message;

			if (e.ChatMessage.Subscriber)
				logStr = "[Sub]" + logStr;

			if (e.ChatMessage.IsModerator)
				logStr = "[Mod]" + logStr;

			if (e.ChatMessage.IsBroadcaster)
				logStr = "[Streamer]" + logStr;

			Logger.LogChat(logStr);
			#endregion Log Chat

			if (e.ChatMessage.Username == "lkezbot") { return; } // ignore messages from self

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
			string cmdLogStr = " " + e.ChatMessage.DisplayName + " >> usou comando \"" + e.ChatMessage.Message + "\"";

			if (e.ChatMessage.Subscriber)
				cmdLogStr = "[Sub]" + cmdLogStr;

			if (e.ChatMessage.IsModerator)
				cmdLogStr = "[Mod]" + cmdLogStr;

			if (e.ChatMessage.IsBroadcaster)
				cmdLogStr = "[Streamer]" + cmdLogStr;

			Logger.LogCmd("[Twitch]" + cmdLogStr);
			#endregion Log Command
		};
	}
}