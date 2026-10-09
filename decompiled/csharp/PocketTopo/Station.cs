using System;
using System.Drawing;
using System.IO;

namespace PocketTopo;

public class Station : IComparable
{
	internal enum Flags : byte
	{
		flipped = 1,
		hasComment = 2,
		invisible = 4,
		special = 8,
		projected = 0x10,
		mark = 0x20,
		readOnly = 0x80
	}

	internal static bool error;

	internal static Survey survey;

	internal ID from;

	internal ID to;

	internal int dist;

	internal short azimuth;

	internal short incl;

	internal byte roll;

	internal Flags flags;

	internal short trip;

	internal string comment;

	internal ID num;

	internal int x;

	internal int y;

	internal int h;

	internal int d;

	internal int idx;

	internal Station start;

	internal int dx;

	internal int dy;

	internal int dh;

	internal int dd;

	internal long ex;

	internal long ey;

	internal long eh;

	internal Station next;

	internal Station cross;

	internal Rectangle olBox;

	internal Rectangle svBox;

	public string From
	{
		get
		{
			return from.ToString();
		}
		set
		{
			survey.Change(survey.database, survey.database.Position, 0, value);
		}
	}

	public string To
	{
		get
		{
			return to.ToString();
		}
		set
		{
			survey.Change(survey.database, survey.database.Position, 1, value);
		}
	}

	public string Dist
	{
		get
		{
			return GetDist(dist, Config.unit);
		}
		set
		{
			survey.Change(survey.database, survey.database.Position, 2, value);
		}
	}

	public string Azimuth
	{
		get
		{
			return GetAngU(azimuth);
		}
		set
		{
			survey.Change(survey.database, survey.database.Position, 3, value);
		}
	}

	public string Incl
	{
		get
		{
			return GetAng(incl);
		}
		set
		{
			survey.Change(survey.database, survey.database.Position, 4, value);
		}
	}

	public string Comment
	{
		get
		{
			if (comment != null)
			{
				return readOnly ? "+" : "*";
			}
			return readOnly ? "-" : "";
		}
	}

	internal bool flipped
	{
		get
		{
			return (flags & Flags.flipped) != 0;
		}
		set
		{
			if (value)
			{
				flags |= Flags.flipped;
			}
			else
			{
				flags &= (Flags)254;
			}
		}
	}

	internal bool projected
	{
		get
		{
			return (flags & Flags.projected) != 0;
		}
		set
		{
			if (value)
			{
				flags |= Flags.projected;
			}
			else
			{
				flags &= (Flags)239;
			}
		}
	}

	internal bool invisible
	{
		get
		{
			return (flags & Flags.invisible) != 0;
		}
		set
		{
			if (value)
			{
				flags |= Flags.invisible;
			}
			else
			{
				flags &= (Flags)251;
			}
		}
	}

	internal bool special
	{
		get
		{
			return (flags & Flags.special) != 0;
		}
		set
		{
			if (value)
			{
				flags |= Flags.special;
			}
			else
			{
				flags &= (Flags)247;
			}
		}
	}

	internal bool readOnly
	{
		get
		{
			return (flags & Flags.readOnly) != 0;
		}
		set
		{
			if (value)
			{
				flags |= Flags.readOnly;
			}
			else
			{
				flags &= (Flags)127;
			}
		}
	}

	internal Angle FwAzimuth
	{
		get
		{
			if (dist == 0)
			{
				return int.MinValue;
			}
			if (num == from)
			{
				return azimuth + 32768;
			}
			return azimuth;
		}
	}

	internal Angle FwIncl
	{
		get
		{
			if (dist == 0)
			{
				return int.MinValue;
			}
			if (num == from)
			{
				return -incl;
			}
			return incl;
		}
	}

	public Station()
	{
		from = ID.UNDEF;
		to = ID.UNDEF;
	}

	internal Station(ID f, ID t, short tr)
	{
		from = f;
		to = t;
		trip = tr;
	}

