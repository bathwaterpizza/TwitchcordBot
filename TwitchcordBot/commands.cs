using System;
using System.IO;
using TwitchLib.Events.Client;
using System.Net;
using Newtonsoft.Json;
using Discord;
using System.Net.Http;
using RestSharp.Extensions.MonoHttp;

// Change command permission checks to support IsModerator and HasRole<adm, mod>
static class TwitchBotCommands
{
	public static void Tags(OnMessageReceivedArgs eventArgs, string command)
	{
		switch (command)
		{
			case ">>":
				string tag = eventArgs.ChatMessage.Message.Substring(2);

				if (tag.Split(' ').Length > 1 || !CommandHelper.IsValidFilename(tag))
				{
					TwitchBot.twitchClient.SendMessage("@" + eventArgs.ChatMessage.DisplayName + " -> Tag não encontrada");
					break;
				}

				if (!File.Exists("data/tags/" + tag + ".txt"))
				{
					TwitchBot.twitchClient.SendMessage("@" + eventArgs.ChatMessage.DisplayName + " -> Tag não encontrada");
					break;
				}

				tag = File.ReadAllText("data/tags/" + tag + ".txt");
				TwitchBot.twitchClient.SendMessage(tag);
				break;

			case "add":
				if (!TwitchBot.IsWhitelisted(eventArgs.ChatMessage.Username))
				{
					TwitchBot.twitchClient.SendMessage("@" + eventArgs.ChatMessage.DisplayName + " -> Acesso negado");
					break;
				}

				string addTag = eventArgs.ChatMessage.Message.Substring(10).Split(' ')[0];
				if (!CommandHelper.IsValidFilename(addTag))
				{
					TwitchBot.twitchClient.SendMessage("@" + eventArgs.ChatMessage.DisplayName + " -> Tag inválida");
					break;
				}
				if (!File.Exists("data/tags/" + addTag + ".txt"))
				{
					FileStream fs = File.Create("data/tags/" + addTag + ".txt");
					fs.Close(); fs.Dispose();
				}
				int tagLen = addTag.Length + 1;

				File.WriteAllText("data/tags/" + addTag + ".txt", eventArgs.ChatMessage.Message.Substring(10 + tagLen));
				TwitchBot.twitchClient.SendMessage("@" + eventArgs.ChatMessage.DisplayName + " -> Tag adicionada");
				break;

			case "del":
				if (!TwitchBot.IsWhitelisted(eventArgs.ChatMessage.Username))
				{
					TwitchBot.twitchClient.SendMessage("@" + eventArgs.ChatMessage.DisplayName + " -> Acesso negado");
					break;
				}
				string delTag = eventArgs.ChatMessage.Message.Substring(10);

				if (!File.Exists("data/tags/" + delTag + ".txt"))
				{
					TwitchBot.twitchClient.SendMessage("@" + eventArgs.ChatMessage.DisplayName + " -> Tag não encontrada");
					break;
				}

				File.Delete("data/tags/" + delTag + ".txt");
				TwitchBot.twitchClient.SendMessage("@" + eventArgs.ChatMessage.DisplayName + " -> Tag removida");
				break;

			case "list":
				string[] fileEntries = Directory.GetFiles("data/tags");
				string tagList = "Tags: ";

				foreach (var fileName in fileEntries)
					tagList = tagList + fileName.Split('\\')[1].Replace(".txt", "") + ", ";

				TwitchBot.twitchClient.SendMessage(tagList.Substring(0, tagList.Length - 2));
				break;

			default:
				TwitchBot.twitchClient.SendMessage("@" + eventArgs.ChatMessage.DisplayName + " -> Comando desconhecido");
				break;
		}
	}

