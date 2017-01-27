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

	/// <summary>
	/// Removes an element at x position from array and shifts elements back to fill the gap
	/// </summary>
	/// <param name="index">Element index to remove from array</param>
	/// <returns>New size len-1 array without the removed element</returns>
	public static T[] RemoveAt<T>(this T[] source, int index)
	{
		T[] dest = new T[source.Length - 1];
		if (index > 0)
			Array.Copy(source, 0, dest, 0, index);

		if (index < source.Length - 1)
			Array.Copy(source, index + 1, dest, index, source.Length - index - 1);

		return dest;
	}
}