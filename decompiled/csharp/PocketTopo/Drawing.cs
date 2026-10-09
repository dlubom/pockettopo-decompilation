using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace PocketTopo;

internal class Drawing
{
	private Element elems;

	private Element removed;

	private Element limit;

	internal Mapping mapping;

	internal bool side;

	internal event EventHandler UndoChanged;

	internal Drawing(Mapping map, bool side)
	{
		mapping = map;
		this.side = side;
	}

	internal void Reset(bool resetMapping)
	{
		elems = null;
		limit = null;
		removed = null;
		this.UndoChanged(null, EventArgs.Empty);
		if (resetMapping)
		{
			mapping.Reset();
		}
		if (mapping.ctrl != null)
		{
			mapping.ctrl.Invalidate();
		}
	}

	internal Element Search(int x, int y)
	{
		int num = mapping.Delta;
		Element result = null;
		for (Element next = elems; next != null; next = next.next)
		{
			if (next.hidden)
			{
				next.hidden = false;
			}
			else
			{
				int num2 = next.Distance(x, y);
				if (num2 < num)
				{
					num = num2;
					result = next;
				}
			}
		}
		return result;
	}

	internal int Paint(Graphics g, Rectangle clip, int x0, int y0)
	{
		clip.X -= 2;
		clip.Y -= 2;
		clip.Width += 4;
		clip.Height += 4;
		mapping.SetAltOffset(-x0, -y0);
		int num = 0;
		Rectangle w = default(Rectangle);
		mapping.Screen2AltWorld(clip, ref w);
		for (Element next = elems; next != null; next = next.next)
		{
			if (next.hidden)
			{
				next.hidden = false;
			}
			else
			{
				num += next.Paint(g, w);
			}
		}
		return num;
	}

	internal void Offset(int dx, int dy)
	{
		dx *= mapping.ScaleFactor();
		dy *= mapping.ScaleFactor();
		for (Element next = elems; next != null; next = next.next)
		{
			if (next.hidden)
			{
				next.hidden = false;
			}
			else
			{
				next.Offset(dx, dy);
			}
		}
		mapping.ctrl.Invalidate();
	}

	internal void Add(Element e)
	{
		e.next = elems;
		elems = e;
		removed = null;
		e.list = this;
		this.UndoChanged(e, EventArgs.Empty);
	}

	internal bool CanUndo()
	{
		return elems != null && elems != limit;
	}

	internal bool CanRedo()
	{
		return removed != null;
	}

	internal void Undo(Control ctrl)
	{
		Element element = elems;
		if (element != null && element != limit)
		{
			elems = element.next;
			element.next = removed;
			removed = element;
			this.UndoChanged(element, EventArgs.Empty);
			ctrl?.Invalidate();
		}
	}

	internal void Redo(Control ctrl)
	{
		Element element = removed;
		if (element != null)
		{
			removed = element.next;
			element.next = elems;
			elems = element;
			this.UndoChanged(element, EventArgs.Empty);
			ctrl?.Invalidate();
		}
	}

	internal void Write(FileWriter w)
	{
		mapping.Write(w);
		for (Element next = elems; next != null; next = next.next)
		{
			if (next.hidden)
			{
				next.hidden = false;
			}
			else
			{
				next.Write(w);
			}
		}
		w.w.Write((byte)0);
	}

	internal void Read(FileReader r)
	{
		if (r.version >= 3)
		{
			mapping.Read(r);
		}
		else
		{
			mapping.Reset();
		}
		elems = null;
		Element element = null;
		for (int num = r.r.ReadByte(); num > 0; num = r.r.ReadByte())
		{
			int count = 0;
			Element element2 = null;
			switch (num)
			{
			case 1:
				element2 = new Polygon();
				break;
			case 3:
				element2 = new XSection();
				break;
			}
			if (element2 != null)
			{
				element2.Read(r);
				if (element == null)
				{
					elems = element2;
				}
				else
				{
					element.next = element2;
				}
				element = element2;
				element2.list = this;
			}
			else
			{
				r.r.ReadBytes(count);
			}
		}
		limit = elems;
		removed = null;
	}

	internal void WriteTherion(TextWriter w, int x0, int y0, int opt)
	{
		for (Element next = elems; next != null; next = next.next)
		{
			if (next.hidden)
			{
				next.hidden = false;
			}
			else
			{
				next.WriteTherion(w, x0, y0, opt);
			}
		}
	}

	internal void WriteDXF(DXFWriter w, int x0, int y0)
	{
		for (Element next = elems; next != null; next = next.next)
		{
			if (next.hidden)
			{
				next.hidden = false;
			}
			else
			{
				next.WriteDXF(w, x0, y0);
			}
		}
	}

	internal static void SetupPen(Pen pen)
	{
		if (pen != null && pen.Width != (float)Config.LineWidth)
		{
			pen.Width = Config.LineWidth;
		}
	}
}
