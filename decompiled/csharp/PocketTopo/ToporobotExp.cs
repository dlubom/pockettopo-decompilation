using System;
using System.Collections;
using System.IO;
using System.Windows.Forms;

namespace PocketTopo;

internal class ToporobotExp : Exporter
{
	internal static char[] eol = new char[2] { '\r', '\n' };

	public override void Install()
	{
		MenuItem menuItem = new MenuItem();
		menuItem.Text = "Toporobot...";
		menuItem.Click += menu_Click;
		AddMenuItem(menuItem);
	}

	private void menu_Click(object sender, EventArgs e)
	{
		Form form = new ToporobotForm(this);
		form.ShowDialog();
	}

	private float XSection(Angle azimuth, Angle incl, Station lines)
	{
		Angle.ToRect(100, incl, out var x, out var y);
		Angle.ToRect(y, azimuth, out var x2, out var y2);
		int num = 5000;
		Station station = null;
		for (Station station2 = lines; station2 != null; station2 = station2.next)
		{
			Angle.ToRect(100, station2.incl, out var x3, out var y3);
			Angle.ToRect(y3, station2.azimuth, out var x4, out var y4);
			int num2 = x4 * x2 + y4 * y2 + x3 * x;
			if (num2 > num)
			{
				num = num2;
				station = station2;
			}
		}
		int num3 = 0;
		if (station != null)
		{
			num3 = station.dist * num / 10000;
		}
		return (float)((double)num3 / 1000.0);
	}

	private void AddXSections(Toporobot.Station tst, Station d, Station next, bool vertical)
	{
		Angle a = int.MinValue;
		Angle a2 = int.MinValue;
		Angle a3 = int.MinValue;
		Angle a4 = int.MinValue;
		if (d.dist > 0)
		{
			a = d.FwIncl;
			a2 = d.FwAzimuth;
		}
		if (next != null && next.dist > 0)
		{
			a3 = next.FwIncl;
			a4 = next.FwAzimuth;
		}
		else if (d.dist == 0)
		{
			return;
		}
		Angle angle = Angle.AngleBisector(a2, a4);
		Angle angle2 = 0;
		if (!vertical)
		{
			angle2 = Angle.AngleBisector(a, a3);
		}
		tst.left = XSection((int)angle - 16384, 0, d.cross);
		tst.right = XSection((int)angle + 16384, 0, d.cross);
		tst.up = XSection(angle, (int)angle2 + 16384, d.cross);
		tst.down = XSection(angle, (int)angle2 - 16384, d.cross);
	}

	private void AddInline(Toporobot tr, Toporobot.Station tst, Station s)
	{
		for (Station cross = s.cross; cross != null; cross = cross.next)
		{
			tr.AddLine(tst, (float)cross.dist / 1000f, (float)((cross.azimuth & 0xFFFF) * Config.fullCircle) / 65536f, (float)(cross.incl * Config.fullCircle) / 65536f);
		}
	}

