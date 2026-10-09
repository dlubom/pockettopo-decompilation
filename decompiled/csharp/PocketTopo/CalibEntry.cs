using System;
using System.Globalization;
using System.IO;

namespace PocketTopo;

internal class CalibEntry
{
	internal const int UNDEF = int.MinValue;

	private static CultureInfo inv = CultureInfo.InvariantCulture;

	internal short gx;

	internal short gy;

	internal short gz;

	internal short mx;

	internal short my;

	internal short mz;

	internal float azimuth;

	internal float incl;

	internal float roll;

	internal float glen;

	internal float mlen;

	internal float alpha;

	internal float delta;

	internal bool valid;

	internal bool evaluated;

	internal byte group;

	public string Valid
	{
		get
		{
			return valid ? "*" : "";
		}
		set
		{
			valid = value != "" && value != " ";
		}
	}

	public string Valid2
	{
		get
		{
			return valid ? "*" : "";
		}
		set
		{
			valid = value != "" && value != " ";
		}
	}

	public string Group
	{
		get
		{
			if (group == 1)
			{
				return "A";
			}
			if (group == 2)
			{
				return "B";
			}
			return " ";
		}
		set
		{
			int num;
			switch (value)
			{
			default:
				num = 0;
				break;
			case "B":
			case "b":
				num = 2;
				break;
			case "A":
			case "a":
				num = 1;
				break;
			}
			group = (byte)num;
		}
	}

	public string Gx => GetSens(gx);

	public string Gy => GetSens(gy);

	public string Gz => GetSens(gz);

	public string Mx => GetSens(mx);

	public string My => GetSens(my);

	public string Mz => GetSens(mz);

	public string Azimuth => GetAng(azimuth);

	public string Incl => GetAng(incl);

	public string Roll => GetAng(roll);

	public string GLen
	{
		get
		{
			if (!evaluated)
			{
				return "";
			}
			return glen.ToString("f3");
		}
	}

	public string MLen
	{
		get
		{
			if (!evaluated)
			{
				return "";
			}
			return mlen.ToString("f3");
		}
	}

	public string Alpha => GetAng(alpha);

	public string Delta
	{
		get
		{
			if (!evaluated || !valid)
			{
				return "";
			}
			return delta.ToString("f2");
		}
	}

	internal CalibEntry()
	{
		glen = (mlen = 1f);
		alpha = (float)Math.PI / 2f;
		valid = true;
		evaluated = false;
	}

	private string GetAng(float val)
	{
		if (!evaluated)
		{
			return "";
		}
		return ((double)(val * (float)Config.fullCircle) / (Math.PI * 2.0)).ToString("f1");
	}

	private string GetSens(int val)
	{
		return val switch
		{
			32767 => "+++", 
			-32768 => "---", 
			_ => val.ToString(), 
		};
	}

	internal void ToggleValid()
	{
		valid = !valid;
		Calibrate.dirty = true;
	}

	internal void IncGroup()
	{
		group = (byte)((group + 1) % 3);
		Calibrate.dirty = true;
	}

	internal void Read(BinaryReader r, int version)
	{
		gx = r.ReadInt16();
		gy = r.ReadInt16();
		gz = r.ReadInt16();
		mx = r.ReadInt16();
		my = r.ReadInt16();
		mz = r.ReadInt16();
		group = r.ReadByte();
		valid = r.ReadBoolean();
		evaluated = false;
	}

	internal void Write(BinaryWriter w)
	{
		w.Write(gx);
		w.Write(gy);
		w.Write(gz);
		w.Write(mx);
		w.Write(my);
		w.Write(mz);
		w.Write(group);
		w.Write(valid);
	}

	private void WriteAngle(TextWriter w, float val)
	{
		w.Write(string.Format(Exporter.inv, "{0,7:f1}", (double)(val * (float)Config.fullCircle) / (Math.PI * 2.0)));
	}

	internal void WriteText(TextWriter w)
	{
		w.Write(Group);
		w.Write("{0,7:d}", gx);
		w.Write("{0,7:d}", gy);
		w.Write("{0,7:d}", gz);
		w.Write("{0,7:d}", mx);
		w.Write("{0,7:d}", my);
		w.Write("{0,7:d}", mz);
		if (evaluated)
		{
			WriteAngle(w, azimuth);
			WriteAngle(w, incl);
			WriteAngle(w, roll);
			w.Write(string.Format(Exporter.inv, "{0,7:f3}", glen));
			w.Write(string.Format(Exporter.inv, "{0,7:f3}", mlen));
			WriteAngle(w, alpha);
			if (valid)
			{
				w.Write(string.Format(Exporter.inv, "{0,7:f2}", delta));
			}
		}
		w.WriteLine();
	}
}
