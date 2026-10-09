using System;
using System.Collections;
using System.Globalization;
using System.IO;

namespace PocketTopo;

public class Toporobot
{
	internal class Reference
	{
		internal string name;

		internal double e;

		internal double n;

		internal double h;

		internal int ser;

		internal int num;

		internal string comment;
	}

	internal class Trip
	{
		internal int idx;

		internal int day;

		internal int month;

		internal int year;

		internal string name1;

		internal string name2;

		internal bool auto;

		internal float aziOffs;

		internal float inclOffs;

		internal int color;

		internal string comment;
	}

	internal class Code
	{
		internal int idx;

		internal int aziUnit;

		internal int inclUnit;

		internal float deltaLen;

		internal float deltaAzi;

		internal float deltaIncl;

		internal float ratio;

		internal float limit;

		internal string comment;
	}

	internal class Series
	{
		internal string name;

		internal int startSer;

		internal int startNum;

		internal int endSer;

		internal int endNum;

		internal int length;

		internal int chance;

		internal int obst;

		internal ArrayList stations = new ArrayList();

		internal string comment;
	}

	internal class Station
	{
		internal int trip;

		internal int code;

		internal float dist;

		internal float azimuth;

		internal float incl;

		internal float left;

		internal float right;

		internal float up;

		internal float down;

		internal string comment;

		internal ArrayList lines;
	}

	internal class Line
	{
		internal float dist;

		internal float azimuth;

		internal float incl;
	}

	private const int rev = -10;

	private const int zen = 1;

	private const int nad = -1;

	private const int percent = 370;

	private const int vertical = 380;

	internal static CultureInfo inv = CultureInfo.InvariantCulture;

	internal ArrayList refs = new ArrayList();

	internal ArrayList codes = new ArrayList();

	internal ArrayList trips = new ArrayList();

	internal ArrayList series = new ArrayList();

	private object actual = null;

	internal static Toporobot Read(TextReader r)
	{
		Toporobot toporobot = new Toporobot();
		while (toporobot.ReadLine(r))
		{
		}
		toporobot.actual = null;
		toporobot.CleanupTrips();
		return toporobot;
	}

