using System.IO;

namespace PocketTopo;

internal class Reference : Station
{
	internal static MetricGrid grid = new MetricGrid();

	internal MetricLocation loc;

	public string Id
	{
		get
		{
			return num.ToString();
		}
		set
		{
			Station.survey.Change(Station.survey.references, Station.survey.references.Position, 0, value);
		}
	}

	public string E
	{
		get
		{
			return grid.GetE(loc);
		}
		set
		{
			Station.survey.Change(Station.survey.references, Station.survey.references.Position, 1, value);
		}
	}

	public string N
	{
		get
		{
			return grid.GetN(loc);
		}
		set
		{
			Station.survey.Change(Station.survey.references, Station.survey.references.Position, 2, value);
		}
	}

	public string Alt
	{
		get
		{
			return grid.GetH(loc);
		}
		set
		{
			Station.survey.Change(Station.survey.references, Station.survey.references.Position, 3, value);
		}
	}

	internal Reference(ID id, MetricLocation loc)
	{
		num = id;
		this.loc = loc;
		trip = -1;
		comment = null;
	}

	internal override void Set(int col, string val)
	{
		switch (col)
		{
		case 0:
			Station.SetNum(ref num, val);
			break;
		case 1:
			grid.PutE(ref loc, val);
			break;
		case 2:
			grid.PutN(ref loc, val);
			break;
		case 3:
			grid.PutH(ref loc, val);
			break;
		default:
			comment = ((val != "") ? val : null);
			break;
		}
	}

	internal override string Get(int col)
	{
		return col switch
		{
			0 => Id, 
			1 => E, 
			2 => N, 
			3 => Alt, 
			_ => (comment == null) ? "" : comment, 
		};
	}

	internal void Write(FileWriter w)
	{
		num.Write(w);
		w.w.Write(loc.e);
		w.w.Write(loc.n);
		w.w.Write(loc.alt);
		w.w.Write((comment != null) ? comment : "");
	}

	internal void Read(FileReader r)
	{
		num = ID.Read(r);
		loc.e = r.r.ReadInt64();
		loc.n = r.r.ReadInt64();
		loc.alt = r.r.ReadInt32();
		string text = r.r.ReadString();
		comment = ((text != "") ? text : null);
	}

	internal void WriteText(TextWriter w)
	{
		Station.WriteNum(w, num);
		w.Write(grid.GetE(loc).PadLeft(14));
		w.Write(grid.GetN(loc).PadLeft(14));
		w.Write(grid.GetH(loc).PadLeft(10));
		w.Write("    ");
		if (comment != null)
		{
			Station.WriteComment(w, comment);
		}
		w.WriteLine();
	}

	internal void WriteThRef(TextWriter w)
	{
		w.Write(num.ToString());
		w.Write("\t");
		w.Write(string.Format(Exporter.inv, "{0:f3}\t", (double)loc.e / 1000.0));
		w.Write(string.Format(Exporter.inv, "{0:f3}\t", (double)loc.n / 1000.0));
		w.Write(string.Format(Exporter.inv, "{0:f3}", (double)loc.alt / 1000.0));
		w.WriteLine();
	}
}
