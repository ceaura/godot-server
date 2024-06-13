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
			for (int i = _tcpClients.Count - 1; i >= 0; i--)
			{
				var client = _tcpClients[i];
				if (client.Available > 0)
				{
					var buffer = new byte[client.Available];
					client.GetStream().Read(buffer, 0, buffer.Length);
					OnTcpPacketReceived(client, buffer);
				}
				else if (!IsClientConnected(client))
				{
					DisconnectClient(client);
					_tcpClients.RemoveAt(i);
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

	private bool IsClientConnected(TcpClient client)
	{
		try
		{
			return !(client.Client.Poll(1, SelectMode.SelectRead) && client.Available == 0);
		}
		catch (SocketException) { return false; }
	}

	private void DisconnectClient(TcpClient client)
	{
		string clientIdentifier = client.Client.RemoteEndPoint.ToString();
		if (_commandHandler is CommandHandler handler)
		{
			handler.ProcessCommand(client, "EXIT");
		}
		client.Close();
		GD.Print($"Client {clientIdentifier} disconnected");
	}
}
