using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;

namespace PocketTopo;

internal class DXFWriter
{
	internal enum Options
	{
		NONE = 0,
		SIDE = 1,
		ENS = 2,
		ENX = 4,
		ENL = 8,
		ENA = 0x10,
		SEPS = 0x20,
		SEPX = 0x40,
		SEPL = 0x80,
		SEPC = 0x100,
		GEO = 0x200,
		ENG = 0x400,
		SEPG = 0x800
	}

	internal int scale;

	internal Options opt;

	internal TextWriter w;

	private Survey survey;

	internal void DxfLayer(Pen pen)
	{
		w.WriteLine(8);
		string value = "Sketch";
		if (pen == MainForm.shotRd || pen == MainForm.shotOr)
		{
			if ((opt & (Options)96) != Options.NONE)
			{
				value = "Shots";
			}
		}
		else if (pen == MainForm.shotXs || pen == MainForm.shotDa || pen == MainForm.shotXOr)
		{
			if ((opt & Options.SEPX) != Options.NONE)
			{
				value = "XSect";
			}
			else if ((opt & Options.SEPS) != Options.NONE)
			{
				value = "Shots";
			}
		}
		else if (pen == MainForm.lgray)
		{
			if ((opt & Options.SEPG) != Options.NONE)
			{
				value = "Grid";
			}
		}
		else if ((opt & Options.SEPC) != Options.NONE)
		{
			value = ((pen == MainForm.black) ? "Black" : ((pen == MainForm.red) ? "Red" : ((pen == MainForm.blue) ? "Blue" : ((pen == MainForm.green) ? "Green" : ((pen == MainForm.gray) ? "Gray" : ((pen == MainForm.brown) ? "Brown" : ((pen != MainForm.orange) ? "LGray" : "Orange")))))));
		}
		w.WriteLine(value);
	}

	internal void DxfColor(Pen pen)
	{
		w.WriteLine(62);
		int value = ((pen != MainForm.black) ? ((pen == MainForm.red || pen == MainForm.shotRd || pen == MainForm.shotXs || pen == MainForm.shotDa) ? 1 : ((pen == MainForm.orange || pen == MainForm.shotOr || pen == MainForm.shotXOr) ? 30 : ((pen == MainForm.blue) ? 5 : ((pen == MainForm.green) ? 3 : ((pen == MainForm.gray) ? 9 : ((pen != MainForm.brown) ? 255 : 32)))))) : 0);
		w.WriteLine(value);
		if (pen.DashStyle == DashStyle.Dash)
		{
			w.WriteLine(6);
			w.WriteLine("DASH");
		}
	}

	internal void DxfCoord(Point p, int index, int x0, int y0)
	{
		double num = p.X + x0;
		double num2 = -(p.Y + y0);
		if ((opt & Options.GEO) != Options.NONE)
		{
			num += (double)survey.refBase.loc.e;
			num2 += (double)survey.refBase.loc.n;
		}
		w.WriteLine(10 + index);
		w.WriteLine((num / (double)scale).ToString(Exporter.inv));
		w.WriteLine(20 + index);
		w.WriteLine((num2 / (double)scale).ToString(Exporter.inv));
	}

	internal void DxfLine(Point from, Point to, Pen pen, int x0f, int y0f, int x0t, int y0t)
	{
		w.WriteLine(0);
		w.WriteLine("LINE");
		DxfLayer(pen);
		DxfColor(pen);
		DxfCoord(from, 0, x0f, y0f);
		DxfCoord(to, 1, x0t, y0t);
	}

	internal void DxfPoint(Point p, Pen pen, int x0, int y0)
	{
		w.WriteLine(0);
		w.WriteLine("POINT");
		DxfLayer(pen);
		DxfColor(pen);
		DxfCoord(p, 0, x0, y0);
	}

