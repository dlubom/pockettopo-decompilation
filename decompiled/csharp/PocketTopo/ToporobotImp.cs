using System;
using System.IO;
using System.Windows.Forms;

namespace PocketTopo;

internal class ToporobotImp : Importer
{
	public override void Install()
	{
		MenuItem menuItem = new MenuItem();
		menuItem.Text = "Toporobot...";
		menuItem.Click += menu_Click;
		AddMenuItem(menuItem);
	}

	private void menu_Click(object sender, EventArgs e)
	{
		if (SaveActual())
		{
			ToporobotForm toporobotForm = new ToporobotForm(null);
			if (toporobotForm.openFileDialog.ShowDialog() == DialogResult.OK)
			{
				Reset();
				ReadImport(toporobotForm.openFileDialog.FileName);
				Update();
			}
		}
	}

	private void AddXSections(Toporobot tr, Toporobot.Series s, int ser, int num, Survey survey)
	{
		ID f = new ID(ser, num);
		Toporobot.Station station = (Toporobot.Station)s.stations[num];
		Toporobot.Code code = (Toporobot.Code)tr.codes[station.code];
		short tr2 = (short)((Toporobot.Trip)tr.trips[station.trip]).idx;
		if (station.lines != null)
		{
			for (int i = 0; i < station.lines.Count; i++)
			{
				Toporobot.Line line = (Toporobot.Line)station.lines[i];
				Station s2 = new Station(f, ID.UNDEF, (int)Math.Round((double)line.dist * 1000.0), Toporobot.AziAngle(line.azimuth, code.aziUnit), Toporobot.InclAngle(line.incl, code.inclUnit, line.dist), 0, tr2);
				survey.AddStation(s2);
			}
			return;
		}
		Toporobot.Station station2 = null;
		if (num < s.length)
		{
			station2 = (Toporobot.Station)s.stations[num + 1];
		}
		Angle angle = new Angle(code.limit, 400);
		Angle angle2 = Toporobot.AziAngle(station.azimuth, code.aziUnit);
		Angle angle3 = Toporobot.InclAngle(station.incl, code.inclUnit, station.dist);
		if (station2 != null && station2.dist > 0f)
		{
			code = (Toporobot.Code)tr.codes[station2.code];
			Angle angle4 = Toporobot.AziAngle(station2.azimuth, code.aziUnit);
			Angle angle5 = Toporobot.InclAngle(station2.incl, code.inclUnit, station2.dist);
			if (station.dist > 0f)
			{
				angle2 = Angle.AngleBisector(angle2, angle4);
				angle3 = Angle.AngleBisector(angle3, angle5);
			}
			else
			{
				angle2 = angle4;
				angle3 = angle5;
			}
		}
		else if (station.dist == 0f)
		{
			return;
		}
		if (Math.Abs(angle3) < 16384 - (int)angle)
		{
			angle3 = 0;
		}
		if (station.left > 0f)
		{
			Station s2 = new Station(f, ID.UNDEF, (int)Math.Round((double)station.left * 1000.0), (int)angle2 - 16384, 0, 0, tr2);
			survey.AddStation(s2);
		}
		if (station.right > 0f)
		{
			Station s2 = new Station(f, ID.UNDEF, (int)Math.Round((double)station.right * 1000.0), (int)angle2 + 16384, 0, 0, tr2);
			survey.AddStation(s2);
		}
		if (station.up > 0f)
		{
			Station s2 = new Station(f, ID.UNDEF, (int)Math.Round((double)station.up * 1000.0), angle2, (int)angle3 + 16384, 0, tr2);
			survey.AddStation(s2);
		}
		if (station.down > 0f)
		{
			Station s2 = new Station(f, ID.UNDEF, (int)Math.Round((double)station.down * 1000.0), angle2, (int)angle3 - 16384, 0, tr2);
			survey.AddStation(s2);
		}
	}

