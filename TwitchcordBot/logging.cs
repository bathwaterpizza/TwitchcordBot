using System;
using System.IO;

static class Logger
{
	public static void LogChat(string message)
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
		Console.Write(DateTime.Now.ToString("[hh:mm]"));
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

		Console.ForegroundColor = ConsoleColor.Green;
		Console.WriteLine(DateTime.Now.ToString("[hh:mm]") + log);
	}

	public static void PrintSystem(string str)
	{
		Console.ForegroundColor = ConsoleColor.Yellow;
		Console.Write("[SYSTEM] ");
		Console.ForegroundColor = ConsoleColor.Green;
		Console.WriteLine(str);
	}
}