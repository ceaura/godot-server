using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Godot;

public class TcpServer : INetworkServer
{
	private TcpListener _tcpListener;
	private List<TcpClient> _tcpClients = new List<TcpClient>();
	private ICommandHandler _commandHandler;
	private Node _signalManager;
	private int _port;

	public TcpServer(int port, ICommandHandler commandHandler, Node signalManager)
	{
		_port = port;
		_commandHandler = commandHandler;
		_signalManager = signalManager;
	}

	public void Start()
	{
		_tcpListener = new TcpListener(IPAddress.Any, _port);
		_tcpListener.Start();

		GD.Print("TCP Server listening on port ", _port);
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
			var clientIdentifier = client.Client.RemoteEndPoint.ToString();
			_signalManager.CallDeferred("emit_signal", "client_connected", clientIdentifier);
		}
	}

	public void Process()
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

	private void OnTcpPacketReceived(TcpClient client, byte[] data)
	{
		string message = Encoding.UTF8.GetString(data);
		GD.Print("Received TCP message: ", message);
		string response = _commandHandler.ProcessCommand(client, message);
		byte[] responseData = Encoding.UTF8.GetBytes(response);
		client.GetStream().Write(responseData, 0, responseData.Length);
	}
}