	private Station NewBranch(Toporobot tr, int s1, int n1, int s2, int n2, double dist, double azimuth, double incl, int cx, int tx)
	{
		Angle d = 0;
		Angle i = 0;
		Toporobot.Code code = null;
		if (dist > 0.0)
		{
			code = (Toporobot.Code)tr.codes[cx];
			d = Toporobot.AziAngle(azimuth, code.aziUnit);
			i = Toporobot.InclAngle(incl, code.inclUnit, dist);
		}
		ID f = ((n1 < 0) ? ID.UNDEF : new ID(s1, n1));
		ID t = new ID(s2, n2);
		short tr2 = -1;
		if (tx >= 0)
		{
			tr2 = (short)((Toporobot.Trip)tr.trips[tx]).idx;
		}
		Station station = new Station(f, t, (int)Math.Round(dist * 1000.0), d, i, 0, tr2);
		if (dist > 0.0 && code.limit < 0f)
		{
			station.invisible = true;
		}
		return station;
	}

	private void GetData(Toporobot tr)
	{
		Survey survey = ActualSurvey();
		for (int i = 1; i < tr.refs.Count; i++)
		{
			if (tr.refs[i] != null)
			{
				Toporobot.Reference reference = (Toporobot.Reference)tr.refs[i];
				Reference reference2 = new Reference(new ID(reference.ser, reference.num), new MetricLocation((long)(reference.e * 1000.0), (long)(reference.n * 1000.0), (int)(reference.h * 1000.0)));
				reference2.comment = reference.comment;
				survey.AddReference(reference2);
			}
		}
		for (int i = 1; i < tr.trips.Count; i++)
		{
			if (tr.trips[i] != null)
			{
				Toporobot.Trip trip = (Toporobot.Trip)tr.trips[i];
				Trip trip2 = new Trip();
				trip2.Date = new DateTime(trip.year, (trip.month == 0) ? 1 : trip.month, (trip.day == 0) ? 1 : trip.day);
				string text = trip.name1 + "\r\n" + trip.name2;
				if (trip.comment != null)
				{
					text = text + "\r\n" + trip.comment;
				}
				trip2.Comment = text;
				trip2.AutoDecl = trip.auto;
				trip2.DeclCorr = new Angle(0f - trip.aziOffs, 400);
				trip.idx = survey.AddTrip(trip2);
			}
		}
		for (int i = 1; i < tr.series.Count; i++)
		{
			if (tr.series[i] != null)
			{
				Toporobot.Series series = (Toporobot.Series)tr.series[i];
				if (series.startSer != i || series.startNum != 0 || series.name != null)
				{
					Station station = NewBranch(tr, series.startSer, series.startNum, i, 0, 0.0, 0.0, 0.0, -1, -1);
					station.comment = series.name;
					survey.AddStation(station);
				}
				AddXSections(tr, series, i, 0, survey);
				for (int j = 1; j <= series.length; j++)
				{
					Toporobot.Station station2 = (Toporobot.Station)series.stations[j];
					Station station = NewBranch(tr, i, j - 1, i, j, station2.dist, station2.azimuth, station2.incl, station2.code, station2.trip);
					station.comment = station2.comment;
					survey.AddStation(station);
					AddXSections(tr, series, i, j, survey);
				}
				if (series.endSer != i || series.endNum != series.length)
				{
					Station station = NewBranch(tr, i, series.length, series.endSer, series.endNum, 0.0, 0.0, 0.0, -1, -1);
					survey.AddStation(station);
				}
			}
		}
	}

	internal bool ReadImport(string name)
	{
		try
		{
			Toporobot toporobot = null;
			using (FileStream stream = File.OpenRead(name))
			{
				using TextReader textReader = new StreamReader(stream, new MacEncoding());
				toporobot = Toporobot.Read(textReader);
				textReader.Close();
			}
			if (toporobot != null)
			{
				Open();
				GetData(toporobot);
				Close();
				return true;
			}
		}
		catch
		{
		}
		return false;
	}
}