	private bool ReadLine(TextReader r)
	{
		string text = r.ReadLine();
		if (text == null)
		{
			return false;
		}
		if (text == "")
		{
			return true;
		}
		int num = int.Parse(text.Substring(1, 5));
		int num2 = int.Parse(text.Substring(6, 6));
		int code = int.Parse(text.Substring(12, 4));
		int trip = int.Parse(text.Substring(20, 4));
		if (text[0] == '(')
		{
			if (text.Length >= 26 && actual != null)
			{
				string text2 = text.Substring(25);
				string text3 = "\r\n";
				if (text[24] == '-')
				{
					text3 = "";
				}
				int length = text2.Length;
				if (actual is Station)
				{
					Station station = (Station)actual;
					if (text2[0] == '[' && text2[length - 1] == ']')
					{
						Line line = new Line();
						int num3 = text2.IndexOf(' ');
						int num4 = text2.IndexOf(' ', num3 + 1);
						line.dist = float.Parse(text2.Substring(1, num3 - 1), inv);
						line.azimuth = float.Parse(text2.Substring(num3 + 1, num4 - num3 - 1), inv);
						line.incl = float.Parse(text2.Substring(num4 + 1, length - num4 - 2), inv);
						if (station.lines == null)
						{
							station.lines = new ArrayList();
						}
						station.lines.Add(line);
					}
					else if (station.comment == null)
					{
						station.comment = text2;
					}
					else
					{
						station.comment = station.comment + text3 + text2;
					}
				}
				else if (actual is Series)
				{
					Series series = (Series)actual;
					if (series.comment == null)
					{
						series.comment = text2;
					}
					else
					{
						series.comment = series.comment + text3 + text2;
					}
				}
				else if (actual is Trip)
				{
					Trip trip2 = (Trip)actual;
					if (trip2.comment == null)
					{
						if (text3 == "" && (text2[length - 1] == '1' || text2[length - 1] == '2'))
						{
							int num5 = length - 1;
							if (num5 > 0 && text2[num5 - 1] == '\u00b8')
							{
								num5--;
							}
							string text4 = text2.Substring(0, num5);
							if (text2[length - 1] == '1')
							{
								trip2.name1 += text4;
							}
							else
							{
								trip2.name2 += text4;
							}
						}
						else
						{
							trip2.comment = text2;
						}
					}
					else
					{
						trip2.comment = trip2.comment + text3 + text2;
					}
				}
				else if (actual is Code)
				{
					Code code2 = (Code)actual;
					if (code2.comment == null)
					{
						code2.comment = text2;
					}
					else
					{
						code2.comment = code2.comment + text3 + text2;
					}
				}
				else if (actual is Reference)
				{
					Reference reference = (Reference)actual;
					if (reference.comment == null)
					{
						reference.comment = text2;
					}
					else
					{
						reference.comment = reference.comment + text3 + text2;
					}
				}
			}
		}
		else
		{
			switch (num)
			{
			case -6:
			{
				Reference reference = new Reference();
				if (text.Length >= 26)
				{
					reference.name = text.Substring(25);
				}
				while (refs.Count <= num2)
				{
					refs.Add(null);
				}
				refs[num2] = reference;
				actual = null;
				break;
			}
			case -5:
			{
				Reference reference = (Reference)refs[num2];
				reference.e = double.Parse(text.Substring(24, 12), inv);
				reference.n = double.Parse(text.Substring(36, 12), inv);
				reference.h = double.Parse(text.Substring(48, 12), inv);
				reference.ser = int.Parse(text.Substring(60, 6));
				reference.num = int.Parse(text.Substring(66, 6));
				actual = reference;
				break;
			}
			case -4:
				actual = null;
				break;
			case -3:
				actual = null;
				break;
			case -2:
			{
				Trip trip2 = new Trip();
				trip2.idx = num2;
				trip2.day = int.Parse(text.Substring(25, 2));
				if (text[27] == '/')
				{
					trip2.month = int.Parse(text.Substring(28, 2));
					trip2.year = int.Parse(text.Substring(31, 2)) + 1900;
					if (trip2.year < 1920)
					{
						trip2.year += 100;
					}
				}
				else
				{
					trip2.month = int.Parse(text.Substring(27, 2));
					trip2.year = int.Parse(text.Substring(29, 4));
				}
				trip2.name1 = text.Substring(35, 13);
				trip2.name2 = text.Substring(49, 13);
				trip2.auto = int.Parse(text.Substring(62, 2)) != 0;
				trip2.aziOffs = float.Parse(text.Substring(64, 8), inv);
				trip2.inclOffs = (float)int.Parse(text.Substring(72, 4)) / 10f;
				trip2.color = int.Parse(text.Substring(76, 4));
				while (trips.Count <= num2)
				{
					trips.Add(null);
				}
				trips[num2] = trip2;
				actual = trip2;
				break;
			}
			case -1:
			{
				Code code2 = new Code();
				code2.idx = num2;
				code2.aziUnit = (int)Math.Round(double.Parse(text.Substring(24, 8), inv));
				code2.inclUnit = (int)Math.Round(double.Parse(text.Substring(32, 8), inv));
				code2.deltaLen = float.Parse(text.Substring(40, 8), inv);
				code2.deltaAzi = float.Parse(text.Substring(48, 8), inv);
				code2.deltaIncl = float.Parse(text.Substring(56, 8), inv);
				code2.ratio = float.Parse(text.Substring(64, 8), inv);
				code2.limit = float.Parse(text.Substring(72, 8), inv);
				while (codes.Count <= num2)
				{
					codes.Add(null);
				}
				codes[num2] = code2;
				actual = code2;
				break;
			}
			default:
				switch (num2)
				{
				case -2:
				{
					Series series = new Series();
					if (text.Length >= 26)
					{
						series.name = text.Substring(25);
					}
					while (this.series.Count <= num)
					{
						this.series.Add(null);
					}
					this.series[num] = series;
					actual = null;
					break;
				}
				case -1:
				{
					Series series = (Series)this.series[num];
					series.startSer = int.Parse(text.Substring(24, 8));
					series.startNum = int.Parse(text.Substring(32, 8));
					series.endSer = int.Parse(text.Substring(40, 8));
					series.endNum = int.Parse(text.Substring(48, 8));
					series.length = int.Parse(text.Substring(56, 8));
					series.chance = int.Parse(text.Substring(64, 8));
					series.obst = int.Parse(text.Substring(72, 8));
					series.stations = new ArrayList(series.length + 1);
					actual = series;
					break;
				}
				default:
				{
					Station station = new Station();
					station.trip = trip;
					station.code = code;
					station.dist = float.Parse(text.Substring(24, 8), inv);
					station.azimuth = float.Parse(text.Substring(32, 8), inv);
					station.incl = float.Parse(text.Substring(40, 8), inv);
					station.left = float.Parse(text.Substring(48, 8), inv);
					station.right = float.Parse(text.Substring(56, 8), inv);
					station.up = float.Parse(text.Substring(64, 8), inv);
					station.down = float.Parse(text.Substring(72, 8), inv);
					((Series)this.series[num]).stations.Add(station);
					actual = station;
					break;
				}
				}
				break;
			}
		}
		return true;
	}

