namespace GameToolkit.Framework;

public static class ListExtensions
{
	public static int FindInsertionIndexBinary<T>(this List<T> list, Func<T, int> comparer)
	{
		int first = 0;
		int last  = list.Count;

		while (first < last)
		{
			int mid = (first + last) / 2;
			int cmp = comparer(list[mid]);
			if (cmp == 0)
			{
				return mid;
			}

			if (mid == first)
			{
				return cmp > 0 ? first : last;
			}

			if (cmp > 0)
			{
				last = mid;
			}
			else
			{
				first = mid;
			}
		}

		return first;
	}
}