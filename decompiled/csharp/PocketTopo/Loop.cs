using System;

namespace PocketTopo;

internal class Loop
{
	private const int IT_LIMIT = 10;

	private Station st1;

	private Station st2;

	private Station end1;

	private Station end2;

	private int wSum;

	internal Loop next;

	internal Loop(Station e1, Station e2)
	{
		end1 = e1;
		end2 = e2;
	}

	internal void Setup(bool all)
	{
		Station start = end1;
		Station start2 = end2;
		while (true)
		{
			bool flag = true;
			bool flag2 = false;
			if (!(start is Reference) && (all || !start.readOnly))
			{
				if ((start.flags & Station.Flags.mark) != 0)
				{
					st1 = (st2 = start);
					break;
				}
				start.flags |= Station.Flags.mark;
				start = start.start;
				flag2 = true;
			}
			if (!(start2 is Reference) && (all || !start2.readOnly))
			{
				if ((start2.flags & Station.Flags.mark) != 0)
				{
					st1 = (st2 = start2);
					break;
				}
				start2.flags |= Station.Flags.mark;
				start2 = start2.start;
				flag2 = true;
			}
			if (!flag2)
			{
				st1 = start;
				st2 = start2;
				break;
			}
		}
		wSum = 0;
		for (start = end1; start != st1; start = start.start)
		{
			wSum += start.dist;
			start.flags &= (Station.Flags)223;
		}
		for (start2 = end2; start2 != st2; start2 = start2.start)
		{
			wSum += start2.dist;
			start2.flags &= (Station.Flags)223;
		}
		while ((start.flags & Station.Flags.mark) != 0)
		{
			start.flags &= (Station.Flags)223;
			start = start.start;
		}
	}

	private int Close()
	{
		if (wSum == 0)
		{
			return 0;
		}
		long num = 0L;
		long num2 = 0L;
		long num3 = 0L;
		for (Station start = end1; start != st1; start = start.start)
		{
			num += start.ex;
			num2 += start.ey;
			num3 += start.eh;
		}
		for (Station start = end2; start != st2; start = start.start)
		{
			num -= start.ex;
			num2 -= start.ey;
			num3 -= start.eh;
		}
		num += (long)(end1.x - end2.x) << 30;
		num2 += (long)(end1.y - end2.y) << 30;
		num3 += (long)(end1.h - end2.h) << 30;
		long num4 = num / wSum;
		long num5 = num2 / wSum;
		long num6 = num3 / wSum;
		for (Station start = end1; start != st1; start = start.start)
		{
			start.ex -= num4 * start.dist;
			start.ey -= num5 * start.dist;
			start.eh -= num6 * start.dist;
		}
		for (Station start = end2; start != st2; start = start.start)
		{
			start.ex += num4 * start.dist;
			start.ey += num5 * start.dist;
			start.eh += num6 * start.dist;
		}
		return (int)(Math.Max(Math.Max(Math.Abs(num), Math.Abs(num2)), Math.Abs(num3)) >> 30);
	}

	internal static void CloseLoops(Loop loops1, Loop loops2, bool all)
	{
		for (Loop loop = loops1; loop != null; loop = loop.next)
		{
			loop.Setup(all);
		}
		for (Loop loop = loops2; loop != null; loop = loop.next)
		{
			loop.Setup(all);
		}
		int num;
		do
		{
			num = 0;
			for (Loop loop = loops1; loop != null; loop = loop.next)
			{
				int num2 = loop.Close();
				if (num2 > num)
				{
					num = num2;
				}
			}
			for (Loop loop = loops2; loop != null; loop = loop.next)
			{
				int num2 = loop.Close();
				if (num2 > num)
				{
					num = num2;
				}
			}
		}
		while (num > 10);
	}
}
