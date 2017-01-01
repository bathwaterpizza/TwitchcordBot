using System;
using System.IO;

static class BotMain
{
	private static void InitFiles()
	{
		if (!Directory.Exists("data"))
		{
			Directory.CreateDirectory("data");
			Logger.PrintSystem("Criando diretório \"data\"");
		}
		if (!Directory.Exists("data/chat_log"))
		{
			Directory.CreateDirectory("data/chat_log");
			Logger.PrintSystem("Criando diretório \"data/chat_log\"");
		}
		if (!Directory.Exists("data/cmd_log"))
		{
			Directory.CreateDirectory("data/cmd_log");
			Logger.PrintSystem("Criando diretório \"data/cmd_log\"");
		}
		if (!Directory.Exists("data/tags"))
		{
			Directory.CreateDirectory("data/tags");
			Logger.PrintSystem("Criando diretório \"data/tags\"");
		}
	}

	// Change command permission checks to support IsModerator and HasRole<adm, mod>
	private static void Main()
	{
		Console.Title = "P2P Twitchcord Bot";
		Console.OutputEncoding = System.Text.Encoding.UTF8;
		InitFiles();

		Logger.LogAll("Inicializando Bots");
		TwitchBot.Start();
		DiscordBot.Start();
	}
}