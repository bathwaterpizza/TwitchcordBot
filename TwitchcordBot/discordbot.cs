using Discord;
using System;

static class DiscordBot
{
	private static DiscordClient discordClient;
	private static readonly ulong[] _discordWhitelist =
	{
		124155830655254530ul, // lookez
		202857441757495296ul, // caricaz
		189048718664663042ul, // betolixo
		236347150181597185ul, // gglucascomap
		202851317687910400ul, // woronkoff
		184345430300033024ul  // wuawydu
	};

	public static bool IsWhitelisted(ulong in_id)
	{
		foreach (var id in _discordWhitelist)
		{
			if (id == in_id)
				return true;
		}

		return false;
	}

	public static void Start()
	{
		discordClient = new DiscordClient();

		discordClient.MessageReceived += (s, e) =>
		{
			if (e.User.Name == "lkez") { return; } // ignore commands from self

			string channelMessage = e.Message.Text.ToLower();
			// make command to clear bot messages (DownloadMessages, foreach -> if message is mine Delete)

			#region Commands
			if (channelMessage.StartsWith(">>")) // >> Find and print tag <any>
				DiscordBotCommands.Tags(e, ">>");

			else if (channelMessage.StartsWith("tags>>")) // >> Run tag operations <add, del, list>
				DiscordBotCommands.Tags(e, channelMessage.Substring(6).Split(' ')[0]);

			else if (channelMessage.StartsWith("web>>")) // >> Run web operations <image, image18+>
				DiscordBotCommands.Web(e, channelMessage.Substring(5).Split(' ')[0]);

			else if (channelMessage.StartsWith("twitch>>")) // >> Run TwitchBot operations <msg, rawmsg>
				DiscordBotCommands.Twitch(e, channelMessage.Substring(8).Split(' ')[0]);

			else if (channelMessage.StartsWith("log>>")) // >> Control bot logging <enable, disable>
				DiscordBotCommands.Log(e, channelMessage.Substring(5).Split(' ')[0]);

			else return;
			#endregion Commands

			#region Log Command
			string cmdLogStr = e.User.Name + " -> usou comando \"" + e.Message.Text + "\"";
			Logger.LogCmd("[Discord] " + cmdLogStr);
			#endregion Log Command
		};

		discordClient.ExecuteAndWait(async () =>
		{
			try
			{
				await discordClient.Connect(Config.DiscordToken, Config.DiscordTokenType);
			}
			catch
			{
				Logger.LogAll("ERRO! Falha ao conectar com o Discord");
			}
			discordClient.SetGame("By Lookez");

			Logger.LogAll("Conectado ao Discord \"Player2Player\"");
		});
	}
}