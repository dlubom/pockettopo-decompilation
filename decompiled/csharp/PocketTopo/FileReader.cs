using System.IO;

namespace PocketTopo;

internal class FileReader
{
	internal BinaryReader r;

	internal int version;

	internal FileReader(BinaryReader r, int version)
	{
		this.r = r;
		this.version = version;
	}
}
