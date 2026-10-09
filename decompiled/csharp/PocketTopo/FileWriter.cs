using System.IO;

namespace PocketTopo;

internal class FileWriter
{
	internal BinaryWriter w;

	internal FileWriter(BinaryWriter w)
	{
		this.w = w;
	}
}
