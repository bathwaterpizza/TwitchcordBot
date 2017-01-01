using System;
using System.IO;
using System.Linq;

static class CommandHelper
{
	public static bool IsValidFilename(string str)
	{
		return !str.Any(Path.GetInvalidFileNameChars().Contains);
	}

	public static string OverwatchPlaytime(string playtime, string playtime_ranked)
	{
		int ptime = Convert.ToInt32(playtime.Split(' ')[0]);
		int ptime_ranked = Convert.ToInt32(playtime_ranked.Split(' ')[0]);

		return (ptime + ptime_ranked).ToString() + "h";
	}

	public static string OverwatchRankName(int rank)
	{
		string rankName = " (Bronze)";

		if (rank < 1500)
			return rankName;
		else if (rank < 2000)
			rankName = " (Silver)";
		else if (rank < 2500)
			rankName = " (Gold)";
		else if (rank < 3000)
			rankName = " (Platinum)";
		else if (rank < 3500)
			rankName = " (Diamond)";
		else if (rank < 4000)
			rankName = " (Master)";
		else
			rankName = " (Grandmaster)";

		return rankName;
	}
}