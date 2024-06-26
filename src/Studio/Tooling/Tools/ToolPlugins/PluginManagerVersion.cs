using System.Text.RegularExpressions;

namespace Mine.Studio;

public readonly struct PluginManagerVersion
{
	private readonly struct StringIntVariant
	{
		public readonly string StringValue;
		public readonly long   LongValue;
		public readonly bool   IsValidNumber;

		public StringIntVariant(string value)
		{
			StringValue   = value;
			IsValidNumber = long.TryParse(value, out LongValue);
		}
			
		public static bool operator ==(StringIntVariant v1, StringIntVariant v2)
		{
			return string.CompareOrdinal(v1.StringValue, v2.StringValue) == 0;
		}

		public static bool operator !=(StringIntVariant v1, StringIntVariant v2)
		{
			return !(v1 == v2);
		}

		public static bool operator <(StringIntVariant v1, StringIntVariant v2)
		{
			if (v1.IsValidNumber && v2.IsValidNumber)
			{
				return v1.LongValue < v2.LongValue; // compare by numeric value
			}

			if (v1.IsValidNumber)
			{
				return true; // number is always less than text
			}

			if (v2.IsValidNumber)
			{
				return false; // text is always greater than number
			}

			return string.CompareOrdinal(v1.StringValue, v2.StringValue) < 0; // string comparison
		}

		public static bool operator >(StringIntVariant v1, StringIntVariant v2)
		{
			if (v1.IsValidNumber && v2.IsValidNumber)
			{
				return v1.LongValue > v2.LongValue; // compare by numeric value
			}

			if (v1.IsValidNumber)
			{
				return false; // number is always less than text
			}

			if (v2.IsValidNumber)
			{
				return true; // text is always greater than number
			}

			return string.CompareOrdinal(v1.StringValue, v2.StringValue) > 0; // string comparison
		}
			
		public bool Equals(StringIntVariant other)
		{
			return StringValue == other.StringValue && LongValue == other.LongValue && IsValidNumber == other.IsValidNumber;
		}

		public override bool Equals(object obj)
		{
			return obj is StringIntVariant other && Equals(other);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hash_code = (StringValue != null ? StringValue.GetHashCode() : 0);
				hash_code = (hash_code * 397) ^ LongValue.GetHashCode();
				hash_code = (hash_code * 397) ^ IsValidNumber.GetHashCode();
				return hash_code;
			}
		}
	}

	private readonly StringIntVariant[] Identifiers;
	public           bool               IsError => Identifiers == null;

	public PluginManagerVersion(string version)
	{
		// capture major, minor, patch and pre-release identifiers as single string, meta is ignored
		string pattern = @"^([0-9]+)\.([0-9]+)\.([0-9]+)(?:-([0-9a-zA-Z-]+(?:\.[0-9a-zA-Z-]+)*))?(?:\+(?:[0-9a-zA-Z-]+(?:\.[0-9a-zA-Z-]+)*))?$";
		Match  match   = Regex.Match(version, pattern);
		if(!match.Success)
		{
			Identifiers = null;
			return;
		}

		// split pre-release identifiers
		string   pre_release               = match.Groups[4].ToString();
		string[] pre_release_strings       = pre_release.Split('.');
		int      pre_release_strings_count = string.IsNullOrEmpty(pre_release_strings[0]) ? 0 : pre_release_strings.Length;

		// allocate
		Identifiers = new StringIntVariant[3 + pre_release_strings_count];
			
		//major, minor and patch
		Identifiers[0] = new StringIntVariant(match.Groups[1].ToString());
		Identifiers[1] = new StringIntVariant(match.Groups[2].ToString());
		Identifiers[2] = new StringIntVariant(match.Groups[3].ToString());

		// pre-release identifiers
		for(int pre_release_string_index=0; pre_release_string_index <pre_release_strings_count; ++pre_release_string_index)
		{
			Identifiers[3 + pre_release_string_index] = new StringIntVariant(pre_release_strings[pre_release_string_index]);
		}
	}

	public static bool IsApiBreak(PluginManagerVersion v1, PluginManagerVersion v2)
	{
		if (v1.Identifiers == null || v2.Identifiers == null)
		{
			return false;
		}

		if (v1.Identifiers.Length == 0 || v2.Identifiers.Length == 0)
		{
			return false;
		}

		return v1.Identifiers[0] != v2.Identifiers[0];
	}

	// comparison operators
	public static bool operator ==(PluginManagerVersion v1, PluginManagerVersion v2)
	{
		if (v1.Identifiers == null || v2.Identifiers == null)
		{
			return false;
		}

		if (v1.Identifiers.Length != v2.Identifiers.Length)
		{
			return false;
		}

		for (int index = 0; index < v1.Identifiers.Length; ++index)
		{
			if (v1.Identifiers[index] != v2.Identifiers[index])
			{
				return false;
			}
		}

		return true;
	}

	public static bool operator !=(PluginManagerVersion v1, PluginManagerVersion v2)
	{
		return !(v1 == v2);
	}

	public static bool operator <(PluginManagerVersion v1, PluginManagerVersion v2)
	{
		if (v1.Identifiers == null || v2.Identifiers == null)
		{
			return false;
		}

		// compare all common identifiers from left to right
		int count = Math.Min(v1.Identifiers.Length, v2.Identifiers.Length);
		for (int index = 0; index < count; ++index)
		{
			if (v1.Identifiers[index] == v2.Identifiers[index])
			{
				continue;
			}
			return v1.Identifiers[index] < v2.Identifiers[index];
		}

		// all common identifiers are equal, longest should be greater with exception - version with exactly 3 identifiers is greater than any prerelease
		if (v1.Identifiers.Length == v2.Identifiers.Length)
		{
			return false;
		}

		if (v1.Identifiers.Length == 3)
		{
			return false;
		}

		if (v2.Identifiers.Length == 3)
		{
			return true;
		}
			
		return v1.Identifiers.Length < v2.Identifiers.Length;
	}

	public static bool operator >(PluginManagerVersion v1, PluginManagerVersion v2)
	{
		if (v1.Identifiers == null || v2.Identifiers == null)
		{
			return false;
		}
			
		// compare all common identifiers from left to right
		int count = Math.Min(v1.Identifiers.Length, v2.Identifiers.Length);
		for (int index = 0; index < count; ++index)
		{
			if (v1.Identifiers[index] == v2.Identifiers[index])
			{
				continue;
			}
			return v1.Identifiers[index] > v2.Identifiers[index];
		}

		// all common identifiers are equal, longest should be greater with exception - version with exactly 3 identifiers is greater than any prerelease
		if (v1.Identifiers.Length == v2.Identifiers.Length)
		{
			return false;
		}

		if (v1.Identifiers.Length == 3)
		{
			return true;
		}

		if (v2.Identifiers.Length == 3)
		{
			return false;
		}
			
		return v1.Identifiers.Length > v2.Identifiers.Length;
	}

	public bool Equals(PluginManagerVersion other)
	{
		return Equals(Identifiers, other.Identifiers);
	}

	public override bool Equals(object? obj)
	{
		return obj is PluginManagerVersion other && Equals(other);
	}

	public override int GetHashCode()
	{
		return (Identifiers != null ? Identifiers.GetHashCode() : 0);
	}
}