	internal Station(ID f, ID t, int l, Angle d, Angle i, byte r, short tr)
	{
		if ((int)i > 16384 || (int)i < -16384)
		{
			i = 32768 - (int)i;
			d = (int)d + 32768;
		}
		from = f;
		to = t;
		dist = l;
		azimuth = (short)(int)d;
		incl = (short)(int)i;
		roll = r;
		comment = null;
		flags = (Flags)0;
		trip = tr;
	}

	internal static void SetNum(ref ID val, string s)
	{
		if (s.TrimStart() == "")
		{
			val = ID.UNDEF;
			return;
		}
		ID iD = new ID(s);
		if (iD != ID.UNDEF)
		{
			val = iD;
		}
		else
		{
			error = true;
		}
	}

	private static void SetDist(ref int val, string s, double unit)
	{
		if (s.TrimStart() == "")
		{
			val = int.MinValue;
			return;
		}
		try
		{
			val = (int)Math.Round((double)float.Parse(s) * unit);
		}
		catch
		{
			error = true;
		}
	}

	private static void SetAng(ref short val, string s)
	{
		try
		{
			val = (short)(int)new Angle(float.Parse(s), Config.fullCircle);
		}
		catch
		{
			error = true;
		}
	}

	private static string GetDist(int val, double unit)
	{
		if (val == int.MinValue)
		{
			return "";
		}
		return ((double)val / unit).ToString((unit < 1000.0) ? "f2" : "f3");
	}

	private static string GetAng(Angle val)
	{
		return val.ToUnit(Config.fullCircle).ToString("f1");
	}

	private static string GetAngU(Angle val)
	{
		return val.ToUnsigned(Config.fullCircle).ToString("f1");
	}

	internal virtual void Set(int col, string val)
	{
		switch (col)
		{
		case 0:
			SetNum(ref from, val);
			break;
		case 1:
			SetNum(ref to, val);
			break;
		case 2:
			SetDist(ref dist, val, Config.unit);
			break;
		case 3:
			SetAng(ref azimuth, val);
			break;
		case 4:
			SetAng(ref incl, val);
			break;
		default:
			comment = ((val != "") ? val : null);
			break;
		}
	}

	internal virtual string Get(int col)
	{
		return col switch
		{
			0 => From, 
			1 => To, 
			2 => Dist, 
			3 => Azimuth, 
			4 => Incl, 
			_ => (comment == null) ? "" : comment, 
		};
	}

	internal Angle RealAzimuth()
	{
		return azimuth + (int)Trip.DeclCorrByIndex(trip);
	}

	internal ID ActualId()
	{
		if (to != ID.UNDEF && (dist == 0 || ID.Dist(to, from) >= 0))
		{
			return to;
		}
		return from;
	}

	internal void SetupNew(Station prev, Station third, Station fourth)
	{
		from = prev.ActualId();
		if (prev.flipped)
		{
			flags |= Flags.flipped;
		}
		if (prev.projected)
		{
			flags |= Flags.projected;
		}
		if (Config.smartMode && to == ID.UNDEF && prev.to == ID.UNDEF && from == prev.from && third != null && from == third.from && third.to == ID.UNDEF && Angle.NearTo(this, prev) && Angle.NearTo(this, third) && Angle.NearTo(prev, third))
		{
			if (Config.reverse)
			{
				to = from;
				from = to.NextStat(survey);
			}
			else if (fourth != null && fourth.to == from && Angle.RevNearTo(this, fourth) && Angle.RevNearTo(prev, fourth) && Angle.RevNearTo(third, fourth))
			{
				to = fourth.from;
			}
			else
			{
				to = from.NextStat(survey);
			}
			prev.from = (third.from = from);
			prev.to = (third.to = to);
		}
	}

	internal bool Renumber(ID oldID, ID newID)
	{
		if (from != ID.UNDEF && dist > 0)
		{
			if (to == ID.UNDEF)
			{
				if ((oldID == ID.UNDEF || from == oldID) && from != newID)
				{
					from = newID;
					return true;
				}
			}
			else if (to == from.Next())
			{
				if ((oldID == ID.UNDEF || from == oldID) && from != newID)
				{
					from = newID;
					to = newID.Next();
					return true;
				}
			}
			else if (to.Next() == from && (oldID == ID.UNDEF || to == oldID) && to != newID)
			{
				to = newID;
				from = newID.Next();
				return true;
			}
		}
		return false;
	}

