using System;

namespace PocketTopo;

public struct ID : IComparable
{
	private const int LIMIT = -1048576;

	private const int RESVD = -256;

	private const int UNDEF_VAL = -1;

	public static ID UNDEF = new ID(-1);

	public static ID DEFAULT = new ID(65536);

	private int value;

	private ID(int value)
	{
		this.value = value;
	}

	public ID(int ser, int num)
	{
		value = (ser << 16) + num;
	}

	public ID(string s)
	{
		value = -1;
		s = s.TrimStart();
		if (!(s != ""))
		{
			return;
		}
		try
		{
			int num = s.LastIndexOf('.');
			if (num < 0)
			{
				int num2 = int.Parse(s);
				if (num2 >= 0 && num2 < 1048320)
				{
					value = num2 + -1048576;
				}
				return;
			}
			string s2 = s.Substring(0, num);
			int num3 = ushort.Parse(s.Substring(num + 1));
			try
			{
				int num4 = short.Parse(s2);
				if (num4 >= 0)
				{
					value = (num4 << 16) + num3;
				}
			}
			catch
			{
			}
		}
		catch
		{
		}
	}

	public static bool operator ==(ID a, ID b)
	{
		return a.value == b.value;
	}

	public static bool operator !=(ID a, ID b)
	{
		return a.value != b.value;
	}

	internal ID NextMajor()
	{
		int num = -1;
		if (value >= 0)
		{
			if (value <= 2147418111)
			{
				num = value + 65536;
			}
		}
		else if (value != -1 && value < -257)
		{
			num = value + 1;
		}
		return new ID(num);
	}

	internal static ID NewStart(Survey survey, ID hint)
	{
		ID iD = Config.nextId;
		if (hint.value >= 0 && iD.value >= -1048576 && iD.value < -1015808)
		{
			iD.value = iD.value - -1048576 << 16;
		}
		while (survey.SearchById(iD) != null)
		{
			iD = iD.NextMajor();
		}
		Config.nextId = iD.NextMajor();
		return iD;
	}

	public override string ToString()
	{
		if (value >= 0)
		{
			return (value >> 16) + "." + (value & 0xFFFF);
		}
		if (value < -256)
		{
			return (value - -1048576).ToString();
		}
		return "";
	}

	public override bool Equals(object obj)
	{
		return obj is ID && ((ID)obj).value == value;
	}

	public override int GetHashCode()
	{
		return value;
	}

	public int CompareTo(object obj)
	{
		return value - ((ID)obj).value;
	}

	internal int Series()
	{
		if (value >= 0 || value < -1048576)
		{
			return value >>> 16;
		}
		return -1;
	}

	public string SeriesString()
	{
		if (value >= 0)
		{
			return (value >> 16).ToString();
		}
		return "";
	}

	internal int Station()
	{
		if (value >= 0 || value < -1048576)
		{
			return value & 0xFFFF;
		}
		if (value < -256)
		{
			return value - -1048576;
		}
		return 0;
	}

	public ID Next()
	{
		return new ID(value + 1);
	}

	public ID NextStat(Survey survey)
	{
		ID iD = Next();
		while (survey.SearchById(iD) != null)
		{
			iD = iD.Next();
		}
		return iD;
	}

	internal bool NextTo(ID id)
	{
		return Math.Abs(id.value - value) == 1;
	}

	internal static int Dist(ID a, ID b)
	{
		return a.value - b.value;
	}

	internal void Adjust(int inc)
	{
		value += inc;
	}

	internal void PrepWrite(FileWriter w)
	{
	}

	internal void Write(FileWriter w)
	{
		int num = value;
		if (num == -1)
		{
			num = int.MinValue;
		}
		else if (num < 0)
		{
			num -= 2146435071;
		}
		w.w.Write(num);
	}

	internal static ID Read(FileReader r)
	{
		int num = r.r.ReadInt32();
		if (num == int.MinValue)
		{
			num = -1;
		}
		else if (num < 0)
		{
			num += 2146435071;
		}
		return new ID(num);
	}
}
