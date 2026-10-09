using System.Collections;
using System.IO;

namespace PocketTopo;

internal class ThreeD
{
	private struct Vector
	{
		internal int x;

		internal int y;

		internal int z;

		internal Vector(int x, int y, int z)
		{
			this.x = x;
			this.y = y;
			this.z = z;
		}

		internal Vector(Point a, Point b)
			: this(b.x - a.x, b.y - a.y, b.z - a.z)
		{
		}

		internal Vector(Point a, Point b, Point c)
			: this((b.y - a.y) * (c.z - a.z) - (b.z - a.z) * (c.y - a.y), (b.z - a.z) * (c.x - a.x) - (b.x - a.x) * (c.z - a.z), (b.x - a.x) * (c.y - a.y) - (b.y - a.y) * (c.x - a.x))
		{
		}

		public static long operator *(Vector a, Vector b)
		{
			return (long)a.x * (long)b.x + (long)a.y * (long)b.y + (long)a.z * (long)b.z;
		}
	}

	private class Point
	{
		internal int x;

		internal int y;

		internal int z;

		internal Point next;

		internal object edge;

		internal Point(int x, int y, int z)
		{
			this.x = x;
			this.y = y;
			this.z = z;
		}

		internal Point(int x, int y, int z, ref Point list)
		{
			this.x = x;
			this.y = y;
			this.z = z;
			next = list;
			list = this;
		}

		internal bool Add(Point p)
		{
			if (edge == null)
			{
				edge = p;
			}
			else
			{
				ArrayList arrayList;
				if (edge is ArrayList)
				{
					arrayList = (ArrayList)edge;
				}
				else
				{
					arrayList = new ArrayList();
					arrayList.Add(edge);
					edge = arrayList;
				}
				if (arrayList.Contains(p))
				{
					return false;
				}
				arrayList.Add(p);
			}
			return true;
		}

		internal bool Remove(Point p)
		{
			if (edge != null)
			{
				if (edge == p)
				{
					edge = null;
					return true;
				}
				if (edge is ArrayList)
				{
					ArrayList arrayList = (ArrayList)edge;
					if (arrayList.Contains(p))
					{
						arrayList.Remove(p);
						if (arrayList.Count == 0)
						{
							edge = null;
						}
						return true;
					}
				}
			}
			return false;
		}

		internal Point Next()
		{
			if (edge != null)
			{
				Point result;
				if (edge is Point)
				{
					result = (Point)edge;
					edge = null;
				}
				else
				{
					ArrayList arrayList = (ArrayList)edge;
					result = (Point)arrayList[0];
					arrayList.RemoveAt(0);
					if (arrayList.Count == 0)
					{
						edge = null;
					}
				}
				return result;
			}
			return null;
		}
	}

	private TextWriter wr;

	private Point points;

	private Vector prev;

	internal ThreeD(TextWriter wr)
	{
		this.wr = wr;
	}

	private void DxfElement(string name)
	{
		wr.WriteLine(0);
		wr.WriteLine(name);
		wr.WriteLine(8);
		wr.WriteLine(0);
	}

	private void DxfCoord(Point p, int index)
	{
		wr.WriteLine(10 + index);
		wr.WriteLine((p.x << 4).ToString());
		wr.WriteLine(20 + index);
		wr.WriteLine((p.y << 4).ToString());
		wr.WriteLine(30 + index);
		wr.WriteLine((p.z << 4).ToString());
	}

	private void DxfLine(Point a, Point b)
	{
		DxfElement("LINE");
		DxfCoord(a, 0);
		DxfCoord(b, 1);
	}

	private void DxfFace(Point a, Point b, Point c)
	{
		DxfElement("3DFACE");
		DxfCoord(a, 0);
		DxfCoord(b, 1);
		DxfCoord(c, 2);
		DxfCoord(c, 3);
	}

	private void AddPoints(Station s)
	{
		while (s.cross == null && s.dist == 0 && s.start != null)
		{
			s = s.start;
		}
		new Point(s.x >> 4, -s.y >> 4, -s.h >> 4, ref points);
		for (Station station = s.cross; station != null; station = station.next)
		{
			new Point(station.x >> 4, -station.y >> 4, -station.h >> 4, ref points);
		}
	}

