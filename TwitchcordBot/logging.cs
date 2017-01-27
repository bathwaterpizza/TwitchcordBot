using System;
using System.IO;

static class Logger
{
	private static void ParsePrintPrefixes(string msg)
	{
		#region Parse Prefixes
		if (msg.Contains("[Twitch]"))
		{
			Console.ForegroundColor = ConsoleColor.Magenta;
			Console.Write("[Twitch] ");
			msg = msg.Replace("[Twitch] ", "");

			string[] _parsed = msg.Split(' ');
			string _username = _parsed[0];
			_parsed = _parsed.RemoveAt(0);
			Console.ForegroundColor = ConsoleColor.Cyan;
			Console.Write(_username + " ");
			Console.ForegroundColor = ConsoleColor.White;
			Console.WriteLine(string.Join(" ", _parsed));

			return;
		}
		if (msg.Contains("[Twitch Staff]"))
		{
			Console.ForegroundColor = ConsoleColor.Red;
			Console.Write("[Twitch Staff]");
			msg = msg.Replace("[Twitch Staff]", "");
		}
		if (msg.Contains("[Global Admin]"))
		{
			Console.ForegroundColor = ConsoleColor.Red;
			Console.Write("[Global Admin]");
			msg = msg.Replace("[Global Admin]", "");
		}
		if (msg.Contains("[Global Mod]"))
		{
			Console.ForegroundColor = ConsoleColor.Red;
			Console.Write("[Global Mod]");
			msg = msg.Replace("[Global Mod]", "");
		}
		if (msg.Contains("[Streamer]"))
		{
			Console.ForegroundColor = ConsoleColor.Red;
			Console.Write("[Streamer]");
			msg = msg.Replace("[Streamer]", "");
		}
		if (msg.Contains("[Mod]"))
		{
			Console.ForegroundColor = ConsoleColor.Blue;
			Console.Write("[Mod]");
			msg = msg.Replace("[Mod]", "");
		}
		if (msg.Contains("[Sub]"))
		{
			Console.ForegroundColor = ConsoleColor.Cyan;
			Console.Write("[Sub]");
			msg = msg.Replace("[Sub]", "");
		}
		#endregion Parse Prefixes

		string newMsg = string.Empty;
		bool appendColon = false;
		string[] parsed = msg.Split(':'); parsed[1] = ":" + parsed[1];
		string username = parsed[0].Substring(1);
		parsed = parsed.RemoveAt(0);

		Console.ForegroundColor = ConsoleColor.Cyan;
		if (username.ToLower() == "nightbot" || username.ToLower() == "moobot" || username.ToLower() == "p2pbot")
		{
			Console.ForegroundColor = ConsoleColor.Green;
			Console.Write(" BOT");
			Console.ForegroundColor = ConsoleColor.White;
			Console.Write('-');
			Console.ForegroundColor = ConsoleColor.Cyan;
			Console.Write(username);
		}
		else
		{
			Console.Write(" " + username);
		}

		foreach (var str in parsed)
		{
			if (appendColon)
			{
				newMsg = newMsg + ":" + str;
				continue;
			}
			newMsg = newMsg + str;
			appendColon = true;
		}

		// Highlight player2player and lookezbr (only Config.TwitchChannel if I release this)
		if (newMsg.ToLower().Contains(Config.TwitchChannel) || newMsg.ToLower().Contains("lookezbr"))
		{
			Console.ForegroundColor = ConsoleColor.White;
			Console.Write(newMsg.Substring(0, 2));
			Console.BackgroundColor = ConsoleColor.Red;
			Console.WriteLine(newMsg.Substring(2));
			Console.BackgroundColor = ConsoleColor.Black;
		}
		else
		{
			Console.ForegroundColor = ConsoleColor.White;
			Console.WriteLine(newMsg);
		}
	}

	public static void LogChat(string message)
	{
		try // Trying to catch messages randomly stop being printed to chat, possible error in ParsePrintPrefixes
		{
			if (Config.PauseLogging) { return; }

			string logTime = DateTime.Now.ToString("[dd/MM/yyyy HH:mm:ss]");
			string fileName = "data/chat_log/" + DateTime.Now.ToString("dd-MM-yy") + ".txt";

			if (!File.Exists(fileName))
				File.Create(fileName).Dispose();

			using (var file = new StreamWriter(fileName, true))
			{
				file.WriteLine(logTime + message);
				file.Close();
			}

			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.Write(DateTime.Now.ToString("[hh:mm tt]"));
			ParsePrintPrefixes(message);
		}
		catch (Exception ex)
		{
			PrintSystem("------------ ERROR ------------ \n" + ex.ToString());
		}
	}

	public static void LogCmd(string command)
	{
		if (Config.PauseLogging) { return; }

		string logTime = DateTime.Now.ToString("[dd/MM/yyyy HH:mm:ss]");
		string fileName = "data/cmd_log/" + DateTime.Now.ToString("dd-MM-yy") + ".txt";

		if (!File.Exists(fileName))
			File.Create(fileName).Dispose();

		using (var file = new StreamWriter(fileName, true))
		{
			file.WriteLine(logTime + command);
			file.Close();
		}

		Console.ForegroundColor = ConsoleColor.Yellow;
		Console.Write(DateTime.Now.ToString("[hh:mm tt]"));
		Console.ForegroundColor = ConsoleColor.Red;
		Console.WriteLine(command);
	}

	public static void LogAll(string log)
	{
		if (Config.PauseLogging) { return; }

		log = " " + log;
		string logTime = DateTime.Now.ToString("[dd/MM/yyyy HH:mm:ss]");
		string cmd_fileName = "data/cmd_log/" + DateTime.Now.ToString("dd-MM-yy") + ".txt";

		if (!File.Exists(cmd_fileName))
			File.Create(cmd_fileName).Dispose();

		using (var file = new StreamWriter(cmd_fileName, true))
		{
			file.WriteLine(logTime + log);
			file.Close();
		}

		Console.ForegroundColor = ConsoleColor.Yellow;
		Console.Write(DateTime.Now.ToString("[hh:mm tt]"));

		if (log.Contains("ERRO!"))
			Console.ForegroundColor = ConsoleColor.Red;
		else
			Console.ForegroundColor = ConsoleColor.Green;

		Console.WriteLine(log);
	}

	public static void PrintSystem(string str)
	{
		Console.ForegroundColor = ConsoleColor.Yellow;
		Console.Write("[SYSTEM] ");
		Console.ForegroundColor = ConsoleColor.Green;
		Console.WriteLine(str);
	}
}