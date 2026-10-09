using System;

namespace PocketTopo;

public class MetricGrid : Grid
{
	public override string ToString(MetricLocation loc)
	{
		string text = ((double)loc.e / 1000.0).ToString("f1");
		text += " / ";
		text += ((double)loc.n / 1000.0).ToString("f1");
		text += " / ";
		return text + ((double)loc.alt / 1000.0).ToString("f1");
	}

	public override string GetE(MetricLocation loc)
	{
		if (loc.e == long.MinValue)
		{
			return "";
		}
		return ((double)loc.e / 1000.0).ToString("f2");
	}

	public override string GetN(MetricLocation loc)
	{
		if (loc.n == long.MinValue)
		{
			return "";
		}
		return ((double)loc.n / 1000.0).ToString("f2");
	}

	public override string GetH(MetricLocation loc)
	{
		if (loc.alt == int.MinValue)
		{
			return "";
		}
		return ((double)loc.alt / 1000.0).ToString("f2");
	}

	public override bool PutE(ref MetricLocation loc, string s)
	{
		if (s.TrimStart() == "")
		{
			loc.e = long.MinValue;
			return true;
		}
		try
		{
			loc.e = (long)Math.Round(double.Parse(s) * 1000.0);
			return true;
		}
		catch
		{
			return false;
		}
	}

	public override bool PutN(ref MetricLocation loc, string s)
	{
		if (s.TrimStart() == "")
		{
			loc.n = long.MinValue;
			return true;
		}
		try
		{
			loc.n = (long)Math.Round(double.Parse(s) * 1000.0);
			return true;
		}
		catch
		{
			return false;
		}
	}

	public override bool PutH(ref MetricLocation loc, string s)
	{
		if (s.TrimStart() == "")
		{
			loc.alt = int.MinValue;
			return true;
		}
		try
		{
			loc.alt = (int)Math.Round(double.Parse(s) * 1000.0);
			return true;
		}
		catch
		{
			return false;
		}
	}
}
