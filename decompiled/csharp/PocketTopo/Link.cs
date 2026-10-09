using System;
using System.IO.Ports;

namespace PocketTopo;

internal static class Link
{
	internal static bool appActive;

	private static SerialPort port;

	private static byte[] ack;

	private static byte[] input;

	private static int timer;

	private static byte type0;

	private static int dist0;

	private static short azi0;

	private static short incl0;

	private static int address;

	private static int index;

	private static int timeout;

	private static byte[] actData;

	private static bool write;

	internal static event EventHandler ProgressEvent;

	static Link()
	{
		appActive = true;
		ack = new byte[2] { 85, 213 };
		input = new byte[8];
		timer = 0;
		type0 = 0;
	}

	internal static void Open()
	{
		string portName = Config.portName;
		if (!(portName != ""))
		{
			return;
		}
		if (port == null || port.PortName != portName)
		{
			Cleanup();
			port = new SerialPort(portName);
		}
		if (!port.IsOpen)
		{
			try
			{
				port.Open();
			}
			catch
			{
			}
		}
	}

	internal static void Close()
	{
		if (port != null && port.IsOpen)
		{
			try
			{
				port.Close();
			}
			catch
			{
			}
		}
		timer = 0;
	}

	internal static void Cleanup()
	{
		Close();
		if (port != null)
		{
			port.Dispose();
			port = null;
		}
	}

	internal static void Reconnect()
	{
		Close();
		Power.Open();
	}

	internal static bool IsOpen()
	{
		return port != null && port.IsOpen;
	}

	internal static bool IsConnected()
	{
		try
		{
			return port != null && port.IsOpen && port.DsrHolding;
		}
		catch
		{
			return false;
		}
	}

	internal static bool Poll(out int dist, out short azimuth, out short incl, out byte roll)
	{
		dist = 0;
		azimuth = 0;
		incl = 0;
		roll = 0;
		try
		{
			timer++;
			if (timer >= 12)
			{
				if (appActive && Config.autoConnect && !IsConnected())
				{
					Reconnect();
				}
				timer = 0;
			}
			if (IsOpen())
			{
				if (port.BytesToRead >= 8)
				{
					port.Read(input, 0, 8);
					byte b = input[0];
					if (b == 56 && actData != null && input[1] + (input[2] << 8) == address)
					{
						if (!write || (input[3] == actData[index] && input[4] == actData[index + 1] && input[5] == actData[index + 2] && input[6] == actData[index + 3]))
						{
							actData[index] = input[3];
							actData[index + 1] = input[4];
							actData[index + 2] = input[5];
							actData[index + 3] = input[6];
							index += 4;
							address += 4;
							Link.ProgressEvent(null, EventArgs.Empty);
							if (index < actData.Length)
							{
								Transfer(address, actData, index);
								timeout = 12;
							}
							else
							{
								actData = null;
							}
						}
					}
					else
					{
						dist = input[1] + (input[2] << 8) + ((b & 0x40) << 10);
						azimuth = (short)(input[3] + (input[4] << 8));
						incl = (short)(input[5] + (input[6] << 8));
						roll = input[7];
						port.Write(ack, b >> 7, 1);
						if (b != type0 || dist != dist0 || azimuth != azi0 || incl != incl0)
						{
							dist0 = dist;
							azi0 = azimuth;
							incl0 = incl;
							type0 = b;
							switch (b & 0x3F)
							{
							case 1:
								if (dist > 100000)
								{
									dist = (dist - 90000) * 10;
								}
								return true;
							case 2:
								Calibrate.AddG((short)dist, azimuth, incl);
								break;
							case 3:
								Calibrate.AddM((short)dist, azimuth, incl);
								Calibrate.Evaluate(evaluate: false);
								break;
							}
						}
					}
				}
				if (actData != null)
				{
					timeout--;
					if (timeout == 0)
					{
						actData = null;
						index = -1;
						Link.ProgressEvent(null, EventArgs.Empty);
					}
					else if ((timeout & 3) == 0)
					{
						Transfer(address, actData, index);
					}
				}
			}
		}
		catch
		{
		}
		return false;
	}

	private static bool SendCommand(byte command)
	{
		Open();
		if (IsConnected())
		{
			byte[] buffer = new byte[1] { command };
			port.Write(buffer, 0, 1);
			return true;
		}
		return false;
	}

	internal static bool StartCalib()
	{
		return SendCommand(49);
	}

	internal static bool StopCalib()
	{
		return SendCommand(48);
	}

	internal static bool LaserOn()
	{
		return SendCommand(54);
	}

	internal static bool LaserOff()
	{
		return SendCommand(55);
	}

	internal static bool Measure()
	{
		return SendCommand(56);
	}

	internal static bool StartRead(int addr, byte[] data)
	{
		Open();
		if (IsConnected())
		{
			actData = data;
			address = addr;
			index = 0;
			write = false;
			Transfer(addr, data, 0);
			timeout = 12;
			return true;
		}
		return false;
	}

	internal static bool StartWrite(int addr, byte[] data)
	{
		Open();
		if (IsConnected())
		{
			actData = data;
			address = addr;
			index = 0;
			write = true;
			Transfer(addr, data, 0);
			timeout = 12;
			return true;
		}
		return false;
	}

	private static void Transfer(int addr, byte[] data, int index)
	{
		if (write)
		{
			byte[] buffer = new byte[7]
			{
				57,
				(byte)addr,
				(byte)(addr >> 8),
				data[index],
				data[index + 1],
				data[index + 2],
				data[index + 3]
			};
			port.Write(buffer, 0, 7);
		}
		else
		{
			byte[] buffer = new byte[3]
			{
				56,
				(byte)addr,
				(byte)(addr >> 8)
			};
			port.Write(buffer, 0, 3);
		}
	}

	internal static int Progress()
	{
		return index >> 2;
	}

	internal static bool Busy()
	{
		return actData != null;
	}
}
