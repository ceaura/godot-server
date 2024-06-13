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

	private INetworkServer _tcpServer;
	private INetworkServer _udpServer;

	private Dictionary<string, Area2D> _clientSpaceships = new Dictionary<string, Area2D>();
	private Node _signalManager;
	private ICommandHandler _commandHandler;

	public override void _EnterTree()
	{
		string localIP = GetLocalIPAddress();
		GD.Print("Local IP is : ", localIP);

		_signalManager = GetTree().Root.GetNode<Node>("Game").GetNode<Node>("SignalManager");
		_commandHandler = new CommandHandler(_clientSpaceships);

		var factory = new ServerFactory(_tcpPort, _udpPort, _commandHandler, _signalManager);
		_tcpServer = factory.CreateTcpServer();
		_udpServer = factory.CreateUdpServer();

		_tcpServer.Start();
		_udpServer.Start();

		GD.Print("Server started");
	}

	public override void _Process(double delta)
	{
		_tcpServer.Process();
	}

	public void AddClientSpaceship(string clientIdentifier, Area2D spaceship)
	{
		if (!_clientSpaceships.ContainsKey(clientIdentifier))
		{
			_clientSpaceships[clientIdentifier] = spaceship;
			GD.Print($"Client added in server");
		}
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
}