	public void DxfCircle(Point p, Pen pen, int x0, int y0)
	{
		w.WriteLine(0);
		w.WriteLine("CIRCLE");
		DxfLayer(pen);
		DxfColor(pen);
		DxfCoord(p, 0, x0, y0);
		w.WriteLine(40);
		w.WriteLine((50.0 / (double)scale).ToString(Exporter.inv));
	}

	internal void DxfPolygon(Pen pen)
	{
		w.WriteLine(0);
		w.WriteLine("POLYLINE");
		w.WriteLine(66);
		w.WriteLine(1);
		DxfLayer(pen);
		DxfColor(pen);
	}

	internal void DxfVertex(Pen pen, Point p, int x0, int y0)
	{
		w.WriteLine(0);
		w.WriteLine("VERTEX");
		DxfLayer(pen);
		DxfCoord(p, 0, x0, y0);
	}

	internal void DxfEnd()
	{
		w.WriteLine(0);
		w.WriteLine("SEQEND");
	}

	internal void DxfText(Point p, string str, int x0, int y0)
	{
		w.WriteLine(0);
		w.WriteLine("TEXT");
		w.WriteLine(8);
		if ((opt & Options.SEPL) != Options.NONE)
		{
			w.WriteLine("Label");
		}
		else if ((opt & Options.SEPS) != Options.NONE)
		{
			w.WriteLine("Shots");
		}
		else
		{
			w.WriteLine("Sketch");
		}
		w.WriteLine(40);
		w.WriteLine(1);
		w.WriteLine(1);
		w.WriteLine(str);
		DxfCoord(p, 0, x0, y0);
	}

	internal static void WriteDXF(string name, Survey survey, Drawing list, int scale, Options opt)
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
			textWriter.WriteLine(9);
			textWriter.WriteLine("$INSUNITS");
			textWriter.WriteLine(70);
			textWriter.WriteLine(4);
			textWriter.WriteLine(0);
			textWriter.WriteLine("ENDSEC");
			textWriter.WriteLine(0);
			textWriter.WriteLine("SECTION");
			textWriter.WriteLine(2);
			textWriter.WriteLine("TABLES");
			textWriter.WriteLine(0);
			textWriter.WriteLine("TABLE");
			textWriter.WriteLine(2);
			textWriter.WriteLine("LTYPE");
			textWriter.WriteLine(70);
			textWriter.WriteLine(1);
			textWriter.WriteLine(0);
			textWriter.WriteLine("LTYPE");
			textWriter.WriteLine(2);
			textWriter.WriteLine("DASH");
			textWriter.WriteLine(70);
			textWriter.WriteLine(0);
			textWriter.WriteLine(3);
			textWriter.WriteLine("---");
			textWriter.WriteLine(72);
			textWriter.WriteLine(65);
			textWriter.WriteLine(73);
			textWriter.WriteLine(2);
			textWriter.WriteLine(40);
			textWriter.WriteLine(0.6);
			textWriter.WriteLine(49);
			textWriter.WriteLine(0.3);
			textWriter.WriteLine(49);
			textWriter.WriteLine(-0.3);
			textWriter.WriteLine(0);
			textWriter.WriteLine("ENDTAB");
			textWriter.WriteLine(0);
			textWriter.WriteLine("ENDSEC");
			textWriter.WriteLine(0);
			textWriter.WriteLine("SECTION");
			textWriter.WriteLine(2);
			textWriter.WriteLine("ENTITIES");
			DXFWriter dXFWriter = new DXFWriter();
			dXFWriter.w = textWriter;
			dXFWriter.opt = opt;
			dXFWriter.scale = scale;
			dXFWriter.survey = survey;
			survey.GetRef(out var x, out var y, out var h, out var d);
			if ((opt & Options.SIDE) != Options.NONE)
			{
				x = d;
				y = h;
			}
			list.WriteDXF(dXFWriter, x, y);
			survey.WriteDXF(dXFWriter);
			if ((opt & Options.ENG) != Options.NONE)
			{
				survey.WriteDXFGrid(dXFWriter);
			}
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
