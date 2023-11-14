using System.Text;

namespace Mine.Studio;

public static class StringUtilities
{
	public static string PrettyCamelCase(this string self)
	{
		if (self.Length <= 1)
		{
			return self;
		}

		StringBuilder stringBuilder = new();

		bool newWord = true;
		char prev    = ' ';
		for(int i=0;i <self.Length;++i)
		{
			char ch = self[i];
			if (!char.IsLetterOrDigit(ch))
			{
				newWord = true;
				continue;
			}

			if (char.IsUpper(ch) && !char.IsUpper(prev))
			{
				newWord = true;
			}
			
			if(char.IsDigit(ch) && !char.IsDigit(prev))
			{
				newWord = true;
			}

			if (newWord)
			{
				if (stringBuilder.Length > 0)
				{
					stringBuilder.Append(' ');
				}

				stringBuilder.Append(char.ToUpper(ch));
				newWord = false;
			}
			else
			{
				stringBuilder.Append(ch);
			}

			prev = ch;
		}

		return stringBuilder.ToString();
	}
}