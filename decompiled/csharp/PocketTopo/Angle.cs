using System;

namespace PocketTopo;

public struct Angle
{
	public const int UNDEF = int.MinValue;

	public const int A90DEG = 16384;

	public const int A180DEG = 32768;

	public const int A270DEG = 49152;

	public const int A360DEG = 65536;

	internal const int S = 30;

	internal const int N = 1073741824;

	internal int value;

	private static int[] sinTab;

	public Angle(int value)
	{
		this.value = value;
	}

	public Angle(double value)
	{
		this.value = (short)Math.Round(value * (32768.0 / Math.PI));
	}

	public Angle(double value, int unit)
	{
		this.value = (short)Math.Round(value * 65536.0 / (double)unit);
	}

	public static implicit operator Angle(int value)
	{
		return new Angle(value);
	}

	public static implicit operator int(Angle a)
	{
		return a.value;
	}

	public double ToRad()
	{
		return (double)(short)value * 9.587379924285257E-05;
	}

	public double ToUnit(int unit)
	{
		return (double)(short)value * ((double)unit / 65536.0);
	}

	public double ToUnsigned(int unit)
	{
		return (double)(int)(ushort)value * ((double)unit / 65536.0);
	}

	internal static int Sin(Angle angle)
	{
		int num = (short)(int)angle;
		if (num < 0)
		{
			if (num >= -16384)
			{
				return -sinTab[-num];
			}
			return -sinTab[num + 32768];
		}
		if (num >= 16384)
		{
			return sinTab[32768 - num];
		}
		return sinTab[num];
	}

	internal static int Cos(Angle angle)
	{
		int num = (short)(int)angle;
		if (num < 0)
		{
			if (num >= -16384)
			{
				return sinTab[num + 16384];
			}
			return -sinTab[-16384 - num];
		}
		if (num >= 16384)
		{
			return -sinTab[num - 16384];
		}
		return sinTab[16384 - num];
	}

	internal static void ToRect(int len, Angle angle, out int x, out int y)
	{
		int num = (short)(int)angle;
		int num2;
		int num3;
		if (num < 0)
		{
			if (num >= -16384)
			{
				num2 = -sinTab[-num];
				num3 = sinTab[num + 16384];
			}
			else
			{
				num2 = -sinTab[num + 32768];
				num3 = -sinTab[-16384 - num];
			}
		}
		else if (num >= 16384)
		{
			num2 = sinTab[32768 - num];
			num3 = -sinTab[num - 16384];
		}
		else
		{
			num2 = sinTab[num];
			num3 = sinTab[16384 - num];
		}
		x = (int)((long)len * (long)num2 + 536870912 >> 30);
		y = (int)((long)len * (long)num3 + 536870912 >> 30);
	}

	internal static int AngleBisector(Angle a1, Angle a2)
	{
		if ((int)a1 == int.MinValue)
		{
			return a2;
		}
		if ((int)a2 == int.MinValue)
		{
			return a1;
		}
		return (int)a1 + ((short)((int)a2 - (int)a1) >> 1);
	}

	private static bool NearTo(int diff, Angle az1, Angle az2, Angle incl1, Angle incl2)
	{
		if (Math.Abs(diff) > 50)
		{
			return false;
		}
		ToRect(1000, incl1, out var x, out var y);
		ToRect(y, az1, out var x2, out var y2);
		ToRect(1000, incl2, out var x3, out y);
		ToRect(y, az2, out var x4, out var y3);
		x2 -= x4;
		y2 -= y3;
		x -= x3;
		return x2 * x2 + y2 * y2 + x * x <= 900;
	}

	internal static bool NearTo(Station a, Station b)
	{
		return NearTo(a.dist - b.dist, a.azimuth, b.azimuth, a.incl, b.incl);
	}

	internal static bool RevNearTo(Station a, Station b)
	{
		return NearTo(a.dist - b.dist, a.azimuth, b.azimuth + 32768, a.incl, -b.incl);
	}

	public override string ToString()
	{
		return value.ToString();
	}

	static Angle()
	{
		sinTab = new int[16385];
		for (int i = 0; i < 16385; i++)
		{
			sinTab[i] = (int)Math.Round(Math.Sin((double)i * 9.587379924285257E-05) * 1073741824.0);
		}
	}
}
