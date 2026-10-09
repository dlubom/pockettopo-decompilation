using System;
using System.IO;
using System.Windows.Forms;

namespace PocketTopo;

internal class DataSet
{
	private const string backDir = "backup";

	internal Survey survey;

	internal Mapping mapMap;

	internal Drawing outline;

	internal Drawing sideview;

	private static int dirty;

	private static int timeout;

	private static string back = "~back1.top";

	private static string back2 = "~back2.top";

	private static string hiddenBack = Path.Combine("backup", back);

	private static string hiddenBack2 = Path.Combine("backup", back2);

	internal static bool Dirty => dirty > 0;

	internal DataSet(Survey survey, Mapping mapMap, Drawing outline, Drawing sideview)
	{
		this.survey = survey;
		this.mapMap = mapMap;
		this.outline = outline;
		this.sideview = sideview;
	}

	internal static void Reset()
	{
		dirty = 0;
		timeout = 0;
		if (File.Exists(back))
		{
			try
			{
				if (!Directory.Exists("backup"))
				{
					Directory.CreateDirectory("backup");
				}
				if (File.Exists(hiddenBack))
				{
					File.Delete(hiddenBack);
				}
				File.Move(back, hiddenBack);
			}
			catch
			{
			}
		}
		if (!File.Exists(back2))
		{
			return;
		}
		try
		{
			if (!Directory.Exists("backup"))
			{
				Directory.CreateDirectory("backup");
			}
			if (File.Exists(hiddenBack2))
			{
				File.Delete(hiddenBack2);
			}
			File.Move(back2, hiddenBack2);
		}
		catch
		{
		}
	}

	internal static void MakeDirty()
	{
		dirty = 7;
	}

	internal static string ActualDirectory()
	{
		if (Config.actualFile == "")
		{
			return "";
		}
		return Path.GetDirectoryName(Config.actualFile);
	}

	internal void Write(string name)
	{
		using FileStream output = File.Open(name, FileMode.Create);
		using BinaryWriter binaryWriter = new BinaryWriter(output);
		binaryWriter.Write((byte)84);
		binaryWriter.Write((byte)111);
		binaryWriter.Write((byte)112);
		binaryWriter.Write((byte)3);
		FileWriter w = new FileWriter(binaryWriter);
		survey.Write(w);
		mapMap.Write(w);
		outline.Write(w);
		sideview.Write(w);
		binaryWriter.Write(0);
		binaryWriter.Close();
	}

	internal bool Read(string name, bool template)
	{
		bool result = false;
		try
		{
			using FileStream input = File.OpenRead(name);
			using BinaryReader binaryReader = new BinaryReader(input);
			int num = binaryReader.ReadInt32();
			int num2 = num >> 24;
			FileReader r = new FileReader(binaryReader, num2);
			if ((num & 0xFFFFFF) == 7368532 && num2 <= 3)
			{
				survey.Read(r, template);
				if (!template)
				{
					if (num2 >= 3)
					{
						mapMap.Read(r);
					}
					else
					{
						mapMap.Reset();
					}
					outline.Read(r);
					sideview.Read(r);
				}
				result = true;
			}
			binaryReader.Close();
		}
		catch
		{
		}
		return result;
	}

	internal bool Load(string name)
	{
		Cursor.Current = Cursors.WaitCursor;
		survey.Reset();
		Trip.Reset();
		string[] array = null;
		try
		{
			array = Directory.GetFiles(ActualDirectory(), "*.top");
		}
		catch
		{
		}
		if (array != null)
		{
			Array.Sort(array);
			for (int i = 0; i < array.Length; i++)
			{
				if (Path.GetFileName(array[i]) != Path.GetFileName(Config.actualFile))
				{
					Read(array[i], template: true);
				}
			}
		}
		survey.SetBoundary();
		bool result = true;
		if (name != "")
		{
			result = Read(name, template: false);
		}
		Cursor.Current = null;
		return result;
	}

	internal void CheckBackup()
	{
		if (dirty <= 1)
		{
			return;
		}
		timeout++;
		if (timeout == 30 && (dirty & 2) != 0)
		{
			try
			{
				Write(back);
			}
			catch
			{
			}
			dirty -= 2;
		}
		if (timeout != 60)
		{
			return;
		}
		if ((dirty & 4) != 0)
		{
			if (Directory.Exists("\\Storage Card"))
			{
				try
				{
					Write(Path.Combine("\\Storage Card", back));
				}
				catch
				{
				}
			}
			else if (Directory.Exists("\\SD-Karte"))
			{
				try
				{
					Write(Path.Combine("\\SD-Karte", back));
				}
				catch
				{
				}
			}
			dirty -= 4;
		}
		string text = back;
		back = back2;
		back2 = text;
		timeout = 0;
	}

	internal void LoadBackup()
	{
		Load(Config.actualFile);
		if (!File.Exists(back) || (File.Exists(back2) && File.GetLastWriteTime(back2) > File.GetLastWriteTime(back)))
		{
			string text = back;
			back = back2;
			back2 = text;
		}
		if (File.Exists(back))
		{
			if (!Load(back) && File.Exists(back2))
			{
				Load(back2);
			}
			dirty = 1;
			timeout = 0;
		}
	}
}
