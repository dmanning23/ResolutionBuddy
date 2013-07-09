using System;
using System.Text;

namespace RandomExtensions
{
	/// <summary>
	/// Class for extending the built-in Random class
	/// </summary>
	public static class RandomExtensions
	{
		/// <summary>
		/// Get a random double within a specified range.  The default version only returns between 0.0-1.0
		/// </summary>
		/// <param name="rand">teh rand object we are extending</param>
		/// <param name="min">minimum number, inclusive</param>
		/// <param name="max">maximum number, inclusive</param>
		/// <returns>double: a random double between the min and max</returns>
		public static double NextDouble(this Random rand, double min, double max)
		{
			if (min > max)
			{
				throw new ArgumentException("min must be less than max");
			}
			return rand.NextDouble() * (max - min) + min;
		}

		/// <summary>
		/// Man, why the built in Random hate floats so much?  let's sort of fix that
		/// </summary>
		/// <param name="rand">teh rand object we are extending</param>
		/// <returns>float: a random double casted to a float</returns>
		public static float NextFloat(this Random rand)
		{
			return (float)rand.NextDouble();
		}

		/// <summary>
		/// Get a random float within a specified range.  The default version only returns between 0.0-1.0
		/// </summary>
		/// <param name="rand">teh rand object we are extending</param>
		/// <param name="min">minimum number, inclusive</param>
		/// <param name="max">maximum number, inclusive</param>
		/// <returns>double: a random float between the min and max</returns>
		public static float NextFloat(this Random rand, float min, float max)
		{
			if (min > max)
			{
				throw new ArgumentException("min must be less than max");
			}
			return rand.NextFloat() * (max - min) + min;
		}

		/// <summary>
		/// Get a random english-ish sounding word
		/// </summary>
		/// <param name="rand">teh random object we are extending</param>
		/// <param name="minLength">min length of the name, must be at least 2</param>
		/// <param name="maxLength">max length of the name, must be greater than or equal to min</param>
		/// <returns>a name-ish sound string, between the length of min-max</returns>
		public static string NextWord(this Random rand, int minLength, int maxLength)
		{
			//make sure the min is long enough (must be at least 2 letters)
			if (minLength < 2)
			{
				throw new ArgumentException("minLength must be at least 2");
			}

			//Get the length of the name we are gonna generate
			int nameLength = rand.Next(minLength, maxLength);

			//create an empty word
			StringBuilder name = new StringBuilder();

			// The letters to choose from.  we add more of some so that we are more likely to get those letters
			string[] consonants = new string[] 
			{
				"b", "b", "c", "d", "d", "f","g","g","h","h","j","k","l","m","m","n","n","p","p","qu","r","r","s","s","s","t","t","t","v","w","x","z",
				"ck", "ck", "sh", "ch", "nn", "gh", "ll", "st", "st", "mn", "sp", "sp","ss", "tt"
			};
			string[] vowels = new string[] 
			{
				"a","a","a","e","e","e","i","i","o","o","u","y", 
				"ou", "ea", "oo"
			};

			//Should this name start with a consonant?
			bool startConsonant = 0 == rand.Next(0, 2);

			//first fill the name with random consonants
			while (name.Length < nameLength)
			{
				//do we want a consonant or a vowel?
				if (startConsonant)
				{
					name.Append(consonants[rand.Next(0, consonants.Length)]);
				}
				else
				{
					name.Append(vowels[rand.Next(0, vowels.Length)]);
				}
				startConsonant = !startConsonant;
			}

			return name.ToString();
		}

		/// <summary>
		/// Get a random english-ish sounding name... same as a word, but the first letter is uppercase
		/// </summary>
		/// <param name="rand">teh random object we are extending</param>
		/// <param name="minLength">min length of the name, must be at least 2</param>
		/// <param name="maxLength">max length of the name, must be greater than or equal to min</param>
		/// <returns>a name-ish sound string, between the length of min-max</returns>
		public static string NextName(this Random rand, int minLength, int maxLength)
		{
			//get a random word
			StringBuilder name = new StringBuilder(rand.NextWord(minLength, maxLength));

			//the first letter should be uppercase!
			name[0] = (Char.ToUpper(name[0]));

			return name.ToString();
		}

		/// <summary>
		/// Get a random swear word
		/// </summary>
		/// <param name="rand">teh random object we are extending</param>
		/// <returns>a four letter swear word</returns>
		public static string NextSwearWord(this Random rand)
		{
			return rand.NextWord(4, 4);
		}
	}
}
