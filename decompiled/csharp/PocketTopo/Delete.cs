using System.Drawing;
using System.IO;

namespace PocketTopo;

internal class Delete : Element
{
	private Element deleted;

	internal static void AddNew(Drawing list, Element e)
	{
		Delete delete = new Delete();
		delete.deleted = e;
		list.Add(delete);
		list.mapping.ctrl.Invalidate();
	}

	internal override int Distance(int x, int y)
	{
		deleted.hidden = true;
		return int.MaxValue;
	}

	internal override int Paint(Graphics g, Rectangle clip)
	{
		deleted.hidden = true;
		return 0;
	}

	internal override void Offset(int dx, int dy)
	{
		deleted.hidden = true;
	}

	internal override void Write(FileWriter w)
	{
		deleted.hidden = true;
	}

	internal override void Read(FileReader r)
	{
	}

	internal override void WriteDXF(DXFWriter w, int x0, int y0)
	{
		deleted.hidden = true;
	}

	internal override void WriteTherion(TextWriter w, int x0, int y0, int opt)
	{
		deleted.hidden = true;
	}
}
