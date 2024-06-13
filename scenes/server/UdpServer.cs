using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Godot;

public class UdpServer : INetworkServer
{
	private UdpClient _udpClient;
	private ICommandHandler _commandHandler;
	private int _port;

	public UdpServer(int port, ICommandHandler commandHandler)
	{
		_port = port;
		_commandHandler = commandHandler;
	}

	public void Start()
	{
		_udpClient = new UdpClient(_port);
		GD.Print("UDP Server listening on port ", _port);
		Thread udpReceiveThread = new Thread(ReceiveUdpPackets);
		udpReceiveThread.Start();
	}

	private void ReceiveUdpPackets()
	{
		IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);
		while (true)
		{
			var data = _udpClient.Receive(ref remoteEP);
			OnUdpPacketReceived(remoteEP, data);
		}
	}

	public void Process()
	{
		// TODO
	}

	private void OnUdpPacketReceived(IPEndPoint remoteEP, byte[] data)
	{
		string message = Encoding.UTF8.GetString(data);
		GD.Print("Received UDP message: ", message);
		string response = _commandHandler.ProcessCommand(null, message);
		byte[] responseData = Encoding.UTF8.GetBytes(response);
		_udpClient.Send(responseData, responseData.Length, remoteEP);
	}
}
