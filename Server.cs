using Godot;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public partial class Server : Node
{
	[Export] private int _tcpPort = 9999;
	[Export] private int _udpPort = 10000;

	private TcpListener _tcpListener;
	private UdpClient _udpClient;
	private List<TcpClient> _tcpClients = new List<TcpClient>();

	public override void _EnterTree()
	{
		GD.Print("Server started");
		StartTcpServer();
		StartUdpServer();
	}

	public override void _Process(double delta)
	{
		// Handle TCP clients
		lock (_tcpClients)
		{
			foreach (var client in _tcpClients)
			{
				if (client.Available > 0)
				{
					var buffer = new byte[client.Available];
					client.GetStream().Read(buffer, 0, buffer.Length);
					OnTcpPacketReceived(client, buffer);
				}
			}
		}
	}

	private void StartTcpServer()
	{
		_tcpListener = new TcpListener(IPAddress.Any, _tcpPort);
		_tcpListener.Start();
		GD.Print("TCP Server listening on port ", _tcpPort);

		Thread tcpAcceptThread = new Thread(AcceptTcpClients);
		tcpAcceptThread.Start();
	}

	private void AcceptTcpClients()
	{
		while (true)
		{
			var client = _tcpListener.AcceptTcpClient();
			lock (_tcpClients)
			{
				_tcpClients.Add(client);
			}
			GD.Print("New TCP client connected");
		}
	}

	private void StartUdpServer()
	{
		_udpClient = new UdpClient(_udpPort);
		GD.Print("UDP Server listening on port ", _udpPort);

		Thread udpReceiveThread = new Thread(ReceiveUdpPackets);
		udpReceiveThread.Start();
	}

	private void ReceiveUdpPackets()
	{
		IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, _udpPort);
		while (true)
		{
			var data = _udpClient.Receive(ref remoteEP);
			OnUdpPacketReceived(remoteEP, data);
		}
	}

	private void OnTcpPacketReceived(TcpClient client, byte[] data)
	{
		string message = Encoding.UTF8.GetString(data);
		GD.Print("Received TCP message: ", message);
		string response = ProcessCommand(message);
		// Send the response back to the client
		byte[] responseData = Encoding.UTF8.GetBytes(response);
		client.GetStream().Write(responseData, 0, responseData.Length);
	}

	private void OnUdpPacketReceived(IPEndPoint remoteEP, byte[] data)
	{
		string message = Encoding.UTF8.GetString(data);
		GD.Print("Received UDP message: ", message);
		string response = ProcessCommand(message);
		// Send the response back to the client
		byte[] responseData = Encoding.UTF8.GetBytes(response);
		_udpClient.Send(responseData, responseData.Length, remoteEP);
	}

	private string ProcessCommand(string command)
	{
		// Parse and handle the command
		var commands = command.Split('#');
		foreach (var cmd in commands)
		{
			var parts = cmd.Split('=');
			var key = parts[0];
			var args = parts.Length > 1 ? parts[1].Split(',') : new string[0];

			switch (key)
			{
				case "NAME":
					return HandleNameCommand(args);
				case "COL":
					return HandleColorCommand(args);
				// Add other command handlers here
				default:
					GD.Print("Unknown command: ", key);
					return "Unknown command: " + key;
			}
		}
		return "Command processed";
	}

	private string HandleNameCommand(string[] args)
	{
		if (args.Length > 0)
		{
			var playerName = args[0];
			GD.Print("Change player name to: ", playerName);
			return "Name changed to: " + playerName;
		}
		return "Invalid NAME command";
	}

	private string HandleColorCommand(string[] args)
	{
		if (args.Length == 1)
		{
			var color = args[0];
			GD.Print("Change player color to: ", color);
			return "Color changed to: " + color;
		}
		else if (args.Length == 3)
		{
			var r = args[0];
			var g = args[1];
			var b = args[2];
			GD.Print($"Change player color to RGB: ({r}, {g}, {b})");
			return $"Color changed to RGB: ({r}, {g}, {b})";
		}
		return "Invalid COL command";
	}
}
