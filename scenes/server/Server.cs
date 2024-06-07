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

	private Dictionary<string, Area2D> clientSpaceships = new Dictionary<string, Area2D>();

	private Node signalManager;

	public override void _EnterTree()
	{
		string localIP = GetLocalIPAddress();
		GD.Print("Local IP is : ", localIP);

		StartTcpServer();
		StartUdpServer();

		signalManager = GetTree().Root.GetNode<Node>("Game").GetNode<Node>("SignalManager");
		GD.Print("Server started");
	}

	public override void _Process(double delta)
	{
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

		GD.Print("TCP Server listening on ", _tcpPort);

		Thread tcpAcceptThread = new Thread(AcceptTcpClients);
		tcpAcceptThread.Start();
	}

	private string GetLocalIPAddress()
	{
		var host = Dns.GetHostEntry(Dns.GetHostName());
		foreach (var ip in host.AddressList)
		{
			if (ip.AddressFamily == AddressFamily.InterNetwork)
			{
				return ip.ToString();
			}
		}
		throw new Exception("No network adapters with an IPv4 address in the system!");
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
			var clientIdentifier = client.Client.RemoteEndPoint.ToString();
			signalManager.CallDeferred("emit_signal", "client_connected", clientIdentifier);
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
		string response = ProcessCommand(client, message);
		byte[] responseData = Encoding.UTF8.GetBytes(response);
		client.GetStream().Write(responseData, 0, responseData.Length);
	}

	private void OnUdpPacketReceived(IPEndPoint remoteEP, byte[] data)
	{
		string message = Encoding.UTF8.GetString(data);
		GD.Print("Received UDP message: ", message);
		string response = ProcessCommand(null, message);
		byte[] responseData = Encoding.UTF8.GetBytes(response);
		_udpClient.Send(responseData, responseData.Length, remoteEP);
	}

	private string ProcessCommand(TcpClient client, string command)
	{
		var commands = command.Split('#');
		float? motL = null;
		float? motR = null;

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
				case "MotL":
					if (args.Length == 1)
					{
						motL = (float)Convert.ToDouble(args[0]);
					}
					break;
				case "MotR":
					if (args.Length == 1)
					{
						motR = (float)Convert.ToDouble(args[0]);
					}
					break;
				default:
					GD.Print("Unknown command: ", key);
					return "Unknown command: " + key;
			}
		}

		if (client != null && (motL.HasValue || motR.HasValue))
		{
			var clientIdentifier = client.Client.RemoteEndPoint.ToString();
			UpdateMotors(clientIdentifier, motL ?? 0.5f, motR ?? 0.5f);
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

	private void UpdateMotors(string clientIdentifier, float motL, float motR)
	{
		if (clientSpaceships.ContainsKey(clientIdentifier))
		{
			var spaceship = clientSpaceships[clientIdentifier];
			spaceship.Call("set_motor_left", motL);
			spaceship.Call("set_motor_right", motR);
			GD.Print($"Motors updated for client {clientIdentifier}: Left={motL}, Right={motR}");
		}
	}

	public void AddClientSpaceship(string clientIdentifier, Area2D spaceship)
	{
		if (!clientSpaceships.ContainsKey(clientIdentifier))
		{
			clientSpaceships[clientIdentifier] = spaceship;
			GD.Print($"client added in server");
		}
	}
}