	private Point FindFace(Point a, Point b)
	{
		Point point = points;
		Vector vector = new Vector(a, b, point);
		for (Point next = point.next; next != null; next = next.next)
		{
			long num = new Vector(a, next) * vector;
			if (num > 0)
			{
				point = next;
				vector = new Vector(a, b, point);
			}
			else if (num == 0)
			{
				num = new Vector(a, b, next) * vector;
				if (num < 0)
				{
					if (vector * prev < 0)
					{
						point = next;
						vector = new Vector(a, b, point);
					}
				}
				else if (num == 0)
				{
					if (vector.x == 0 && vector.y == 0 && vector.z == 0)
					{
						point = next;
						vector = new Vector(a, b, point);
					}
				}
				else
				{
					num = new Vector(b, point, next) * vector;
					if (num < 0 || (num == 0 && new Vector(a, point, next) * vector > 0))
					{
						point = next;
					}
				}
			}
		}
		prev = vector;
		return point;
	}

	private void WriteConvexHull()
	{
		int num = int.MinValue;
		Point next;
		for (next = points; next != null; next = next.next)
		{
			if (next.x > num)
			{
				num = next.x;
			}
		}
		Point point = new Point(num + 1, points.y, points.z);
		Point a = new Point(num + 1, points.y, points.z + 1);
		next = FindFace(a, point);
		Point point2 = FindFace(point, next);
		point2.Add(next);
		while (true)
		{
			bool flag = true;
			Point point3 = FindFace(next, point2);
			DxfFace(next, point2, point3);
			if (point3.Remove(next))
			{
				if (point2.Remove(point3))
				{
					next = points;
					while (next.edge == null)
					{
						next = next.next;
						if (next == null)
						{
							return;
						}
					}
					point2 = next.Next();
				}
				else
				{
					next = point3;
				}
			}
			else if (point2.Remove(point3))
			{
				point2 = point3;
			}
			else
			{
				if (!point3.Add(point2))
				{
					break;
				}
				point2 = point3;
			}
		}
	}

	private void WriteStretch(Station s0, Station s1)
	{
		points = null;
		AddPoints(s0);
		AddPoints(s1);
		if (points != null && points.next != null)
		{
			if (points.next.next == null)
			{
				DxfLine(points, points.next);
			}
			else if (points.next.next.next == null)
			{
				DxfFace(points, points.next, points.next.next);
			}
			else
			{
				WriteConvexHull();
			}
		}
	}

	private static void WriteDXF3D(TextWriter w, Survey survey)
	{
		ThreeD threeD = new ThreeD(w);
		Station[] stations = survey.GetStations(inclTemplate: true);
		foreach (Station station in stations)
		{
			if (station.dist > 0 && station.start != null && !station.invisible)
			{
				threeD.WriteStretch(station.start, station);
			}
		}
	}

	internal static void WriteDXF(string name, Survey survey, bool side)
	{
		try
		{
			using FileStream stream = File.Open(name, FileMode.Create);
			using TextWriter textWriter = new StreamWriter(stream);
			textWriter.WriteLine(0);
			textWriter.WriteLine("SECTION");
			textWriter.WriteLine(2);
			textWriter.WriteLine("HEADER");
			textWriter.WriteLine(9);
			textWriter.WriteLine("$ACADVER");
			textWriter.WriteLine(1);
			textWriter.WriteLine("AC1002");
			textWriter.WriteLine(0);
			textWriter.WriteLine("ENDSEC");
			textWriter.WriteLine(0);
			textWriter.WriteLine("SECTION");
			textWriter.WriteLine(2);
			textWriter.WriteLine("ENTITIES");
			WriteDXF3D(textWriter, survey);
			textWriter.WriteLine(0);
			textWriter.WriteLine("ENDSEC");
			textWriter.WriteLine(0);
			textWriter.WriteLine("EOF");
			textWriter.Close();
		}
		catch
		{
		}
	}
}
