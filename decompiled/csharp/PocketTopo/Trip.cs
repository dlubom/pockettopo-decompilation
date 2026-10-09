using System;
using System.Collections;
using System.IO;

namespace PocketTopo;

internal class Trip
{
	private DateTime date;

	private string comment;

	private Angle declCorr;

	private Angle decl;

	private bool automatic;

	private static ArrayList list;

	private static int boundary;

	public DateTime Date
	{
		get
		{
			return date;
		}
		set
		{
			date = value;
		}
	}

	public string Comment
	{
		get
		{
			return (comment != null) ? comment : "";
		}
		set
		{
			comment = ((value != "") ? value : null);
		}
	}

	public Angle DeclCorr
	{
		get
		{
			return declCorr;
		}
		set
		{
			declCorr = value;
		}
	}

	public bool AutoDecl
	{
		get
		{
			return automatic;
		}
		set
		{
			automatic = value;
		}
	}

	public Angle Declination => decl;

	internal static Angle GetDeclination(bool auto, Angle decl, DateTime date, Reference rf)
	{
		return decl;
	}

	internal bool Change(DateTime date, bool auto, Angle decl, string comment)
	{
		if (!Equals(date, auto, decl, comment))
		{
			this.date = date;
			declCorr = decl;
			automatic = auto;
			this.comment = comment;
			return true;
		}
		return false;
	}

	internal bool Equals(Trip t)
	{
		return date == t.date && automatic == t.automatic && (int)declCorr == (int)t.declCorr && Comment == t.Comment;
	}

	internal bool Equals(DateTime date, bool auto, Angle decl, string comment)
	{
		return this.date == date && automatic == auto && (int)declCorr == (int)decl && Comment == comment;
	}

	internal static void Reset()
	{
		list = new ArrayList();
		boundary = 0;
	}

	private void SetDecl(Reference rf)
	{
		decl = GetDeclination(automatic, declCorr, date, rf);
	}

	internal static void Evaluate(Reference rf)
	{
		for (int i = 0; i < list.Count; i++)
		{
			((Trip)list[i]).SetDecl(rf);
		}
	}

	internal static void SetBoundary()
	{
		boundary = list.Count;
	}

	internal short Idx()
	{
		return (short)list.IndexOf(this);
	}

	internal static Trip ByIndex(int i)
	{
		if (i >= 0 && i < list.Count)
		{
			return (Trip)list[i];
		}
		return null;
	}

	internal static Angle DeclCorrByIndex(int i)
	{
		if (i >= 0 && i < list.Count)
		{
			return ((Trip)list[i]).Declination;
		}
		return 0;
	}

	internal static short NewIdx()
	{
		Trip trip = new Trip();
		trip.date = Config.tripDate;
		if ((int)Config.declCorr == -32768)
		{
			trip.declCorr = 0;
			trip.automatic = true;
		}
		else
		{
			trip.declCorr = Config.declCorr;
			trip.automatic = false;
		}
		trip.Comment = Config.tripComment;
		int num = list.Count - 1;
		if (num >= boundary && trip.Equals((Trip)list[num]))
		{
			return (short)num;
		}
		return (short)list.Add(trip);
	}

	internal static int GetDefault()
	{
		Trip trip = new Trip();
		trip.date = new DateTime(2008, 1, 1);
		return list.Add(trip);
	}

	internal static int WriteList(FileWriter w)
	{
		w.w.Write(list.Count - boundary);
		for (int i = boundary; i < list.Count; i++)
		{
			((Trip)list[i]).Write(w);
		}
		return boundary;
	}

	internal static int ReadList(FileReader r)
	{
		int count = list.Count;
		int num = r.r.ReadInt32();
		for (int i = 0; i < num; i++)
		{
			Trip trip = new Trip();
			trip.Read(r);
			list.Add(trip);
		}
		return count;
	}

	internal static int WriteText(TextWriter w)
	{
		for (int i = boundary; i < list.Count; i++)
		{
			((Trip)list[i]).WriteTxt(w, i - boundary);
		}
		return boundary;
	}

	internal static int WriteTherion(TextWriter w, int i)
	{
		i += boundary;
		if (i < list.Count)
		{
			((Trip)list[i]).WriteThe(w);
			return i;
		}
		return -1;
	}

	private void Write(FileWriter w)
	{
		w.w.Write(date.Ticks);
		w.w.Write(Comment);
		w.w.Write(automatic ? short.MinValue : ((short)(int)declCorr));
	}

	private void Read(FileReader r)
	{
		date = new DateTime(r.r.ReadInt64());
		Comment = r.r.ReadString();
		declCorr = r.r.ReadInt16();
		if ((int)declCorr == -32768)
		{
			automatic = true;
			declCorr = 0;
		}
		else
		{
			automatic = false;
		}
	}

	private void WriteTxt(TextWriter w, int idx)
	{
		w.Write("[{0:d}]: ", idx + 1);
		w.Write("{0:d4}/{1:d2}/{2:d2} ", new object[3] { date.Year, date.Month, date.Day });
		if (automatic)
		{
			w.Write("    Auto:");
		}
		Station.WriteAngle(w, declCorr, 0);
		w.Write("  ");
		if (comment != null)
		{
			Station.WriteComment(w, comment);
		}
		w.WriteLine();
	}

	private void WriteThe(TextWriter w)
	{
		w.WriteLine("TRIP");
		w.Write("DATE ");
		w.WriteLine("{0:d4}-{1:d2}-{2:d2} ", new object[3] { date.Year, date.Month, date.Day });
		w.Write("DECLINATION ");
		Station.WriteAngle(w, Declination, 360);
		w.WriteLine();
	}

	internal static int AddTrip(Trip t)
	{
		return list.Add(t);
	}

	internal static string GetAng(Angle val)
	{
		double num = val.ToUnit(Config.fullCircle);
		string text = num.ToString("f2");
		Angle val2 = val;
		SetAng(ref val2, text);
		if ((int)val2 != (int)val)
		{
			text = num.ToString("f3");
		}
		return text;
	}

	internal static void SetAng(ref Angle val, string s)
	{
		try
		{
			val = new Angle(float.Parse(s), Config.fullCircle);
		}
		catch
		{
		}
	}
}
