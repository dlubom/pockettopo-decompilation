using System.Drawing;
using System.IO;

namespace PocketTopo;

internal abstract class Element
{
	internal Element next;

	internal Drawing list;

	internal bool hidden;

	internal abstract int Distance(int x, int y);

	internal abstract int Paint(Graphics g, Rectangle clip);

	internal abstract void Offset(int dx, int dy);

	internal abstract void Write(FileWriter w);

	internal abstract void Read(FileReader r);

	internal abstract void WriteDXF(DXFWriter w, int x0, int y0);

	internal abstract void WriteTherion(TextWriter w, int x0, int y0, int opt);
}