	private void CleanupTrips()
	{
		for (int i = 1; i < trips.Count; i++)
		{
			if (trips[i] != null)
			{
				Trip trip = (Trip)trips[i];
				trip.name1 = trip.name1.Trim();
				trip.name2 = trip.name2.Trim();
			}
		}
	}

	internal void Write(TextWriter w)
	{
		if (refs.Count > 0)
		{
			for (int i = 1; i < refs.Count; i++)
			{
				if (refs[i] != null)
				{
					LineHeader(w, -6, i, 1, 1);
					w.WriteLine(Limit(((Reference)refs[i]).name, 55));
				}
			}
			for (int i = 1; i < refs.Count; i++)
			{
				if (refs[i] != null)
				{
					LineHeader(w, -5, i, 1, 1);
					Reference reference = (Reference)refs[i];
					w.Write(string.Format(inv, " {0,10:f2}  {1,10:f2}  {2,10:f2}", reference.e, reference.n, reference.h));
					w.WriteLine("{0,6:d}{1,6:d}", reference.ser, reference.num);
					CommentLines(w, -5, i, reference.comment, 1);
				}
			}
			DateTime now = DateTime.Now;
			for (int i = 1; i <= 2; i++)
			{
				LineHeader(w, -4, i, 1, 1);
				w.Write("{0:d2}/{1:d2}/{2:d2} ", new object[3]
				{
					now.Year % 100,
					now.Month,
					now.Day
				});
				w.WriteLine("{0:d2}:{1:d2}:{2:d2}  Guest", new object[3] { now.Hour, now.Minute, now.Second });
			}
			LineHeader(w, -3, 1, 1, 1);
			w.WriteLine();
		}
		for (int i = 1; i < trips.Count; i++)
		{
			if (trips[i] != null)
			{
				LineHeader(w, -2, i, 1, 1);
				Trip trip = (Trip)trips[i];
				if (trip.year >= 1920 && trip.year < 2020)
				{
					w.Write("{0:d2}/{1:d2}/{2:d2}  ", new object[3]
					{
						trip.day,
						trip.month,
						trip.year % 100
					});
				}
				else
				{
					w.Write("{0:d2}{1:d2}{2:d4}  ", new object[3] { trip.day, trip.month, trip.year });
				}
				w.Write(Limit(trip.name1, 12).PadRight(14));
				w.Write(Limit(trip.name2, 12).PadRight(14));
				w.Write(trip.auto ? '1' : '0');
				w.WriteLine(string.Format(inv, " {0,7:f2}{1,4:d1}{2,4:d1}", trip.aziOffs, (int)Math.Round(trip.inclOffs * 10f), trip.color));
				int n = 1;
				if (trip.name1.Length > 12)
				{
					LineHeader(w, -2, i, n++, -1);
					w.WriteLine("-{0}\u00b81", Limit(trip.name1.Substring(12), 53));
				}
				if (trip.name2.Length > 12)
				{
					LineHeader(w, -2, i, n++, -1);
					w.WriteLine("-{0}\u00b82", Limit(trip.name2.Substring(12), 53));
				}
				CommentLines(w, -2, i, trip.comment, n);
			}
		}
		for (int i = 1; i < codes.Count; i++)
		{
			if (codes[i] != null)
			{
				LineHeader(w, -1, i, 1, 1);
				Code code = (Code)codes[i];
				w.Write(" {0:d3}.00  {1:d3}.00", code.aziUnit, code.inclUnit);
				w.Write(string.Format(inv, " {0,7:f2} {1,7:f2} {2,7:f2}", code.deltaLen, code.deltaAzi, code.deltaIncl));
				w.WriteLine(string.Format(inv, " {0,7:f2} {1,7:f2}", code.ratio, code.limit));
				CommentLines(w, -1, i, code.comment, 1);
			}
		}
		for (int i = 1; i < this.series.Count; i++)
		{
			if (this.series[i] == null)
			{
				continue;
			}
			Series series = (Series)this.series[i];
			LineHeader(w, i, -2, 1, 1);
			string name = series.name;
			if (name != null)
			{
				w.WriteLine(Limit(name, 55));
			}
			else
			{
				w.WriteLine();
			}
			LineHeader(w, i, -1, 1, 1);
			w.Write("{0,7:d} {1,7:d}", series.startSer, series.startNum);
			w.Write(" {0,7:d} {1,7:d} {2,7:d}", new object[3] { series.endSer, series.endNum, series.length });
			w.WriteLine(" {0,7:d} {1,7:d}", series.chance, series.obst);
			CommentLines(w, i, -1, series.comment, 1);
			for (int j = 0; j < series.stations.Count; j++)
			{
				Station station = (Station)series.stations[j];
				if (station == null)
				{
					continue;
				}
				LineHeader(w, i, j, station.code, station.trip);
				w.Write(string.Format(inv, "{0,7:f2} {1,7:f2} {2,7:f2}", station.dist, station.azimuth, station.incl));
				w.Write(string.Format(inv, " {0,7:f2} {1,7:f2}", station.left, station.right));
				w.WriteLine(string.Format(inv, " {0,7:f2} {1,7:f2}", station.up, station.down));
				int n = CommentLines(w, i, j, station.comment, 1);
				if (station.lines != null)
				{
					for (int k = 0; k < station.lines.Count; k++)
					{
						Line line = (Line)station.lines[k];
						LineHeader(w, i, j, n++, -1);
						w.WriteLine(string.Format(inv, " [{0:f2} {1:f1} {2:f1}]", line.dist, line.azimuth, line.incl));
					}
				}
			}
		}
	}

