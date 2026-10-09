using System;
using System.Drawing;
using System.IO;

namespace PocketTopo;

internal class Polygon : Element
{
	internal const byte id = 1;

	private Point[] points;

	private Pen pen;

	private Rectangle box;

	private static Point left;

	private static Point right;

	private static Point last;

	private static Point temp;

	private static int Orientation(Point a, Point b, Point c)
	{
		return (c.Y - a.Y) * (b.X - a.X) - (c.X - a.X) * (b.Y - a.Y);
	}

	internal static Polygon AddNew(Drawing list, int x, int y, Pen pen)
	{
		list.mapping.Screen2AltWorld(x, y, out x, out y);
		Polygon polygon = new Polygon();
		Point[] array = new Point[1];
		array[0].X = x;
		array[0].Y = y;
		polygon.points = array;
		temp = (last = array[0]);
		polygon.pen = pen;
		Rectangle s = (polygon.box = new Rectangle(x, y, 1, 1));
		list.Add(polygon);
		list.mapping.AltWorld2Screen(s, ref s);
		s.X--;
		s.Y--;
		s.Width += 3;
		s.Height += 2;
		list.mapping.ctrl.Invalidate(s);
		return polygon;
	}

	internal void AddPoint(int x, int y)
	{
		list.mapping.Screen2AltWorld(x, y, out x, out y);
		if (x == temp.X && y == temp.Y)
		{
			return;
		}
		Point point = new Point(x, y);
		if (last != temp && (Orientation(last, left, point) < 0 || Orientation(last, right, point) > 0))
		{
			last = temp;
		}
		int num = point.X - last.X;
		int num2 = point.Y - last.Y;
		int num3 = list.mapping.ScaleFactor() >> 1;
		Point c = point;
		Point c2 = point;
		if (num > 0)
		{
			c.Y -= num3;
			c2.Y += num3;
		}
		if (num < 0)
		{
			c.Y += num3;
			c2.Y -= num3;
		}
		if (num2 > 0)
		{
			c.X += num3;
			c2.X -= num3;
		}
		if (num2 < 0)
		{
			c.X -= num3;
			c2.X += num3;
		}
		if (last != temp)
		{
			if (Orientation(last, left, c) > 0)
			{
				left = c;
			}
			if (Orientation(last, right, c2) < 0)
			{
				right = c2;
			}
		}
		else
		{
			left = c;
			right = c2;
			int num4 = points.Length;
			Point[] destinationArray = new Point[num4 + 1];
			Array.Copy(points, destinationArray, num4);
			points = destinationArray;
		}
		points[points.Length - 1] = point;
		Rectangle s = Rectangle.FromLTRB(Math.Min(x, last.X), Math.Min(y, last.Y), Math.Max(x, last.X), Math.Max(y, last.Y));
		box = Rectangle.Union(box, s);
		if (!s.Contains(temp))
		{
			s = Rectangle.Union(s, new Rectangle(temp.X, temp.Y, 1, 1));
		}
		list.mapping.AltWorld2Screen(s, ref s);
		s.X--;
		s.Y--;
		s.Width += 2;
		s.Height += 2;
		list.mapping.ctrl.Invalidate(s);
		temp = point;
	}

	internal override int Distance(int x, int y)
	{
		int num = int.MaxValue;
		for (int i = 0; i < points.Length; i++)
		{
			int num2 = Math.Abs(x - points[i].X) + Math.Abs(y - points[i].Y);
			if (num2 < num)
			{
				num = num2;
			}
		}
		return num;
	}

	internal override int Paint(Graphics g, Rectangle clip)
	{
		if (box.IntersectsWith(clip))
		{
			Mapping mapping = list.mapping;
			Drawing.SetupPen(pen);
			int num = points.Length;
			mapping.AltWorld2Screen(points[0].X, points[0].Y, out var sx, out var sy);
			int sx2;
			int sy2;
			if (num > 2)
			{
				Point[] array = new Point[num];
				ref Point reference = ref array[0];
				reference = new Point(sx, sy);
				for (int i = 1; i < num; i++)
				{
					mapping.AltWorld2Screen(points[i].X, points[i].Y, out sx2, out sy2);
					ref Point reference2 = ref array[i];
					reference2 = new Point(sx2, sy2);
				}
				g.DrawLines(pen, array);
			}
			else if (num > 1)
			{
				mapping.AltWorld2Screen(points[1].X, points[1].Y, out sx2, out sy2);
				g.DrawLine(pen, sx, sy, sx2, sy2);
			}
			else
			{
				g.DrawLine(pen, sx, sy, sx + 1, sy);
			}
			return 1;
		}
		return 0;
	}

