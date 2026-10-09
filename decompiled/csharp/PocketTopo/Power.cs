using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace PocketTopo;

internal static class Power
{
	private struct MsgQOptions
	{
		internal uint dwSize;

		internal uint dwFlags;

		internal uint dwMaxMessages;

		internal uint cbMaxMessage;

		internal bool bReadAccess;
	}

	private static IntPtr ptr = IntPtr.Zero;

	private static Thread t = null;

	private static bool done = false;

	private static bool open = false;

	private static bool init = false;

	[DllImport("coredll.dll")]
	private static extern IntPtr RequestPowerNotifications(IntPtr hMsgQ, uint Flags);

	[DllImport("coredll.dll")]
	private static extern uint WaitForSingleObject(IntPtr hHandle, int wait);

	[DllImport("coredll.dll")]
	private static extern IntPtr CreateMsgQueue(string name, ref MsgQOptions options);

	[DllImport("coredll.dll")]
	private static extern bool ReadMsgQueue(IntPtr hMsgQ, byte[] lpBuffer, uint cbBufSize, ref uint lpNumRead, int dwTimeout, ref uint pdwFlags);

	internal static void Start()
	{
		try
		{
			MsgQOptions options = default(MsgQOptions);
			options.dwFlags = 0u;
			options.dwMaxMessages = 20u;
			options.cbMaxMessage = 256u;
			options.bReadAccess = true;
			options.dwSize = (uint)Marshal.SizeOf(options);
			ptr = CreateMsgQueue("Test", ref options);
			RequestPowerNotifications(ptr, 1u);
			t = new Thread(DoWork);
			t.Start();
			init = true;
		}
		catch
		{
		}
	}

	internal static void Stop()
	{
		if (init)
		{
			done = true;
			t.Abort();
		}
	}

	private static void DoWork()
	{
		byte[] array = new byte[256];
		uint lpNumRead = 0u;
		uint pdwFlags = 0u;
		uint num = 0u;
		while (!done)
		{
			if (WaitForSingleObject(ptr, 1000) == 0)
			{
				ReadMsgQueue(ptr, array, (uint)array.Length, ref lpNumRead, -1, ref pdwFlags);
				if ((array[6] & 0x22) != 0)
				{
					Link.Close();
				}
			}
			if (open)
			{
				Link.Open();
				open = false;
			}
		}
	}

	internal static void Open()
	{
		if (init)
		{
			open = true;
		}
		else
		{
			Link.Open();
		}
	}
}
