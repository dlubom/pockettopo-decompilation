using System;
using Microsoft.Win32;

namespace PocketTopo;

public class Config
{
	public const double METER = 1000.0;

	public const double FEET = 304.79999999999995;

	private static string actual;

	private static string port;

	private static bool title;

	private static bool raster;

	private static bool auto;

	private static bool smart;

	private static bool all;

	private static bool idP;

	private static bool idS;

	private static bool rev;

	private static bool metric;

	private static bool thick;

	private static int angle = 360;

	private static short decl = 0;

	private static string tComment;

	private static DateTime tDate;

	private static ID next = ID.UNDEF;

	public static string actualFile
	{
		get
		{
			return actual;
		}
		set
		{
			Set("Actual", actual = value);
		}
	}

	public static string portName
	{
		get
		{
			return port;
		}
		set
		{
			Set("Port", port = value);
		}
	}

	public static bool titleVisible
	{
		get
		{
			return title;
		}
		set
		{
			SetBool("Title", title = value);
		}
	}

	public static bool rasterVisible
	{
		get
		{
			return raster;
		}
		set
		{
			SetBool("Raster", raster = value);
		}
	}

	public static bool autoConnect
	{
		get
		{
			return auto;
		}
		set
		{
			SetBool("AutoBT", auto = value);
		}
	}

	public static bool smartMode
	{
		get
		{
			return smart;
		}
		set
		{
			SetBool("Smart", smart = value);
		}
	}

	public static bool paintAll
	{
		get
		{
			return all;
		}
		set
		{
			SetBool("PaintAll", all = value);
		}
	}

	public static bool showMapId
	{
		get
		{
			return idP;
		}
		set
		{
			SetBool("ShowMapId", idP = value);
		}
	}

	public static bool showSketchId
	{
		get
		{
			return idS;
		}
		set
		{
			SetBool("ShowSketchId", idS = value);
		}
	}

	public static bool reverse
	{
		get
		{
			return rev;
		}
		set
		{
			SetBool("Reverse", rev = value);
		}
	}

	public static bool thickLines
	{
		get
		{
			return thick;
		}
		set
		{
			SetBool("ThickLines", thick = value);
		}
	}

	public static int LineWidth => (!thick) ? 1 : 2;

	public static bool metricUnits
	{
		get
		{
			return metric;
		}
		set
		{
			Set("Unit", (metric = value) ? "m" : "feet");
		}
	}

	public static int fullCircle
	{
		get
		{
			return angle;
		}
		set
		{
			int num = (angle = value);
			Set("Angle", num.ToString());
		}
	}

	public static Angle declCorr
	{
		get
		{
			return decl;
		}
		set
		{
			short num = (decl = (short)(int)value);
			Set("DeclCorr", num.ToString());
		}
	}

	public static string tripComment
	{
		get
		{
			return tComment;
		}
		set
		{
			Set("TripComment", tComment = value);
		}
	}

	public static DateTime tripDate
	{
		get
		{
			return tDate;
		}
		set
		{
			DateTime dateTime = (tDate = value);
			Set("TripDate", dateTime.Ticks.ToString());
		}
	}

	public static ID nextId
	{
		get
		{
			return next;
		}
		set
		{
			ID iD = (next = value);
			Set("NextId", iD.ToString());
		}
	}

	public static double unit => metric ? 1000.0 : 304.79999999999995;

	public static char unitCh => metric ? 'm' : '\'';

	private static void Set(string name, string value)
	{
		RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("PocketTopo", writable: true);
		registryKey.SetValue(name, value);
		registryKey.Close();
	}

	private static void SetBool(string name, bool value)
	{
		Set(name, value ? "on" : "off");
	}

	internal static void Open()
	{
		RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("PocketTopo", writable: true);
		if (registryKey == null)
		{
			Registry.CurrentUser.CreateSubKey("PocketTopo");
			registryKey = Registry.CurrentUser.OpenSubKey("PocketTopo", writable: true);
		}
		port = (string)registryKey.GetValue("Port", "");
		metric = (string)registryKey.GetValue("Unit", "m") == "m";
		angle = int.Parse((string)registryKey.GetValue("Angle", "360"));
		actual = (string)registryKey.GetValue("Actual", "");
		title = (string)registryKey.GetValue("Title", "on") == "on";
		raster = (string)registryKey.GetValue("Raster", "on") == "on";
		auto = (string)registryKey.GetValue("AutoBT", "off") == "on";
		smart = (string)registryKey.GetValue("Smart", "on") == "on";
		all = (string)registryKey.GetValue("PaintAll", "on") == "on";
		idS = (string)registryKey.GetValue("ShowSketchId", "on") == "on";
		idP = (string)registryKey.GetValue("ShowMapId", "on") == "on";
		rev = (string)registryKey.GetValue("Reverse", "off") == "on";
		thick = (string)registryKey.GetValue("ThickLines", "off") == "on";
		next = new ID((string)registryKey.GetValue("NextId", "1.0"));
		string s = (string)registryKey.GetValue("DeclCorr", "0");
		try
		{
			decl = short.Parse(s);
		}
		catch
		{
			decl = (short)(int)new Angle(float.Parse(s), fullCircle);
		}
		tComment = (string)registryKey.GetValue("TripComment", "");
		long num = long.Parse((string)registryKey.GetValue("TripDate", "0"));
		if (num == 0)
		{
			tDate = DateTime.Today;
		}
		else
		{
			tDate = new DateTime(num);
		}
		registryKey.Close();
	}
}