	internal bool ToBack()
	{
		return (to == ID.UNDEF && Config.reverse) || ID.Dist(to, from) > 0;
	}

	internal bool TurnToBranch(Station last)
	{
		if (from != ID.UNDEF)
		{
			if (to == ID.UNDEF)
			{
				if (Config.reverse)
				{
					to = from;
					if (last != null && last.to == to)
					{
						from = last.from;
					}
					else
					{
						from = to.NextStat(survey);
					}
				}
				else if (last != null && last.from == from)
				{
					to = last.to;
				}
				else
				{
					to = from.NextStat(survey);
				}
				return true;
			}
			ID iD = to;
			to = from;
			from = iD;
			return true;
		}
		return false;
	}

	public int CompareTo(object obj)
	{
		return num.CompareTo(((Station)obj).num);
	}

	internal void SetBox()
	{
		if (start != null)
		{
			olBox = Rectangle.FromLTRB(Math.Min(x, start.x), Math.Min(y, start.y), Math.Max(x, start.x), Math.Max(y, start.y));
			svBox = Rectangle.FromLTRB(Math.Min(d, start.d), Math.Min(h, start.h), Math.Max(d, start.d), Math.Max(h, start.h));
		}
		else
		{
			olBox = Rectangle.FromLTRB(x, y, x + 1, y + 1);
			svBox = Rectangle.FromLTRB(d, h, d + 1, h + 1);
		}
	}

	internal void PaintOutline(Mapping m, Graphics g, Pen pen, int showStat)
	{
		m.World2Screen(x, y, out var sx, out var sy);
		if (start != null && (flags & Flags.invisible) == 0)
		{
			m.World2Screen(start.x, start.y, out var sx2, out var sy2);
			if ((flags & Flags.special) != 0 && pen == MainForm.black)
			{
				pen = MainForm.blue;
			}
			if (idx == survey.selectedRow)
			{
				pen = MainForm.selected;
			}
			if (to == ID.UNDEF && Math.Abs((int)incl) > 8192)
			{
				if (idx == survey.selectedRow)
				{
					pen = MainForm.selectedDa;
				}
				else
				{
					if (pen != MainForm.shotXs)
					{
						return;
					}
					pen = MainForm.shotDa;
				}
			}
			g.DrawLine(pen, sx, sy, sx2, sy2);
		}
		if (showStat > 0 && m.BigScale)
		{
			PaintCircle(g, pen, sx, sy);
			if (showStat >= 2 && dist > 0)
			{
				g.DrawString(num.ToString(), MainForm.labelFont, MainForm.blackBrush, sx, sy);
			}
		}
	}

	internal void PaintSideview(Mapping m, Graphics g, Pen pen, int showStat)
	{
		m.World2Screen(d, h, out var sx, out var sy);
		if (start != null && (flags & Flags.invisible) == 0)
		{
			m.World2Screen(start.d, start.h, out var sx2, out var sy2);
			if (idx == survey.selectedRow)
			{
				pen = MainForm.selected;
			}
			int num = d - start.d;
			if (to == ID.UNDEF && dist * (dist >> 4) > num * (num >> 3) + dh * (dh >> 3))
			{
				pen = MainForm.shotDa;
				if (idx == survey.selectedRow)
				{
					pen = MainForm.selectedDa;
				}
			}
			g.DrawLine(pen, sx, sy, sx2, sy2);
		}
		if (showStat > 0 && m.BigScale)
		{
			PaintCircle(g, pen, sx, sy);
			if (showStat >= 2 && dist > 0)
			{
				g.DrawString(this.num.ToString(), MainForm.labelFont, MainForm.blackBrush, sx, sy);
			}
		}
	}

	private void PaintHXSection(Graphics g, Pen pen, int x0, int y0, int factor)
	{
		if (Math.Abs((int)incl) > 8192)
		{
			pen = MainForm.shotDa;
		}
		g.DrawLine(pen, x0, y0, x0 + dx / factor, y0 - dy / factor);
	}