	private void AddToporobot(Toporobot tr, string cname, Survey data, int code, int trip, bool auto, bool manual, bool include, bool vertical)
	{
		Station[] stations = data.GetStations(inclTemplate: false);
		int num = stations.Length;
		for (int i = 0; i < num; i++)
		{
			if (stations[i] is Reference)
			{
				Reference reference = (Reference)stations[i];
				Toporobot.Reference reference2 = tr.AddReference(reference.num.Series(), reference.num.Station());
				reference2.e = (double)reference.loc.e / 1000.0;
				reference2.n = (double)reference.loc.n / 1000.0;
				reference2.h = (double)reference.loc.alt / 1000.0;
				reference2.name = "";
				if (cname != "")
				{
					reference2.name = cname;
				}
				cname = "";
				if (reference.comment != null)
				{
					reference2.comment = reference.comment;
				}
			}
		}
		ArrayList arrayList = new ArrayList();
		for (int i = 0; i < num; i++)
		{
			Trip trip2 = Trip.ByIndex(stations[i].trip);
			if (trip2 != null && arrayList.IndexOf(trip2) < 0)
			{
				arrayList.Add(trip2);
			}
		}
		int[] array = new int[arrayList.Count];
		while (tr.trips.Count < trip)
		{
			tr.trips.Add(null);
		}
		for (int i = 0; i < arrayList.Count; i++)
		{
			Trip trip2 = (Trip)arrayList[i];
			DateTime date = trip2.Date;
			string text = trip2.Comment;
			string[] array2 = new string[2] { "", "" };
			int num2 = 0;
			while (num2 < 2 && text != "")
			{
				int num3 = text.IndexOfAny(eol);
				int num4 = num3 + 1;
				if (num3 < 0)
				{
					num4 = text.Length;
				}
				string text2 = text.Substring(0, num4).Trim();
				text = text.Substring(num4);
				if (text2 != "")
				{
					array2[num2++] = text2;
				}
			}
			Toporobot.Trip trip3 = tr.AddTrip(date.Day, date.Month, date.Year, array2[0], array2[1]);
			if (auto || (trip2.AutoDecl && !manual))
			{
				trip3.auto = true;
				trip3.aziOffs = 0f;
			}
			else
			{
				trip3.auto = false;
				trip3.aziOffs = (float)((int)trip2.Declination * -400) / 65536f;
			}
			trip3.inclOffs = 0f;
			trip3.color = 1;
			if (text != "")
			{
				trip3.comment = text;
			}
			array[i] = trip3.idx;
		}
		while (tr.codes.Count < code)
		{
			tr.codes.Add(null);
		}
		Toporobot.Code code2 = tr.AddCode(Config.fullCircle, Config.fullCircle, 0.05f, 1f, 1f, 100f, (!vertical) ? 100 : 0);
		int num5 = -1;
		for (int i = 0; i < num; i++)
		{
			int trip4 = trip;
			Station station = stations[i];
			ID iD;
			if (station is Reference)
			{
				iD = ID.UNDEF;
			}
			else
			{
				Trip trip2 = Trip.ByIndex(station.trip);
				if (trip2 != null)
				{
					trip4 = array[arrayList.IndexOf(trip2)];
				}
				iD = ((!(station.num == station.to)) ? station.to : station.from);
			}
			int num6 = station.num.Series();
			int num7 = station.num.Station();
			int num8 = iD.Series();
			int num9 = iD.Station();
			Toporobot.Series series;
			if (num6 != num8 && num6 == num5 && num7 > 1 && iD != ID.UNDEF)
			{
				int num2;
				for (num2 = i + 1; num2 < num && stations[num2].num.Series() == num6; num2++)
				{
				}
				int num10 = stations[num2 - 1].num.Station();
				if (num7 == num10)
				{
					series = tr.AddSeries(num6);
					series.endSer = num8;
					series.endNum = num9;
				}
				continue;
			}
			series = tr.AddSeries(num6);
			Toporobot.Station station2 = null;
			if (num6 != num8 || (num6 != num5 && num9 == 0))
			{
				if (num7 == 0 && !(station is Reference) && station.comment != null)
				{
					series.name = station.comment;
				}
				if (iD == ID.UNDEF)
				{
					series.startSer = num6;
					series.startNum = 0;
				}
				else
				{
					series.startSer = num8;
					series.startNum = num9;
				}
				int num2;
				for (num2 = i + 1; num2 < num && stations[num2].num.Series() == num6; num2++)
				{
				}
				int num10 = stations[num2 - 1].num.Station();
				series.endSer = num6;
				series.endNum = num10;
				series.length = num10;
				series.chance = 3;
				series.obst = 0;
				station2 = tr.AddStation(num6, 0);
				station2.code = code2.idx;
				station2.trip = trip4;
				if (num7 != 0)
				{
					if (station.start != null)
					{
						AddXSections(station2, station.start, station, vertical);
						if (include)
						{
							AddInline(tr, station2, station.start);
						}
					}
					num8 = num6;
					num9 = 0;
				}
			}
			if (num6 == num8 && num7 != num9)
			{
				station2 = tr.AddStation(num6, num7);
				station2.code = code2.idx;
				station2.trip = trip4;
				station.GetAvDirection(out var dist, out var azimuth, out var incl, rev: false);
				station2.dist = dist / 1000f;
				station2.azimuth = azimuth * (float)Config.fullCircle / ((float)Math.PI * 2f);
				station2.incl = incl * (float)Config.fullCircle / ((float)Math.PI * 2f);
			}
			Station station3 = null;
			if (i < num - 1)
			{
				Station station4 = stations[i + 1];
				if (station4.num == station.num.Next())
				{
					station3 = station4;
				}
			}
			if (station2 != null)
			{
				AddXSections(station2, station, station3, vertical);
				if (num7 != 0 && station.comment != null)
				{
					station2.comment = station.comment;
				}
				if (include)
				{
					AddInline(tr, station2, station);
				}
			}
			num5 = num6;
		}
	}

	internal void WriteExport(string inname, string outname, int code, int trip, bool auto, bool manual, bool include, bool vertical)
	{
		Survey data = ActualSurvey();
		try
		{
			Toporobot toporobot = null;
			string cname = "";
			if (inname != null)
			{
				using FileStream stream = File.OpenRead(inname);
				using TextReader textReader = new StreamReader(stream, new MacEncoding());
				toporobot = Toporobot.Read(textReader);
				textReader.Close();
			}
			using FileStream stream2 = File.Open(outname, FileMode.Create);
			using TextWriter textWriter = new StreamWriter(stream2, new MacEncoding());
			if (toporobot == null)
			{
				toporobot = new Toporobot();
				cname = Path.GetFileNameWithoutExtension(outname);
			}
			AddToporobot(toporobot, cname, data, code, trip, auto, manual, include, vertical);
			toporobot.Write(textWriter);
			textWriter.Close();
		}
		catch
		{
		}
	}
}
