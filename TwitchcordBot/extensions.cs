using System;

static class Extensions
{
	/// <summary>
	/// Truncates a decimal value to x number of decimal digits
	/// </summary>
	/// <param name="digits">Number of decimal digits to show after Truncate</param>
	/// <returns>Truncated double value</returns>
	public static double Truncate(this double value, int digits)
	{
		double mult = Math.Pow(10.0, digits);
		double result = Math.Truncate(mult * value) / mult;
		return result;
	}
}