	private void PaintVXSection(Graphics g, Pen pen, int x0, int y0, int direction, int factor)
	{
		Angle.ToRect(dd, azimuth - direction, out var num, out var value);
		if (Math.Abs(value) * 7 > dist * 5)
		{
			pen = MainForm.shotDa;
		}
		g.DrawLine(pen, x0, y0, x0 + num / factor, y0 - dh / factor);
	}

	internal void PaintXSection(Mapping m, Graphics g, Pen pen, int x, int y, int direction)
	{
		m.AltWorld2Screen(x, y, out var sx, out var sy);
		if (m.BigScale)
		{
			PaintCircle(g, pen, sx, sy);
			if (Config.showSketchId && dist > 0)
			{
				g.DrawString(num.ToString(), MainForm.labelFont, MainForm.blackBrush, sx, sy);
			}
		}
		Drawing.SetupPen(pen);
		int factor = m.ScaleFactor();
		if (direction == -1)
		{
			for (Station station = cross; station != null; station = station.next)
			{
				station.PaintHXSection(g, pen, sx, sy, factor);
			}
		}
		else
		{
			for (Station station = cross; station != null; station = station.next)
			{
				station.PaintVXSection(g, pen, sx, sy, direction, factor);
			}
		}
	}

	internal static void PaintCircle(Graphics g, Pen pen, int x, int y)
	{
		if (Config.thickLines)
		{
			g.DrawEllipse(pen, x - 4, y - 4, 8, 8);
		}
		else
		{
			g.DrawEllipse(pen, x - 2, y - 2, 4, 4);
		}
	}

	internal bool OutlineVisible(Rectangle clip)
	{
		return olBox.IntersectsWith(clip);
	}

	internal bool SideviewVisible(Rectangle clip)
	{
		return svBox.IntersectsWith(clip);
	}

	internal void ResetError()
	{
		ex = (ey = (eh = 0L));
	}

	public override string ToString()
	{
		return string.Concat(from, "->", to, ":", dist, ",", azimuth, ",", incl, "(", idx, ")");
	}

	internal void Write(FileWriter w, int tripOffs)
	{
		from.Write(w);
		to.Write(w);
		w.w.Write(dist);
		w.w.Write(azimuth);
		w.w.Write(incl);
		Flags flags = this.flags;
		flags = ((comment == null) ? (flags & (Flags)253) : (flags | Flags.hasComment));
		w.w.Write((byte)flags);
		w.w.Write(roll);
		short num = trip;
		if (num >= 0)
		{
			num -= (short)tripOffs;
		}
		w.w.Write(num);
		if (comment != null)
		{
			w.w.Write(comment);
		}
	}

	internal void Read(FileReader r, int tripOffs)
	{
		from = ID.Read(r);
		to = ID.Read(r);
		dist = r.r.ReadInt32();
		azimuth = r.r.ReadInt16();
		incl = r.r.ReadInt16();
		if (r.version >= 2)
		{
			flags = (Flags)r.r.ReadByte();
			roll = r.r.ReadByte();
			trip = r.r.ReadInt16();
			if ((flags & Flags.hasComment) != 0)
			{
				comment = r.r.ReadString();
			}
		}
		else
		{
			flags = (Flags)0;
			roll = 0;
			trip = 0;
		}
		if (trip >= 0)
		{
			trip += (short)tripOffs;
		}
	}

	internal void GetAvDirection(out float dist, out float azimuth, out float incl, bool rev)
	{
		int num = dx;
		int num2 = dy;
		int num3 = dh;
		dist = (float)Math.Sqrt((long)num * (long)num + (long)num2 * (long)num2 + (long)num3 * (long)num3);
		if (dist > 0f)
		{
			if (rev)
			{
				num = -num;
				num2 = -num2;
				num3 = -num3;
			}
			azimuth = (float)Math.Atan2(num, num2);
			azimuth -= (float)Trip.DeclCorrByIndex(trip).ToRad();
			if (azimuth < 0f)
			{
				azimuth += (float)Math.PI * 2f;
			}
			incl = (float)Math.Atan2(num3, Math.Sqrt((long)num * (long)num + (long)num2 * (long)num2));
		}
		else
		{
			azimuth = (incl = 0f);
		}
	}

