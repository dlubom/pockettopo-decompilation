using System.IO;

namespace PocketTopo;

internal class Therion
{
	private static void WriteThSketch(TextWriter w, Survey survey, Drawing list, bool side)
	{
		survey.GetRef(out var x, out var y, out var h, out var d);
		if (side)
		{
			x = d;
			y = h;
		}
		survey.WriteThSketch1(w, side);
		list.WriteTherion(w, x, y, 1);
		survey.WriteThSketch2(w, side);
		list.WriteTherion(w, x, y, 2);
		list.WriteTherion(w, x, y, 0);
	}

	internal static void WriteExport(string name, Survey survey, Drawing outline, Drawing sideview)
	{
		try
		{
			using FileStream stream = File.Open(name, FileMode.Create);
			using TextWriter textWriter = new StreamWriter(stream);
			survey.WriteTherion(textWriter);
			textWriter.WriteLine();
			textWriter.WriteLine("PLAN");
			WriteThSketch(textWriter, survey, outline, side: false);
			textWriter.WriteLine();
			textWriter.WriteLine("ELEVATION");
			WriteThSketch(textWriter, survey, sideview, side: true);
			textWriter.Close();
		}
		catch
		{
		}
	}
}
