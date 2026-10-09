using Microsoft.WindowsCE.Forms;

namespace PocketTopo;

internal class Input
{
	private static InputPanel inputPanel = new InputPanel();

	internal static int Enable()
	{
		inputPanel.Enabled = true;
		return inputPanel.VisibleDesktop.Height;
	}

	internal static void Disable()
	{
		inputPanel.Enabled = false;
	}
}