	internal static void WriteNum(TextWriter w, ID val)
	{
		string text = val.ToString();
		int num = text.IndexOf(".");
		if (num > 0)
		{
			text = text.PadRight(num + 6);
		}
		w.Write(text.PadLeft(12));
	}

	internal static void WriteDist(TextWriter w, int val, double unit)
	{
		if (val == int.MinValue)
		{
			w.Write("        ");
			return;
		}
		w.Write(string.Format(Exporter.inv, (unit < 1000.0) ? "{0,8:f2}" : "{0,8:f3}", (double)val / unit));
	}

	internal static void WriteAngle(TextWriter w, Angle val, int unit)
	{
		if (unit == 0)
		{
			unit = Config.fullCircle;
		}
		w.Write(string.Format(Exporter.inv, "{0,8:f2}", val.ToUnit(unit)));
	}

	internal static void WriteAngleU(TextWriter w, Angle val, int unit)
	{
		if (unit == 0)
		{
			unit = Config.fullCircle;
		}
		w.Write(string.Format(Exporter.inv, "{0,8:f2}", val.ToUnsigned(unit)));
	}

	internal static void WriteComment(TextWriter w, string str)
	{
		w.Write('"');
		str = str.Replace("\"", "\\\"");
		str = str.Replace("\\", "\\\\");
		str = str.Replace("\r", "\\r");
		str = str.Replace("\n", "");
		w.Write(str);
		w.Write('"');
	}

	internal void WriteText(TextWriter w, int tripOffs)
	{
		WriteNum(w, from);
		WriteNum(w, to);
		WriteDist(w, dist, Config.unit);
		WriteAngleU(w, azimuth, 0);
		WriteAngle(w, incl, 0);
		if (trip >= 0)
		{
			w.Write("  [{0:d}]  ", trip - tripOffs + 1);
		}
		else
		{
			w.Write("       ");
		}
		if (invisible)
		{
			w.Write("~ ");
		}
		else if (projected)
		{
			w.Write("= ");
		}
		else if (flipped)
		{
			w.Write("< ");
		}
		else if (special)
		{
			w.Write(". ");
		}
		else
		{
			w.Write("  ");
		}
		if (comment != null)
		{
			WriteComment(w, comment);
		}
		w.WriteLine();
	}

	internal void WriteTherion(TextWriter w)
	{
		if (to != ID.UNDEF || dist != 0)
		{
			w.Write(from.ToString());
			w.Write("\t");
			w.Write(to.ToString());
			w.Write("\t");
			GetAvDirection(out var num, out var num2, out var num3, to != ID.UNDEF && this.num == from);
			w.Write(string.Format(Exporter.inv, "{0:f2}\t", (double)(num2 * 360f) / (Math.PI * 2.0)));
			w.Write(string.Format(Exporter.inv, "{0:f2}\t", (double)(num3 * 360f) / (Math.PI * 2.0)));
			w.Write(string.Format(Exporter.inv, "{0:f3}\t", (double)num / 1000.0));
			w.Write(flipped ? "<" : ">");
			w.WriteLine();
		}
	}

	internal void WriteThShot(TextWriter w, bool side)
	{
		if (start != null)
		{
			w.Write(string.Format(Exporter.inv, "{0:f3}\t", (double)(side ? start.d : start.x) / 1000.0));
			w.Write(string.Format(Exporter.inv, "{0:f3}\t", (double)(-(side ? start.h : start.y)) / 1000.0));
			w.Write(string.Format(Exporter.inv, "{0:f3}\t", (double)(side ? d : x) / 1000.0));
			w.WriteLine(string.Format(Exporter.inv, "{0:f3}", (double)(-(side ? h : y)) / 1000.0));
		}
	}

	internal void WriteThStation(TextWriter w, bool side)
	{
		bool flag = false;
		w.Write(string.Format(Exporter.inv, "{0:f3}\t", (double)(side ? d : x) / 1000.0));
		w.Write(string.Format(Exporter.inv, "{0:f3}\t", (double)(-(side ? h : y)) / 1000.0));
		w.Write(num.ToString());
		w.WriteLine();
	}

