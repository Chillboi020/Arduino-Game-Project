using Godot;
using System;
using System.IO.Ports;
using System.Diagnostics;

namespace SpaceShooter2D.scripts.Tools;
public partial class SerialCommunicator : Node
{
	private SerialPort _serialPort;

	public new bool IsConnected { get; private set; }

	[Signal]
	public delegate void DataReceivedEventHandler(string data);

	public override void _Ready()
	{
		IsConnected = false;
		ConnectArduino();
	}
	
	private void ConnectArduino()
	{
		var ports = SerialPort.GetPortNames();
		
		foreach (var port in ports)
		{
			_serialPort = new SerialPort(port, 9600);

			try
			{
				_serialPort.Open();
				if (Performhandshake())
				{
					IsConnected = true;
					GD.Print("Connected to Arduino on port: " + port);
					break;	
				}
				else
				{
					_serialPort.Close();
					IsConnected = false;
				}
			}
			catch (Exception ex)
			{
				GD.Print("Failed to connect on port: " + port);
				GD.Print("Exception: " + ex.Message);
				if (_serialPort.IsOpen)
				{
					_serialPort.Close();
				}
				IsConnected = false;
			}
		}

		if (!IsConnected)
		{
			GD.Print("Could not connect to any Arduino port.");
		}
	}
	
	private bool Performhandshake()
	{
		try
		{
			var stopwatch = Stopwatch.StartNew();
			var handshakeTimeout = TimeSpan.FromMilliseconds(3000);

			while (stopwatch.Elapsed < handshakeTimeout)
			{
				if (_serialPort.BytesToRead > 0)
				{
					var response = _serialPort.ReadLine().Trim();
					if (response == "OK?")
					{
						_serialPort.WriteLine("OK!");
						_serialPort.BaseStream.Flush();
						return true; // Handshake successful
					}
				}
			}
			
		}
		catch (Exception e)
		{
			GD.Print("Handshake failed: " + e.Message);
		}

		return false;
	}

	private void ReadData()
	{
		while (IsConnected && _serialPort.IsOpen)
		{
			try
			{
				string data = _serialPort.ReadLine();
				EmitSignal(nameof(DataReceived), data);
			}
			catch (Exception ex)
			{
				GD.Print("Error reading data from Arduino: " + ex.Message);
				IsConnected = false;
			}
		}

		if (_serialPort is { IsOpen: true })
		{
			_serialPort.Close();
		}
	}
}
