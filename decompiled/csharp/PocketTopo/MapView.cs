using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;

namespace PocketTopo;

internal class MapView : Control
{
	private const int UNDEF = int.MinValue;

	internal Survey survey;

	internal Mapping map;

	private static StringFormat format = new StringFormat();

	private int mX = int.MinValue;

	private int mY;

	private bool moved;

	private bool first;

	internal Station tapped;

	private Bitmap buffer;

	internal void Setup()
	{
		format.Alignment = StringAlignment.Center;
		tapped = null;
		first = true;
	}

	private int PaintAll(Graphics g, Rectangle clip)
	{
		g.Clear(Color.White);
		string directoryName;
		try
		{
			directoryName = Path.GetDirectoryName(Config.actualFile);
			int num = directoryName.LastIndexOf(Path.DirectorySeparatorChar);
			directoryName = ((num < 0) ? "" : (directoryName.Substring(num + 1) + ": "));
			directoryName += Path.GetFileNameWithoutExtension(Config.actualFile);
			g.DrawString(directoryName, MainForm.textFont, MainForm.grayBrush, base.Width >> 1, 1f, format);
		}
		catch
		{
		}
		if (tapped != null)
		{
			directoryName = survey.StationInfo(tapped);
		}
		else
		{
			directoryName = "L = " + (int)((double)survey.TotLength / Config.unit);
			directoryName = directoryName + "(" + (int)((double)survey.ActLength / Config.unit);
			directoryName = directoryName + ")" + Config.unitCh;
			directoryName += "    ";
			directoryName += map.ScaleText();
			directoryName += "    H = ";
			directoryName += (int)((double)survey.TotHeight / Config.unit);
			directoryName = directoryName + "(" + (int)((double)survey.ActHeight / Config.unit);
			directoryName = directoryName + ")" + Config.unitCh;
		}
		int num2 = base.Height - (int)g.MeasureString(directoryName, MainForm.textFont).Height - 1;
		g.DrawString(directoryName, MainForm.textFont, MainForm.grayBrush, base.Width >> 1, num2, format);
		return survey.PaintOutline(map, g, clip, all: true, Config.showMapId, MainForm.shotRd, null, MainForm.black, null);
	}

	internal static void WritePict(Bitmap pict, string name)
	{
		string text = Path.GetExtension(name).ToLower();
		ImageFormat imageFormat = ImageFormat.Bmp;
		if (text == ".gif")
		{
			imageFormat = ImageFormat.Gif;
		}
		if (text == ".png")
		{
			imageFormat = ImageFormat.Png;
		}
		Image.FromHbitmap(pict.GetHbitmap()).Save(name, imageFormat);
	}

	internal void Dump(string name)
	{
		Bitmap bitmap = new Bitmap(base.Width, base.Height);
		Graphics g = Graphics.FromImage(bitmap);
		PaintAll(g, new Rectangle(0, 0, base.Width, base.Height));
		WritePict(bitmap, name);
	}

	protected override void OnPaint(PaintEventArgs a)
	{
		if (buffer == null || buffer.Width != base.Width || buffer.Height != base.Height)
		{
			if (buffer != null)
			{
				buffer.Dispose();
			}
			buffer = new Bitmap(base.Width, base.Height);
		}
		Graphics graphics = Graphics.FromImage(buffer);
		survey.Update();
		graphics.Clip = new Region(a.ClipRectangle);
		int num = PaintAll(graphics, a.ClipRectangle);
		a.Graphics.DrawImage(buffer, 0, 0);
		if (first && num == 0)
		{
			survey.GetRef(out var x, out var y, out var _, out var _);
			map.SetOrigin(x, y);
			Invalidate();
		}
		first = false;
	}

	protected override void OnMouseDown(MouseEventArgs a)
	{
		mX = a.X;
		mY = a.Y;
		moved = false;
	}

	protected override void OnMouseMove(MouseEventArgs a)
	{
		if (mX != int.MinValue)
		{
			map.MoveOrigin(mX - a.X, mY - a.Y);
			mX = a.X;
			mY = a.Y;
			moved = true;
		}
	}

	protected override void OnMouseUp(MouseEventArgs a)
	{
		if (mX != int.MinValue && !moved)
		{
			map.Screen2World(a.X, a.Y, out var wx, out var wy);
			tapped = survey.Nearest(wx, wy, map.Delta, all: true, tapped, null);
			Invalidate();
		}
		mX = int.MinValue;
	}

	protected override void OnPaintBackground(PaintEventArgs a)
	{
	}
}
