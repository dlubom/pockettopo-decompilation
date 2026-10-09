using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace PocketTopo;

internal class VisualTopoImp : Importer
{
	private CultureInfo inv = CultureInfo.InvariantCulture;

	private Survey survey;

	private TextReader rd;

	private string line;

	private int idx;

	private double corr;

	private Angle decl;

	private int aUnit;

	private int iUnit;

	private bool aInv;

	private bool iInv;

	private bool xInv;

	private Trip trip;

	private Reference refc;

	private ID prevId;

	private double prevDist;

	private Angle prevAzim;

	private double left;

	private double right;

	private double up;

	private double down;

	private double actHeight;

	public override void Install()
	{
		MenuItem menuItem = new MenuItem();
		menuItem.Text = "VisualTopo...";
		menuItem.Click += menu_Click;
		AddMenuItem(menuItem);
	}

	private void menu_Click(object sender, EventArgs e)
	{
		if (SaveActual())
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Filter = "VisualTopo (tro)|*.tro|*.*|*.*";
			if (openFileDialog.ShowDialog() == DialogResult.OK)
			{
				Reset();
				ReadImport(openFileDialog.FileName);
				Update();
			}
		}
	}

	private string NextField()
	{
		while (idx < line.Length && line[idx] <= ' ')
		{
			idx++;
		}
		int num = idx;
		while (idx < line.Length && line[idx] > ' ')
		{
			idx++;
		}
		return line.Substring(num, idx - num);
	}

	private ID ParseId(string str)
	{
		for (int i = 0; i < str.Length; i++)
		{
			int num = str[i] & 0xDF;
			if (num >= 65 && num <= 90)
			{
				num -= 64;
				str = str.Substring(0, i) + (char)(48 + num / 10) + (char)(48 + num % 10) + str.Substring(i + 1);
				i++;
			}
		}
		int num2 = str.IndexOf('.');
		if (num2 < 0)
		{
			str = str.Replace('_', '.');
		}
		return new ID(str);
	}

	private Angle ParseAngle(string str, int unit, double dist)
	{
		if (unit < 0)
		{
			return 16384 - (int)ParseAngle(str, -unit, dist);
		}
		double num = double.Parse(str, inv);
		switch (unit)
		{
		case 60:
		{
			int num3 = (int)num;
			num = (num - (double)num3) * 100.0;
			int num4 = (int)num;
			num = (num - (double)num4) * 100.0;
			int num5 = (int)num;
			return new Angle((double)num3 + ((double)num4 + (double)num5 / 60.0) / 60.0, 360);
		}
		case 1:
			return new Angle(Math.Atan2(num, 100.0));
		case 2:
			return new Angle(Math.Asin(num / dist));
		case 3:
		{
			double num2 = actHeight;
			actHeight = num;
			return new Angle(Math.Asin((num - num2) / dist));
		}
		default:
			return new Angle(num, unit);
		}
	}

	private double ParseXSection(string str)
	{
		if (str == "*")
		{
			return 0.0;
		}
		return double.Parse(str, inv);
	}

	private void XSections(ID id, double dist, Angle azim, double nDist, Angle nAzi)
	{
		if (dist == 0.0)
		{
			azim = int.MinValue;
		}
		if (nDist == 0.0)
		{
			nAzi = int.MinValue;
		}
		azim = Angle.AngleBisector(azim, nAzi);
		if ((int)azim != int.MinValue)
		{
			if (left > 0.0)
			{
				Station s = new Station(id, ID.UNDEF, (int)(left * 1000.0), (int)azim - 16384, 0, 0, trip.Idx());
				survey.AddStation(s);
			}
			if (right > 0.0)
			{
				Station s = new Station(id, ID.UNDEF, (int)(right * 1000.0), (int)azim + 16384, 0, 0, trip.Idx());
				survey.AddStation(s);
			}
			if (up > 0.0)
			{
				Station s = new Station(id, ID.UNDEF, (int)(up * 1000.0), azim, 16384, 0, trip.Idx());
				survey.AddStation(s);
			}
			if (down > 0.0)
			{
				Station s = new Station(id, ID.UNDEF, (int)(down * 1000.0), azim, -16384, 0, trip.Idx());
				survey.AddStation(s);
			}
		}
	}

	private bool ReadLine()
	{
		line = rd.ReadLine();
		idx = 0;
		if (line == null)
		{
			return false;
		}
		if (line == "")
		{
			return true;
		}
		string text = NextField();
		if (text == "Param")
		{
			text = NextField();
			if (text == "Topof")
			{
				corr = double.Parse(NextField(), inv) / 100.0;
			}
			else
			{
				corr = 0.0;
			}
			text = NextField();
			if (text == "Degd")
			{
				aUnit = 360;
			}
			else if (text == "Gra")
			{
				aUnit = 400;
			}
			else
			{
				aUnit = 60;
			}
			text = NextField();
			if (text == "Deniv")
			{
				iUnit = 2;
			}
			else if (text == "Prof")
			{
				iUnit = 3;
			}
			else
			{
				switch (NextField())
				{
				case "Degd":
					iUnit = 360;
					break;
				case "Gra":
					iUnit = 400;
					break;
				case "%":
					iUnit = 1;
					break;
				default:
					iUnit = 60;
					break;
				}
				if (text == "Vulc")
				{
					iUnit = -iUnit;
				}
			}
			decl = -(int)ParseAngle(NextField(), 60, 0.0);
			text = NextField();
			if (text.Length == 11)
			{
				aInv = text.Substring(0, 3) == "Inv";
				iInv = text.Substring(4, 3) == "Inv";
				xInv = text.Substring(8, 3) == "Inv";
			}
			if (trip == null || (int)trip.DeclCorr != (int)decl)
			{
				trip = new Trip();
				trip.Date = DateTime.Today;
				trip.DeclCorr = decl;
				Trip.AddTrip(trip);
			}
			actHeight = 0.0;
			return true;
		}
		if (text == "[Configuration")
		{
			return false;
		}
		if (trip == null)
		{
			if (text == "Trou")
			{
				string[] array = line.Substring(idx).Split(',');
				string text2 = array[0].Trim();
				if (text2 != "")
				{
					refc.comment = text2;
				}
				refc.loc.e = (long)(double.Parse(array[1], inv) * 1000000.0);
				refc.loc.n = (long)(double.Parse(array[2], inv) * 1000000.0);
				refc.loc.alt = int.Parse(array[3]) * 1000;
			}
			else if (text == "Entree")
			{
				refc.num = ParseId(NextField());
			}
			return true;
		}
		ID iD = ((!(text == "*")) ? ParseId(text) : prevId);
		text = NextField();
		ID t = ParseId(text);
		text = NextField();
		double num;
		if (corr == 0.0)
		{
			num = double.Parse(text, inv);
		}
		else
		{
			num = double.Parse(NextField(), inv) - double.Parse(text, inv);
			num *= corr;
		}
		Angle angle = ParseAngle(NextField(), aUnit, 0.0);
		Angle angle2 = ParseAngle(NextField(), iUnit, num);
		if (aInv)
		{
			angle = (int)angle + 32768;
		}
		if (iInv)
		{
			angle2 = -(int)angle2;
		}
		if (iD == prevId)
		{
			XSections(prevId, prevDist, prevAzim, num, angle);
		}
		else
		{
			XSections(prevId, prevDist, prevAzim, 0.0, int.MinValue);
		}
		prevId = t;
		prevDist = num;
		prevAzim = angle;
		left = ParseXSection(NextField());
		right = ParseXSection(NextField());
		up = ParseXSection(NextField());
		down = ParseXSection(NextField());
		if (xInv)
		{
			double num2 = left;
			left = right;
			right = num2;
		}
		Station station = new Station(iD, t, (int)(num * 1000.0), angle, angle2, 0, trip.Idx());
		survey.AddStation(station);
		NextField();
		NextField();
		string[] array2 = line.Split(';');
		if (array2.Length >= 2 && array2[1] != "")
		{
			station.comment = array2[1];
		}
		return true;
	}

	internal void ReadImport(string name)
	{
		survey = ActualSurvey();
		try
		{
			using (FileStream stream = File.OpenRead(name))
			{
				using TextReader textReader = new StreamReader(stream, Encoding.Default);
				rd = textReader;
				Open();
				trip = null;
				refc = new Reference(ID.DEFAULT, new MetricLocation(0L, 0L, 0));
				survey.AddReference(refc);
				prevDist = 0.0;
				while (ReadLine())
				{
				}
				textReader.Close();
				XSections(prevId, prevDist, prevAzim, 0.0, int.MinValue);
			}
			Close();
		}
		catch
		{
		}
		rd = null;
		trip = null;
		refc = null;
		survey = null;
		line = null;
	}
}
