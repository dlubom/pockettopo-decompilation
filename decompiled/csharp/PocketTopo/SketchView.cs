using System.Drawing;
using System.Windows.Forms;

namespace PocketTopo;

internal class SketchView : Control
{
	private const int UNDEF = int.MinValue;

	internal Survey survey;

	internal MainForm form;

	internal Pen pen;

	internal bool erase;

	internal bool offset;

	internal bool cornerMove;

	private Drawing list;

	private bool side;

	private Mapping map;

	private Polygon actualLine;

	private int mX = int.MinValue;

	private int mY;

	private bool moved;

	private bool first;

	internal Station tapped;

	private static Brush gray = new SolidBrush(Color.Gray);

	private static StringFormat format = new StringFormat();

	private Bitmap buffer;

	private Graphics bufferG;

	internal void Setup(Drawing list, Mapping map, bool side)
	{
		this.list = list;
		this.map = map;
		this.side = side;
		actualLine = null;
		format.Alignment = StringAlignment.Center;
		tapped = null;
		first = true;
	}

	internal void ResetFocus()
	{
		tapped = null;
		Invalidate();
	}

	private int PaintAll(Graphics g, Rectangle clip)
	{
		g.Clear(Color.White);
		if (Config.rasterVisible && map.BigScale)
		{
			int num = (int)Config.unit;
			if ((double)num <= 304.79999999999995)
			{
				num *= 5;
			}
			if (map.LargeScale)
			{
				num /= 5;
			}
			Rectangle w = default(Rectangle);
			map.Screen2World(clip, ref w);
			int i = w.X / num * num;
			int j = w.Y / num * num;
			int num2 = w.Right / num * num;
			int num3 = w.Bottom / num * num;
			int sx;
			int sy;
			for (; i <= num2; i += num)
			{
				map.World2Screen(i, j, out sx, out sy);
				g.DrawLine(MainForm.lgray, sx, clip.Top, sx, clip.Bottom);
			}
			for (; j <= num3; j += num)
			{
				map.World2Screen(i, j, out sx, out sy);
				g.DrawLine(MainForm.lgray, clip.Left, sy, clip.Right, sy);
			}
		}
		string text = ((tapped == null) ? map.ScaleText() : survey.StationInfo(tapped));
		int num4 = base.Height - (int)g.MeasureString(text, MainForm.textFont).Height - 1;
		g.DrawString(text, MainForm.textFont, MainForm.grayBrush, base.Width >> 1, num4, format);
		int num5 = ((!side) ? survey.PaintOutline(map, g, clip, Config.paintAll, Config.showSketchId, MainForm.shotRd, MainForm.shotXs, MainForm.shotOr, MainForm.shotOr) : survey.PaintSideview(map, g, clip, Config.showSketchId, MainForm.shotRd, MainForm.shotXs));
		survey.GetRef(out var x, out var y, out var h, out var d);
		if (side)
		{
			x = d;
			y = h;
		}
		return num5 + list.Paint(g, clip, x, y);
	}

	internal void Dump(string name)
	{
		Bitmap bitmap = new Bitmap(base.Width, base.Height);
		Graphics g = Graphics.FromImage(bitmap);
		PaintAll(g, new Rectangle(0, 0, base.Width, base.Height));
		MapView.WritePict(bitmap, name);
	}

	protected override void OnPaint(PaintEventArgs a)
	{
		if (a.ClipRectangle.Width < base.Width || a.ClipRectangle.Height < base.Height)
		{
			PaintAll(a.Graphics, a.ClipRectangle);
			return;
		}
		if (buffer == null || buffer.Width != base.Width || buffer.Height != base.Height)
		{
			if (buffer != null)
			{
				buffer.Dispose();
			}
			if (bufferG != null)
			{
				bufferG.Dispose();
			}
			buffer = new Bitmap(base.Width, base.Height);
			bufferG = Graphics.FromImage(buffer);
		}
		bufferG.Clip = new Region(a.ClipRectangle);
		int num = PaintAll(bufferG, a.ClipRectangle);
		a.Graphics.DrawImage(buffer, 0, 0);
		if (first && num == 0)
		{
			survey.GetRef(out var x, out var y, out var h, out var d);
			if (side)
			{
				x = d;
				y = h;
			}
			map.SetOrigin(x, y);
			Invalidate();
		}
		first = false;
	}

	protected override void OnMouseDown(MouseEventArgs a)
	{
		if (erase)
		{
			map.Screen2AltWorld(a.X, a.Y, out var wx, out var wy);
			Element element = list.Search(wx, wy);
			if (element != null)
			{
				Delete.AddNew(list, element);
			}
		}
		else if (pen != null)
		{
			actualLine = Polygon.AddNew(list, a.X, a.Y, pen);
		}
		else if (XSection.actual != null)
		{
			XSection.Add(list, a.X, a.Y);
			form.ReenableMove();
		}
		else
		{
			mX = a.X;
			mY = a.Y;
			moved = false;
		}
	}

	protected override void OnMouseMove(MouseEventArgs a)
	{
		if (mX == a.X && mY == a.Y)
		{
			return;
		}
		if (actualLine != null)
		{
			actualLine.AddPoint(a.X, a.Y);
		}
		else if (!erase && mX != int.MinValue)
		{
			if (offset)
			{
				list.Offset(a.X - mX, a.Y - mY);
			}
			else
			{
				map.MoveOrigin(mX - a.X, mY - a.Y);
			}
			mX = a.X;
			mY = a.Y;
			moved = true;
		}
	}

	protected override void OnMouseUp(MouseEventArgs a)
	{
		if (!erase && pen == null && mX != int.MinValue && !moved)
		{
			map.Screen2World(a.X, a.Y, out var wx, out var wy);
			if (side)
			{
				tapped = survey.NearestSide(wx, wy, map.Delta, tapped, null);
			}
			else
			{
				tapped = survey.Nearest(wx, wy, map.Delta, Config.paintAll, tapped, null);
			}
			Invalidate();
		}
		actualLine = null;
		mX = int.MinValue;
	}

	protected override void OnPaintBackground(PaintEventArgs a)
	{
	}

	internal void MoveMouseDown(object sender, MouseEventArgs a)
	{
		mX = a.X;
		mY = a.Y;
	}

	internal void MoveMouseMove(object sender, MouseEventArgs a)
	{
		if (mX != int.MinValue && (mX != a.X || mY != a.Y))
		{
			map.MoveOrigin(mX - a.X, mY - a.Y);
			mX = a.X;
			mY = a.Y;
			moved = true;
		}
	}

	internal void MoveMouseUp(object sender, MouseEventArgs a)
	{
		mX = int.MinValue;
	}
}
