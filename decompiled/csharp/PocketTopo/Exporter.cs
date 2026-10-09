using System.Globalization;
using System.IO;
using System.Windows.Forms;

namespace PocketTopo;

public abstract class Exporter : Extension
{
	internal static CultureInfo inv = CultureInfo.InvariantCulture;

	internal static MainForm main;

	internal static MenuItem menu;

	public void AddMenuItem(MenuItem item)
	{
		menu.MenuItems.Add(item);
	}

	public Survey ActualSurvey()
	{
		return main.dataSet.survey;
	}

	internal static void WriteText(string name, Survey data)
	{
		try
		{
			using FileStream stream = File.Open(name, FileMode.Create);
			using TextWriter textWriter = new StreamWriter(stream);
			textWriter.Write(Path.GetFileNameWithoutExtension(Config.actualFile));
			textWriter.Write("   (");
			if (Config.unit == 1000.0)
			{
				textWriter.Write("m");
			}
			else if (Config.unit == 304.79999999999995)
			{
				textWriter.Write("feet");
			}
			else
			{
				textWriter.Write(Config.unit.ToString(inv));
			}
			textWriter.WriteLine(", {0:d})", Config.fullCircle);
			textWriter.WriteLine();
			data.WriteText(textWriter);
			textWriter.Close();
		}
		catch
		{
		}
	}
}
