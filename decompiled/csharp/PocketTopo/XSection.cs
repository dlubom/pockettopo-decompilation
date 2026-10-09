using System;
using System.Drawing;
using System.IO;

namespace PocketTopo;

internal class XSection : Element
{
	internal const byte id = 3;

	private Point center;

	private ID num;

	private Station station;

	private int direction;

	private Rectangle box;

	internal static XSection actual;

	internal static Survey survey;

	internal static void Create(Survey survey, Station station, bool hor)
	{
		XSection xSection = new XSection();
		xSection.station = station;
		xSection.num = station.num;
		if (hor)
		{
			xSection.direction = -1;
		}
		else
		{
			xSection.direction = (ushort)(int)station.FwAzimuth;
			Station station2 = survey.SearchById(station.num.Next());
			if (station2 != null && station2.start == station)
			{
				xSection.direction = (ushort)Angle.AngleBisector(station.FwAzimuth, station2.FwAzimuth);
			}
		}
		actual = xSection;
	}

	internal static void Add(Drawing list, int x, int y)
	{
		list.mapping.Screen2AltWorld(x, y, out x, out y);
		actual.center = new Point(x, y);
		actual.SetBox();
		list.Add(actual);
		actual = null;
		list.mapping.ctrl.Invalidate();
	}

	private void SetBox()
	{
		if (station == null)
		{
			station = survey.SearchById(this.num);
		}
		int num = 0;
		if (station != null)
		{
			for (Station cross = station.cross; cross != null; cross = cross.next)
			{
				if (cross.dist > num)
				{
					num = cross.dist;
				}
			}
		}
		box = new Rectangle(center.X - num, center.Y - num, num << 1, num << 1);
	}

	internal override int Distance(int x, int y)
	{
		return Math.Abs(x - center.X) + Math.Abs(y - center.Y);
	}

	internal override int Paint(Graphics g, Rectangle clip)
	{
		if (station == null)
		{
			SetBox();
		}
		if (station != null && box.IntersectsWith(clip))
		{
			list.mapping.AltWorld2Screen(center.X, center.Y, out var sx, out var sy);
			int sx2;
			int sy2;
			if (list.side)
			{
				list.mapping.World2Screen(station.d, station.h, out sx2, out sy2);
			}
			else
			{
				list.mapping.World2Screen(station.x, station.y, out sx2, out sy2);
			}
			g.DrawLine(MainForm.dashed, sx, sy, sx2, sy2);
			station.PaintXSection(list.mapping, g, MainForm.shotXs, center.X, center.Y, direction);
			return 1;
		}
		return 0;
	}

	internal override void Offset(int dx, int dy)
	{
		center.X += dx;
		center.Y += dy;
		station = null;
	}

	internal override void Write(FileWriter w)
	{
		w.w.Write((byte)3);
		w.w.Write(center.X);
		w.w.Write(center.Y);
		if (station != null)
		{
			num = station.num;
		}
		num.Write(w);
		w.w.Write(direction);
	}

	internal override void Read(FileReader r)
	{
		int x = r.r.ReadInt32();
		int y = r.r.ReadInt32();
		center = new Point(x, y);
		num = ID.Read(r);
		station = null;
		direction = r.r.ReadInt32();
	}

	internal override void WriteDXF(DXFWriter w, int x0, int y0)
	{
		if (station == null)
		{
			SetBox();
		}
		if (station != null)
		{
			w.DxfLine(to: (!list.side) ? new Point(station.x, station.y) : new Point(station.d, station.h), from: center, pen: MainForm.dashed, x0f: x0, y0f: y0, x0t: 0, y0t: 0);
			if ((w.opt & DXFWriter.Options.ENS) != DXFWriter.Options.NONE)
			{
				station.WriteXSectDXF(w, center.X, center.Y, direction, x0, y0);
			}
		}
	}

	internal override void WriteTherion(TextWriter w, int x0, int y0, int opt)
	{
		if (station == null)
		{
			SetBox();
		}
		if (station != null)
		{
			switch (opt)
			{
			case 0:
				w.Write("POLYLINE ");
				w.WriteLine("CONNECT");
				w.Write(string.Format(Exporter.inv, "{0:f3}\t", (double)(center.X + x0) / 1000.0));
				w.WriteLine(string.Format(Exporter.inv, "{0:f3}", (double)(-(center.Y + y0)) / 1000.0));
				w.Write(string.Format(Exporter.inv, "{0:f3}\t", (double)(list.side ? station.d : station.x) / 1000.0));
				w.WriteLine(string.Format(Exporter.inv, "{0:f3}", (double)(-(list.side ? station.h : station.y)) / 1000.0));
				break;
			case 1:
				w.Write(string.Format(Exporter.inv, "{0:f3}\t", (double)(center.X + x0) / 1000.0));
				w.Write(string.Format(Exporter.inv, "{0:f3}\t", (double)(-(center.Y + y0)) / 1000.0));
				w.Write(station.num.ToString());
				w.WriteLine();
				break;
			default:
				station.WriteThXSect(w, center.X + x0, center.Y + y0, direction);
				break;
			}
		}
	}
}
