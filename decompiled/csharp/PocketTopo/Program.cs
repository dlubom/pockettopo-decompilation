using System;
using System.Windows.Forms;

namespace PocketTopo;

public static class Program
{
	[STAThread]
	public static void Main()
	{
		Form mainForm = new MainForm();
		while (true)
		{
			try
			{
				Application.Run(mainForm);
				break;
			}
			catch
			{
			}
		}
	}
}
