using System.Drawing;
using System.Windows.Forms;

namespace PocketTopo;

internal class Mapping
{
	private static int PixPerMm = 5;

	private static int delta = 8;

	internal Control ctrl;

	private int x0;

	private int y0;

	private int x1;

	private int y1;

	private int mx;

	private int my;

	private int scaleFact;

	internal bool BigScale => scaleFact * PixPerMm < 1000;

	internal bool LargeScale => scaleFact * PixPerMm < 100;

	internal int Delta => delta * scaleFact;

	internal Mapping(Control ctrl)
	{
		this.ctrl = ctrl;
		Reset();
	}

	internal static void SetVga()
	{
		PixPerMm = 10;
		delta = 16;
	}

	internal string ScaleText()
	{
		return "1:" + scaleFact * PixPerMm;
	}

	internal void Prepare()
	{
		mx = ctrl.Width >> 1;
		my = ctrl.Height >> 1;
	}

	internal void Reset()
	{
		x0 = 0;
		y0 = 0;
		scaleFact = 500 / PixPerMm;
		Prepare();
	}

	internal void SetOrigin(int x, int y)
	{
		x0 = x;
		y0 = y;
	}

	internal void MoveOrigin(int dx, int dy)
	{
		x0 += dx * scaleFact;
		y0 += dy * scaleFact;
		ctrl.Invalidate();
	}

	internal void SetAltOffset(int dx, int dy)
	{
		x1 = x0 + dx;
		y1 = y0 + dy;
	}

	internal void Larger()
	{
		int num = scaleFact * PixPerMm;
		if (num >= 50000)
		{
			scaleFact = 20000 / PixPerMm;
		}
		else if (num >= 20000)
		{
			scaleFact = 10000 / PixPerMm;
		}
		else if (num >= 10000)
		{
			scaleFact = 5000 / PixPerMm;
		}
		else if (num >= 5000)
		{
			scaleFact = 3000 / PixPerMm;
		}
		else if (num >= 3000)
		{
			scaleFact = 2000 / PixPerMm;
		}
		else if (num >= 2000)
		{
			scaleFact = 1500 / PixPerMm;
		}
		else if (num >= 1500)
		{
			scaleFact = 1000 / PixPerMm;
		}
		else if (num >= 1000)
		{
			scaleFact = 750 / PixPerMm;
		}
		else if (num >= 750)
		{
			scaleFact = 500 / PixPerMm;
		}
		else if (num >= 500)
		{
			scaleFact = 300 / PixPerMm;
		}
		else if (num >= 300)
		{
			scaleFact = 200 / PixPerMm;
		}
		else if (num >= 200)
		{
			scaleFact = 150 / PixPerMm;
		}
		else if (num >= 150)
		{
			scaleFact = 100 / PixPerMm;
		}
		else if (num >= 100)
		{
			scaleFact = 75 / PixPerMm;
		}
		else if (num >= 70)
		{
			scaleFact = 50 / PixPerMm;
		}
		else if (num >= 50)
		{
			scaleFact = 30 / PixPerMm;
		}
		else if (num >= 30)
		{
			scaleFact = 20 / PixPerMm;
		}
		else if (num >= 20)
		{
			scaleFact = 15 / PixPerMm;
		}
		else
		{
			scaleFact = 10 / PixPerMm;
		}
		ctrl.Invalidate();
	}

	internal void Smaller()
	{
		int num = scaleFact * PixPerMm;
		if (num <= 10 && PixPerMm < 8)
		{
			scaleFact = 15 / PixPerMm;
		}
		else if (num <= 15)
		{
			scaleFact = 20 / PixPerMm;
		}
		else if (num <= 20)
		{
			scaleFact = 30 / PixPerMm;
		}
		else if (num <= 30)
		{
			scaleFact = 50 / PixPerMm;
		}
		else if (num <= 50)
		{
			scaleFact = 75 / PixPerMm;
		}
		else if (num <= 75)
		{
			scaleFact = 100 / PixPerMm;
		}
		else if (num <= 100)
		{
			scaleFact = 150 / PixPerMm;
		}
		else if (num <= 150)
		{
			scaleFact = 200 / PixPerMm;
		}
		else if (num <= 200)
		{
			scaleFact = 300 / PixPerMm;
		}
		else if (num <= 300)
		{
			scaleFact = 500 / PixPerMm;
		}
		else if (num <= 500)
		{
			scaleFact = 750 / PixPerMm;
		}
		else if (num <= 750)
		{
			scaleFact = 1000 / PixPerMm;
		}
		else if (num <= 1000)
		{
			scaleFact = 1500 / PixPerMm;
		}
		else if (num <= 1500)
		{
			scaleFact = 2000 / PixPerMm;
		}
		else if (num <= 2000)
		{
			scaleFact = 3000 / PixPerMm;
		}
		else if (num <= 3000)
		{
			scaleFact = 5000 / PixPerMm;
		}
		else if (num <= 5000)
		{
			scaleFact = 10000 / PixPerMm;
		}
		else if (num <= 10000)
		{
			scaleFact = 20000 / PixPerMm;
		}
		else
		{
			scaleFact = 50000 / PixPerMm;
		}
		ctrl.Invalidate();
	}

	internal int ScaleFactor()
	{
		return scaleFact;
	}

	internal void World2Screen(int wx, int wy, out int sx, out int sy)
	{
		sx = mx + (wx - x0) / scaleFact;
		sy = my + (wy - y0) / scaleFact;
	}

	internal void World2Screen(Rectangle w, ref Rectangle s)
	{
		s.X = mx + (w.X - x0) / scaleFact;
		s.Y = my + (w.Y - y0) / scaleFact;
		s.Width = w.Width / scaleFact;
		s.Height = w.Height / scaleFact;
	}

	internal void AltWorld2Screen(int wx, int wy, out int sx, out int sy)
	{
		sx = mx + (wx - x1) / scaleFact;
		sy = my + (wy - y1) / scaleFact;
	}

	internal void AltWorld2Screen(Rectangle w, ref Rectangle s)
	{
		s.X = mx + (w.X - x1) / scaleFact;
		s.Y = my + (w.Y - y1) / scaleFact;
		s.Width = w.Width / scaleFact;
		s.Height = w.Height / scaleFact;
	}

	internal void Screen2World(int sx, int sy, out int wx, out int wy)
	{
		wx = (sx - mx) * scaleFact + x0;
		wy = (sy - my) * scaleFact + y0;
	}

	internal void Screen2World(Rectangle s, ref Rectangle w)
	{
		w.X = (s.X - mx) * scaleFact + x0;
		w.Y = (s.Y - my) * scaleFact + y0;
		w.Width = s.Width * scaleFact;
		w.Height = s.Height * scaleFact;
	}

	internal void Screen2AltWorld(int sx, int sy, out int wx, out int wy)
	{
		wx = (sx - mx) * scaleFact + x1;
		wy = (sy - my) * scaleFact + y1;
	}

	internal void Screen2AltWorld(Rectangle s, ref Rectangle w)
	{
		w.X = (s.X - mx) * scaleFact + x1;
		w.Y = (s.Y - my) * scaleFact + y1;
		w.Width = s.Width * scaleFact;
		w.Height = s.Height * scaleFact;
	}

	internal void Write(FileWriter w)
	{
		w.w.Write(x0);
		w.w.Write(y0);
		w.w.Write(scaleFact * PixPerMm);
	}

	internal void Read(FileReader r)
	{
		x0 = r.r.ReadInt32();
		y0 = r.r.ReadInt32();
		scaleFact = r.r.ReadInt32() / PixPerMm;
	}
}
