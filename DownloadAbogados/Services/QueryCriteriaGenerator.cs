using System;
using DownloadAbogados.Data;

namespace DownloadAbogados.Services;

public class QueryCriteriaGenerator
{
	private string firstLetter = "a";
	private string secondLetter = "a";
	private string thirdLetter = "a";
	private string currentParameter = "";

	public string? GetNextQueryParameter()
	{
		if (currentParameter == "zzz") return null;
		if (thirdLetter != "z")
		{
			thirdLetter = GetNextLetter(thirdLetter);
		}
		else
		{
			thirdLetter = "a";
			if (secondLetter != "z")
			{
				secondLetter = GetNextLetter(secondLetter);
			}
			else
			{
				secondLetter = "a";
				firstLetter = GetNextLetter(firstLetter);
			}
		}

		currentParameter = firstLetter + secondLetter + thirdLetter;
		return currentParameter;
	}

	private string GetNextLetter(string letter)
	{
		if (letter.Length != 1 || letter[0] < 'a' || letter[0] > 'z')
			throw new ArgumentException("Invalid letter");

		if (letter == "z")
			return "a";

		char nextChar = (char)(letter[0] + 1);
		return nextChar.ToString();
	}
}