	internal override void Offset(int dx, int dy)
	{
		for (int i = 0; i < points.Length; i++)
		{
			points[i].X += dx;
			points[i].Y += dy;
		}
		box.X += dx;
		box.Y += dy;
	}

	internal override void Write(FileWriter w)
	{
		w.w.Write((byte)1);
		w.w.Write(points.Length);
		for (int i = 0; i < points.Length; i++)
		{
			w.w.Write(points[i].X);
			w.w.Write(points[i].Y);
		}
		if (pen == MainForm.black)
		{
			w.w.Write((byte)1);
		}
		else if (pen == MainForm.gray)
		{
			w.w.Write((byte)2);
		}
		else if (pen == MainForm.brown)
		{
			w.w.Write((byte)3);
		}
		else if (pen == MainForm.blue)
		{
			w.w.Write((byte)4);
		}
		else if (pen == MainForm.red)
		{
			w.w.Write((byte)5);
		}
		else if (pen == MainForm.orange)
		{
			w.w.Write((byte)7);
		}
		else
		{
			w.w.Write((byte)6);
		}
	}

	internal override void Read(FileReader r)
	{
		int num = r.r.ReadInt32();
		points = new Point[num];
		int num2 = int.MaxValue;
		int num3 = int.MaxValue;
		int num4 = int.MinValue;
		int num5 = int.MinValue;
		for (int i = 0; i < num; i++)
		{
			int num6 = r.r.ReadInt32();
			int num7 = r.r.ReadInt32();
			points[i].X = num6;
			points[i].Y = num7;
			if (num6 < num2)
			{
				num2 = num6;
			}
			if (num6 > num4)
			{
				num4 = num6;
			}
			if (num7 < num3)
			{
				num3 = num7;
			}
			if (num7 > num5)
			{
				num5 = num7;
			}
		}
		box = Rectangle.FromLTRB(num2, num3, num4, num5);
		switch (r.r.ReadByte())
		{
		case 1:
			pen = MainForm.black;
			break;
		case 2:
			pen = MainForm.gray;
			break;
		case 3:
			pen = MainForm.brown;
			break;
		case 4:
			pen = MainForm.blue;
			break;
		case 5:
			pen = MainForm.red;
			break;
		case 7:
			pen = MainForm.orange;
			break;
		default:
			pen = MainForm.green;
			break;
		}
	}

	internal override void WriteDXF(DXFWriter w, int x0, int y0)
	{
		if (points.Length > 1)
		{
			w.DxfPolygon(pen);
			for (int i = 0; i < points.Length; i++)
			{
				w.DxfVertex(pen, points[i], x0, y0);
			}
			w.DxfEnd();
		}
		else
		{
			w.DxfPoint(points[0], pen, x0, y0);
		}
	}

	internal override void WriteTherion(TextWriter w, int x0, int y0, int opt)
	{
		if (opt == 0)
		{
			w.Write("POLYLINE ");
			if (pen == MainForm.red)
			{
				w.WriteLine("RED");
			}
			else if (pen == MainForm.blue)
			{
				w.WriteLine("BLUE");
			}
			else if (pen == MainForm.gray)
			{
				w.WriteLine("GRAY");
			}
			else if (pen == MainForm.brown)
			{
				w.WriteLine("BROWN");
			}
			else if (pen == MainForm.green)
			{
				w.WriteLine("GREEN");
			}
			else if (pen == MainForm.orange)
			{
				w.WriteLine("ORANGE");
			}
			else
			{
				w.WriteLine("BLACK");
			}
			for (int i = 0; i < points.Length; i++)
			{
				w.Write(string.Format(Exporter.inv, "{0:f3}\t", (double)(points[i].X + x0) / 1000.0));
				w.WriteLine(string.Format(Exporter.inv, "{0:f3}", (double)(-(points[i].Y + y0)) / 1000.0));
			}
		}
	}
}