	private string Limit(string s, int len)
	{
		if (s.Length <= len)
		{
			return s;
		}
		return s.Substring(0, len);
	}

	private void LineHeader(TextWriter w, int ser, int num, int code, int trip)
	{
		if (trip < 0)
		{
			w.Write('(');
		}
		else
		{
			w.Write(' ');
		}
		w.Write("{0,5:d}", ser);
		w.Write("{0,6:d}", num);
		w.Write("{0,4:d}", code);
		w.Write("{0,4:d}", 1);
		w.Write("{0,4:d}", Math.Abs(trip));
		if (trip >= 0)
		{
			w.Write(' ');
		}
	}

	private int CommentLines(TextWriter w, int ser, int st, string comment, int n)
	{
		if (comment != null)
		{
			string[] array = comment.Split('\r', '\n');
			int num = 254;
			for (int i = 0; i < array.Length; i++)
			{
				if (num <= 0)
				{
					break;
				}
				string text = Limit(array[i].Trim(), num);
				if (text != "")
				{
					num -= text.Length + 2;
					LineHeader(w, ser, st, n++, -1);
					w.Write(' ');
					while (text.Length > 55)
					{
						w.WriteLine(text.Substring(0, 55));
						LineHeader(w, ser, st, n++, -1);
						w.Write('-');
						text = text.Substring(55);
					}
					w.WriteLine(text);
				}
			}
		}
		return n;
	}

	internal Reference AddReference(int ser, int num)
	{
		for (int i = 1; i < refs.Count; i++)
		{
			if (refs[i] != null && ((Reference)refs[i]).ser == ser && ((Reference)refs[i]).num == num)
			{
				return (Reference)refs[i];
			}
		}
		Reference reference = new Reference();
		reference.ser = ser;
		reference.num = num;
		if (refs.Count == 0)
		{
			refs.Add(null);
		}
		refs.Add(reference);
		return reference;
	}