	public static async void Overwatch(OnMessageReceivedArgs eventArgs, string command)
	{
		switch (command)
		{
			case "inspect":
				try
				{
					string btag_raw = eventArgs.ChatMessage.Message.Substring(19);
					string btag;

					if (btag_raw == "caricaz")
						btag = "caricaz-1399";
					else if (btag_raw == "wydu")
						btag = "Wuawydu-1490";
					else if (btag_raw == "duque")
						btag = "Duquin-1491";
					else if (btag_raw == "spell")
						btag = "Spell-1869";
					else
						btag = btag_raw.Replace('#', '-');

					using (WebClient wclient = new WebClient())
					{
						string response = await wclient.DownloadStringTaskAsync(new Uri("https://api.lootbox.eu/pc/us/" + btag + "/profile"));
						if (response.StartsWith(@"{""statusCode"":404"))
						{
							TwitchBot.twitchClient.SendMessage("Jogador não encontrado");
							break;
						}
						dynamic json = JsonConvert.DeserializeObject(response);

						string level = json.data.level;
						string wins = json.data.games.quick.wins;
						string rwins = json.data.games.competitive.wins;
						string rtotal = json.data.games.competitive.played;
						string rptime = json.data.playtime.competitive;
						string ptime = json.data.playtime.quick;
						string rank = json.data.competitive.rank;
						string totalWin = (Convert.ToInt32(wins) + Convert.ToInt32(rwins)).ToString();
						string totalPtime = CommandHelper.OverwatchPlaytime(ptime, rptime);
						string winRate = ((Convert.ToDouble(rwins) / Convert.ToDouble(rtotal)).Truncate(4) * 100).ToString();

						response = btag.Replace('-', '#') + ": Level " + level + " | Playtime " + totalPtime + " (" + rptime.Split(' ')[0] + "h ranked) | Wins " +
							totalWin + " (" + rwins + " ranked) | Ranked Winrate " + winRate + "% | MMR " + rank + CommandHelper.OverwatchRankName(Convert.ToInt32(rank));
						TwitchBot.twitchClient.SendMessage(response);
					}
				}
				catch
				{
					TwitchBot.twitchClient.SendMessage("@" + eventArgs.ChatMessage.DisplayName + " -> Jogador não encontrado");
				}
				break;

			default:
				TwitchBot.twitchClient.SendMessage("@" + eventArgs.ChatMessage.DisplayName + " -> Comando desconhecido");
				break;
		}
	}
}

static class DiscordBotCommands
{
	public static void Tags(MessageEventArgs eventArgs, string command)
	{
		switch (command)
		{
			case ">>":
				string tag = eventArgs.Message.Text.Substring(2);

				if (tag.Split(' ').Length > 1 || !CommandHelper.IsValidFilename(tag))
				{
					eventArgs.Channel.SendMessage(eventArgs.User.Name + " -> Tag não encontrada");
					break;
				}

				if (!File.Exists("data/tags/" + tag + ".txt"))
				{
					eventArgs.Channel.SendMessage(eventArgs.User.Name + " -> Tag não encontrada");
					break;
				}

				tag = File.ReadAllText("data/tags/" + tag + ".txt");
				eventArgs.Channel.SendMessage(tag);
				break;

			case "add":
				if (!DiscordBot.IsWhitelisted(eventArgs.User.Id))
				{
					eventArgs.Channel.SendMessage(eventArgs.User.Name + " -> Acesso negado");
					break;
				}

				string addTag = eventArgs.Message.Text.Substring(10).Split(' ')[0];
				if (!CommandHelper.IsValidFilename(addTag))
				{
					eventArgs.Channel.SendMessage(eventArgs.User.Name + " -> Tag inválida");
					break;
				}
				if (!File.Exists("data/tags/" + addTag + ".txt"))
				{
					FileStream fs = File.Create("data/tags/" + addTag + ".txt");
					fs.Close(); fs.Dispose();
				}
				int tagLen = addTag.Length + 1;

				File.WriteAllText("data/tags/" + addTag + ".txt", eventArgs.Message.Text.Substring(10 + tagLen));
				eventArgs.Channel.SendMessage(eventArgs.User.Name + " -> Tag adicionada");
				break;

			case "del":
				if (!DiscordBot.IsWhitelisted(eventArgs.User.Id))
				{
					eventArgs.Channel.SendMessage(eventArgs.User.Name + " -> Acesso negado");
					break;
				}
				string delTag = eventArgs.Message.Text.Substring(10);

				if (!File.Exists("data/tags/" + delTag + ".txt"))
				{
					eventArgs.Channel.SendMessage(eventArgs.User.Name + " -> Tag não encontrada");
					break;
				}

				File.Delete("data/tags/" + delTag + ".txt");
				eventArgs.Channel.SendMessage(eventArgs.User.Name + " -> Tag removida");
				break;

			case "list":
				string[] fileEntries = Directory.GetFiles("data/tags");
				string tagList = "Tags: ";

				foreach (var fileName in fileEntries)
					tagList = tagList + fileName.Split('\\')[1].Replace(".txt", "") + ", ";

				eventArgs.Channel.SendMessage(tagList.Substring(0, tagList.Length - 2));
				break;

			default:
				eventArgs.Channel.SendMessage(eventArgs.User.Name + " -> Comando desconhecido");
				break;
		}
	}

