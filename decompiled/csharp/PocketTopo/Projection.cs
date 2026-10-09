using System;

namespace PocketTopo;

internal class Projection
{
	private const bool newProjection = true;

	internal static int SideProjection(Angle a1, Angle a2, int len, Angle a, bool flip1, bool flip2)
	{
		int num;
		if ((int)a1 == int.MinValue)
		{
			if ((int)a2 == int.MinValue)
			{
				num = 0;
			}
			else
			{
				num = (int)((long)len * (long)Angle.Cos((int)a - (int)a2) >> 30);
				if (flip2)
				{
					num = -num;
				}
			}
		}
		else if ((int)a2 == int.MinValue)
		{
			num = (int)((long)len * (long)Angle.Cos((int)a - (int)a1) >> 30);
			if (flip1)
			{
				num = -num;
			}
		}
		else
		{
			int num2 = (short)((int)a2 - (int)a1);
			int num3 = Math.Abs(num2);
			int num4 = (short)((int)a - (int)a1 + 32768);
			int num5 = (short)((int)a - (int)a2);
			if (flip1 != flip2)
			{
				if (Math.Abs(num4) < Math.Abs(num5))
				{
					num = (int)((long)len * (long)Angle.Cos((int)a - (int)a1) >> 30);
					if (flip1)
					{
						num = -num;
					}
				}
				else
				{
					num = (int)((long)len * (long)Angle.Cos((int)a - (int)a2) >> 30);
					if (flip2)
					{
						num = -num;
					}
				}
			}
			else
			{
				int num6 = num5;
				if (Math.Abs(num4) < Math.Abs(num5))
				{
					len = -len;
					num6 = -num4;
				}
				if (num2 < 0)
				{
					num6 = -num6;
				}
				int num7 = 16384 - (num3 >> 1);
				int num8 = num7 >> 1;
				bool flag = false;
				if (num8 < 4096)
				{
					num8 = 4096;
				}
				if (num6 < 0 && num7 + num8 < 16384)
				{
					num8 = 16384 - num7;
				}
				num = (int)((Math.Abs(num6) <= num8) ? ((long)len * (long)Angle.Cos(num6) >> 30) : ((num6 < 0) ? ((long)len * (long)Angle.Cos(num8) / Angle.Sin(num7 + num8) * Angle.Sin(num7 - num6) >> 30) : ((long)len * (long)Angle.Cos(num8) / Angle.Sin(num7 - num8) * Angle.Sin(num7 - num6) >> 30)));
				if (flip1)
				{
					num = -num;
				}
			}
		}
		return num;
	}
}