	internal Trip AddTrip(int day, int month, int year, string name1, string name2)
	{
		for (int i = 1; i < trips.Count; i++)
		{
			if (trips[i] != null && ((Trip)trips[i]).day == day && ((Trip)trips[i]).month == month && ((Trip)trips[i]).year == year && ((Trip)trips[i]).name1 == name1 && ((Trip)trips[i]).name2 == name2)
			{
				return (Trip)trips[i];
			}
		}
		Trip trip = new Trip();
		trip.day = day;
		trip.month = month;
		trip.year = year;
		trip.name1 = name1;
		trip.name2 = name2;
		if (trips.Count == 0)
		{
			trips.Add(null);
		}
		trip.idx = trips.Count;
		trips.Add(trip);
		return trip;
	}

	internal Code AddCode(int aziUnit, int inclUnit, float deltaLen, float deltaAzi, float deltaIncl, float ratio, float limit)
	{
		for (int i = 1; i < codes.Count; i++)
		{
			if (codes[i] != null && ((Code)codes[i]).aziUnit == aziUnit && ((Code)codes[i]).inclUnit == inclUnit && ((Code)codes[i]).deltaLen == deltaLen && ((Code)codes[i]).deltaAzi == deltaAzi && ((Code)codes[i]).deltaIncl == deltaIncl && ((Code)codes[i]).ratio == ratio && ((Code)codes[i]).limit == limit)
			{
				return (Code)codes[i];
			}
		}
		Code code = new Code();
		code.aziUnit = aziUnit;
		code.inclUnit = inclUnit;
		code.deltaLen = deltaLen;
		code.deltaAzi = deltaAzi;
		code.deltaIncl = deltaIncl;
		code.ratio = ratio;
		code.limit = limit;
		if (codes.Count == 0)
		{
			codes.Add(null);
		}
		code.idx = codes.Count;
		codes.Add(code);
		return code;
	}

	internal Series AddSeries(int ser)
	{
		if (this.series.Count > ser && this.series[ser] != null)
		{
			return (Series)this.series[ser];
		}
		Series series = new Series();
		while (this.series.Count <= ser)
		{
			this.series.Add(null);
		}
		this.series[ser] = series;
		return series;
	}

	internal Station AddStation(int sernum, int stat)
	{
		Series series = (Series)this.series[sernum];
		ArrayList stations = series.stations;
		if (stations.Count > stat && stations[stat] != null)
		{
			return (Station)stations[stat];
		}
		Station station = new Station();
		while (stations.Count <= stat)
		{
			stations.Add(null);
		}
		stations[stat] = station;
		if (stat > series.length)
		{
			if (series.endSer == sernum && series.endNum == series.length)
			{
				series.endNum = stat;
			}
			series.length = stat;
		}
		return station;
	}

	internal Line AddLine(Station stat, float dist, float azimuth, float incl)
	{
		ArrayList arrayList = stat.lines;
		if (arrayList == null)
		{
			arrayList = (stat.lines = new ArrayList());
		}
		else
		{
			for (int i = 0; i < arrayList.Count; i++)
			{
				Line line = (Line)arrayList[i];
				if (line != null && (double)Math.Abs(line.dist - dist) < 0.1 && (double)Math.Abs(line.azimuth - azimuth) < 0.1 && (double)Math.Abs(line.incl - incl) < 0.1)
				{
					return line;
				}
			}
		}
		Line line2 = new Line();
		line2.dist = dist;
		line2.azimuth = azimuth;
		line2.incl = incl;
		arrayList.Add(line2);
		return line2;
	}

	internal static Angle AziAngle(double a, int unit)
	{
		return unit switch
		{
			360 => new Angle(a, 360), 
			400 => new Angle(a, 400), 
			350 => new Angle(a + 180.0, 360), 
			390 => new Angle(a + 200.0, 400), 
			_ => int.MinValue, 
		};
	}

	internal static Angle InclAngle(double a, int unit, double length)
	{
		return unit switch
		{
			360 => new Angle(a, 360), 
			400 => new Angle(a, 400), 
			350 => new Angle(0.0 - a, 360), 
			390 => new Angle(0.0 - a, 400), 
			361 => new Angle(90.0 - a, 360), 
			401 => new Angle(100.0 - a, 400), 
			359 => new Angle(a - 90.0, 360), 
			399 => new Angle(a - 100.0, 400), 
			370 => new Angle(Math.Atan2(a, 100.0)), 
			380 => new Angle(Math.Asin(a / length)), 
			_ => int.MinValue, 
		};
	}
}