	public static void Twitch(MessageEventArgs eventArgs, string command)
	{
		switch (command)
		{
			case "msg":
				if (eventArgs.Message.Text.Contains("http://") || eventArgs.Message.Text.Contains("https://") || eventArgs.Message.Text.Contains("www.")
				|| eventArgs.Message.Text.Contains(".com") || eventArgs.Message.Text.Contains(".net") || eventArgs.Message.Text.Contains(".co"))
				{
					eventArgs.Channel.SendMessage(eventArgs.User.Name + " -> Links são proibidos.");
					return;
				}

				TwitchBot.twitchClient.SendMessage("(Discord) " + eventArgs.User.Name + ": " + eventArgs.Message.Text.Substring(12));
				break;

			case "rawmsg":
				if (!DiscordBot.IsWhitelisted(eventArgs.User.Id))
				{
					eventArgs.Channel.SendMessage(eventArgs.User.Name + " -> Acesso negado");
					break;
				}

				TwitchBot.twitchClient.SendMessage(eventArgs.Message.Text.Substring(15));
				break;

			default:
				eventArgs.Channel.SendMessage(eventArgs.User.Name + " -> Comando desconhecido");
				break;
		}
	}

	public static void Log(MessageEventArgs eventArgs, string command)
	{
		switch (command)
		{
			case "enable":
				if (!DiscordBot.IsWhitelisted(eventArgs.User.Id))
				{
					eventArgs.Channel.SendMessage(eventArgs.User.Name + " -> Acesso negado");
					break;
				}

				Config.PauseLogging = false;
				eventArgs.Channel.SendMessage(eventArgs.User.Name + " -> Log ativado");
				break;

			case "disable":
				if (!DiscordBot.IsWhitelisted(eventArgs.User.Id))
				{
					eventArgs.Channel.SendMessage(eventArgs.User.Name + " -> Acesso negado");
					break;
				}

				Config.PauseLogging = true;
				eventArgs.Channel.SendMessage(eventArgs.User.Name + " -> Log desativado");
				break;

			default:
				eventArgs.Channel.SendMessage(eventArgs.User.Name + " -> Comando desconhecido");
				break;
		}
	}

	public static async void Web(MessageEventArgs eventArgs, string command)
	{
		switch (command)
		{
			case "image":
				string arg = eventArgs.Message.Text.Substring(11).Replace(" ", "%20");
				string offset = "0"; int len = arg.Length;
				HttpClient hclient = new HttpClient();
				hclient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", Config.BingAPIKey);

				if ((len > 5) && (arg.Substring(len - 5, 4) == "%20#"))
				{ offset = arg.Substring(len - 1); arg = arg.Substring(0, len - 5); }

				string url = "https://api.cognitive.microsoft.com/bing/v5.0/images/search?q=" +
				arg + "&count=1&offset=" + offset + "&mkt=pt-br&safeSearch=Strict";
				string webResponse = await hclient.GetStringAsync(url); hclient.Dispose();
				dynamic imageUrl = JsonConvert.DeserializeObject(webResponse);

				if (webResponse.Contains("\"value\": [],")) { webResponse = "Erro: imagem não encontrada"; }
				else { webResponse = HttpUtility.ParseQueryString(new Uri(imageUrl.value[0].contentUrl.ToString()).Query)["r"]; }

				await eventArgs.Channel.SendMessage(webResponse);
				break;

			case "image18+":
				if (!DiscordBot.IsWhitelisted(eventArgs.User.Id))
				{
					await eventArgs.Channel.SendMessage(eventArgs.User.Name + " -> Acesso negado");
					break;
				}

				string _arg = eventArgs.Message.Text.Substring(14).Replace(" ", "%20");
				string _offset = "0"; int _len = _arg.Length;
				HttpClient _hclient = new HttpClient();
				_hclient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", Config.BingAPIKey);

				if ((_len > 5) && (_arg.Substring(_len - 5, 4) == "%20#"))
				{ _offset = _arg.Substring(_len - 1); _arg = _arg.Substring(0, _len - 5); }

				string _url = "https://api.cognitive.microsoft.com/bing/v5.0/images/search?q=" +
				_arg + "&count=1&offset=" + _offset + "&mkt=pt-br&safeSearch=Off";
				string _webResponse = await _hclient.GetStringAsync(_url); _hclient.Dispose();
				dynamic _imageUrl = JsonConvert.DeserializeObject(_webResponse);

				if (_webResponse.Contains("\"value\": [],")) { _webResponse = "Erro: imagem não encontrada"; }
				else { _webResponse = HttpUtility.ParseQueryString(new Uri(_imageUrl.value[0].contentUrl.ToString()).Query)["r"]; }

				await eventArgs.Channel.SendMessage(_webResponse);
				break;

			default:
				await eventArgs.Channel.SendMessage(eventArgs.User.Name + " -> Comando desconhecido");
				break;
		}
	}
}