	internal void WriteThXSect(TextWriter w, int x, int y, int direction)
	{
		if (direction == -1)
		{
			for (Station station = cross; station != null; station = station.next)
			{
				w.Write(string.Format(Exporter.inv, "{0:f3}\t", (double)x / 1000.0));
				w.Write(string.Format(Exporter.inv, "{0:f3}\t", (double)(-y) / 1000.0));
				w.Write(string.Format(Exporter.inv, "{0:f3}\t", (double)(x + station.dx) / 1000.0));
				w.WriteLine(string.Format(Exporter.inv, "{0:f3}\t", (double)(-(y - station.dy)) / 1000.0));
			}
			return;
		}
		for (Station station = cross; station != null; station = station.next)
		{
			Angle.ToRect(station.dd, station.azimuth - direction, out var num, out var _);
			w.Write(string.Format(Exporter.inv, "{0:f3}\t", (double)x / 1000.0));
			w.Write(string.Format(Exporter.inv, "{0:f3}\t", (double)(-y) / 1000.0));
			w.Write(string.Format(Exporter.inv, "{0:f3}\t", (double)(x + num) / 1000.0));
			w.WriteLine(string.Format(Exporter.inv, "{0:f3}\t", (double)(-(y - station.dh)) / 1000.0));
		}
	}

	internal void WriteDXF(DXFWriter w, Pen pen, Pen dashed)
	{
		Point p;
		if ((w.opt & DXFWriter.Options.SIDE) != DXFWriter.Options.NONE)
		{
			p = new Point(d, h);
			if (start != null && (flags & Flags.invisible) == 0)
			{
				Point point = new Point(start.d, start.h);
				int num = d - start.d;
				if (to == ID.UNDEF && dist * (dist >> 4) > num * (num >> 3) + dh * (dh >> 3))
				{
					pen = dashed;
				}
				if (pen != null)
				{
					w.DxfLine(point, p, pen, 0, 0, 0, 0);
				}
			}
		}
		else
		{
			p = new Point(x, y);
			if (start != null && (flags & Flags.invisible) == 0)
			{
				Point point = new Point(start.x, start.y);
				if (to == ID.UNDEF && Math.Abs((int)incl) > 8192)
				{
					pen = dashed;
				}
				if (pen != null)
				{
					w.DxfLine(point, p, pen, 0, 0, 0, 0);
				}
			}
		}
		if (to != ID.UNDEF)
		{
			w.DxfCircle(p, pen, 0, 0);
			if ((w.opt & DXFWriter.Options.ENL) != DXFWriter.Options.NONE && dist > 0)
			{
				w.DxfText(p, this.num.ToString(), 0, 0);
			}
		}
	}

	internal void WriteXSectDXF(DXFWriter w, int x, int y, int direction, int x0, int y0)
	{
		Point point = new Point(x, y);
		w.DxfCircle(point, MainForm.shotRd, x0, y0);
		if ((w.opt & DXFWriter.Options.ENX) != DXFWriter.Options.NONE)
		{
			if (direction == -1)
			{
				for (Station station = cross; station != null; station = station.next)
				{
					Point point2 = new Point(x + station.dx, y - station.dy);
					Pen pen = MainForm.shotXs;
					if (Math.Abs((int)station.incl) > 8192)
					{
						pen = MainForm.shotDa;
					}
					w.DxfLine(point, point2, pen, x0, y0, x0, y0);
				}
			}
			else
			{
				for (Station station = cross; station != null; station = station.next)
				{
					Angle.ToRect(station.dd, station.azimuth - direction, out var num, out var value);
					Point point2 = new Point(x + num, y - station.dh);
					Pen pen = MainForm.shotXs;
					if (Math.Abs(value) * 7 > station.dist * 5)
					{
						pen = MainForm.shotDa;
					}
					w.DxfLine(point, point2, pen, x0, y0, x0, y0);
				}
			}
		}
		if ((w.opt & DXFWriter.Options.ENL) != DXFWriter.Options.NONE && dist > 0)
		{
			w.DxfText(point, this.num.ToString(), x0, y0);
		}
	}
}
