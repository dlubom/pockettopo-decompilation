using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace PocketTopo;

internal class VisualTopoExp : Exporter
{
	private TextWriter wr;

	private Station[] stations;

	public override void Install()
	{
		MenuItem menuItem = new MenuItem();
		menuItem.Text = "VisualTopo...";
		menuItem.Click += menu_Click;
		AddMenuItem(menuItem);
	}

	private void menu_Click(object sender, EventArgs e)
	{
		SaveFileDialog saveFileDialog = new SaveFileDialog();
		saveFileDialog.FileName = Path.GetFileNameWithoutExtension(Config.actualFile);
		saveFileDialog.Filter = "VisualTopo (tro)|*.tro";
		if (saveFileDialog.ShowDialog() == DialogResult.OK)
		{
			WriteExport(saveFileDialog.FileName);
		}
	}

	private void WriteXSection(Angle azimuth, Angle incl, Station lines)
	{
		int num = 0;
		if ((int)azimuth != int.MinValue)
		{
			Angle.ToRect(1000, incl, out var x, out var y);
			Angle.ToRect(y, azimuth, out var x2, out var y2);
			int num2 = 500000;
			Station station = null;
			for (Station station2 = lines; station2 != null; station2 = station2.next)
			{
				Angle.ToRect(1000, station2.incl, out var x3, out var y3);
				Angle.ToRect(y3, station2.azimuth, out var x4, out var y4);
				int num3 = x4 * x2 + y4 * y2 + x3 * x;
				if (num3 > num2)
				{
					num2 = num3;
					station = station2;
				}
			}
			if (station != null)
			{
				num = (int)((long)station.dist * (long)num2 / 1000000);
			}
		}
		wr.Write(string.Format(Exporter.inv, "{0,6:f2} ", (double)(float)num / 1000.0));
	}

	private void WriteXSections(Station d, int i)
	{
		Station station = null;
		if (i < stations.Length - 1)
		{
			Station station2 = stations[i + 1];
			if (station2.num == d.num.Next())
			{
				station = station2;
			}
		}
		int num = int.MinValue;
		int num2 = int.MinValue;
		if (d.dist > 0)
		{
			num = d.FwAzimuth;
		}
		if (station != null && station.dist > 0)
		{
			num2 = station.FwAzimuth;
		}
		Angle angle = Angle.AngleBisector(num, num2);
		WriteXSection((int)angle - 16384, 0, d.cross);
		WriteXSection((int)angle + 16384, 0, d.cross);
		WriteXSection(angle, 16384, d.cross);
		WriteXSection(angle, -16384, d.cross);
	}

	private void WriteId(ID id)
	{
		wr.Write(id.SeriesString() + '_' + id.Station());
	}

	private void WriteParam(Angle decl)
	{
		double num = decl.ToUnit(360);
		wr.Write("Param Deca ");
		wr.Write((Config.fullCircle == 360) ? "Degd" : "Gra");
		wr.Write(" Clino ");
		wr.Write((Config.fullCircle == 360) ? "Degd" : "Gra");
		if (num > 0.0)
		{
			wr.Write('-');
		}
		else
		{
			num = 0.0 - num;
		}
		int num2 = (int)num;
		num = (num - (double)num2) * 60.0;
		int num3 = (int)num;
		num = (num - (double)num3) * 60.0;
		int num4 = (int)num;
		wr.Write(" {0:d}.{1:d2}{2:d2} ", new object[3] { num2, num3, num4 });
		wr.WriteLine("Dir,Dir,Dir Inc Std");
	}

	private void WriteData(Station s, int idx)
	{
		ID iD = ((s.num == s.to) ? s.from : s.to);
		wr.Write(iD.ToString().Replace('.', '_').PadRight(10));
		wr.Write(' ');
		wr.Write(s.num.ToString().Replace('.', '_').PadRight(10));
		wr.Write("           ");
		s.GetAvDirection(out var dist, out var azimuth, out var incl, rev: false);
		wr.Write(string.Format(Exporter.inv, "{0,9:f2} ", (double)dist / 1000.0));
		wr.Write(string.Format(Exporter.inv, "{0,7:f2} ", azimuth * (float)Config.fullCircle / ((float)Math.PI * 2f)));
		wr.Write(string.Format(Exporter.inv, "{0,7:f2} ", incl * (float)Config.fullCircle / ((float)Math.PI * 2f)));
		WriteXSections(s, idx);
		bool flag = false;
		if (s.start != null)
		{
			flag = s.start.flipped;
		}
		if (s.flipped != flag)
		{
			wr.Write("I ");
		}
		else
		{
			wr.Write("N ");
		}
		wr.Write("I ");
		if (s.comment != null)
		{
			wr.Write(';');
			wr.Write(s.comment.Trim().Replace('\r', ' ').Replace('\n', ' ')
				.Replace(';', ','));
			wr.Write(';');
		}
		wr.WriteLine();
	}

	private void WriteList()
	{
		stations = ActualSurvey().GetStations(inclTemplate: false);
		int num = stations.Length;
		wr.WriteLine("Version 5.02");
		Reference reference = null;
		for (int i = 0; i < num; i++)
		{
			if (stations[i] is Reference && (reference == null || stations[i].idx < reference.idx))
			{
				reference = (Reference)stations[i];
			}
		}
		if (reference != null)
		{
			wr.Write("Trou ");
			if (reference.comment != null)
			{
				wr.Write(reference.comment);
			}
			else
			{
				wr.Write('?');
			}
			wr.Write(',');
			wr.Write(string.Format(Exporter.inv, "{0:f3},", (double)reference.loc.e / 1000000.0));
			wr.Write(string.Format(Exporter.inv, "{0:f3},", (double)reference.loc.n / 1000000.0));
			wr.Write("{0:d},", reference.loc.alt / 1000);
			wr.WriteLine("SWISS");
			wr.Write("Entree ");
			wr.WriteLine(reference.num.ToString().Replace('.', '_'));
		}
		Angle angle = int.MinValue;
		for (int i = 0; i < num; i++)
		{
			Station station = stations[i];
			if (!(station is Reference))
			{
				Angle angle2 = 0;
				Trip trip = Trip.ByIndex(station.trip);
				if (trip == null && i < num - 1)
				{
					trip = Trip.ByIndex(stations[i + 1].trip);
				}
				if (trip != null)
				{
					angle2 = trip.Declination;
				}
				if ((int)angle2 != (int)angle)
				{
					WriteParam(angle2);
					angle = angle2;
				}
				WriteData(station, i);
			}
		}
	}

	internal void WriteExport(string name)
	{
		try
		{
			using FileStream stream = File.Open(name, FileMode.Create);
			using TextWriter textWriter = new StreamWriter(stream, Encoding.Default);
			wr = textWriter;
			WriteList();
			textWriter.Close();
		}
		catch
		{
		}
		wr = null;
		stations = null;
	}
}
