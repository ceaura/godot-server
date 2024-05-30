using Godot;
using System.Net.Sockets;
using System.Text;

public partial class Client : Node
{
	[Export] private string _serverAddress = "localhost";
	[Export] private int _tcpPort = 9999;
	[Export] private int _udpPort = 10000;

	private TcpClient _tcpClient;
	private UdpClient _udpClient;

	public override void _EnterTree()
	{
		ConnectToServer();
		SendTcpMessage("test");
	}

	private void ConnectToServer()
	{
		_tcpClient = new TcpClient();
		_tcpClient.Connect(_serverAddress, _tcpPort);
		GD.Print("Connected to TCP server");

		_udpClient = new UdpClient();
		_udpClient.Connect(_serverAddress, _udpPort);
		GD.Print("Connected to UDP server");
	}

	public void SendTcpMessage(string message)
	{
		byte[] data = Encoding.UTF8.GetBytes(message);
		_tcpClient.GetStream().Write(data, 0, data.Length);
		GD.Print("Sent TCP message: ", message);
	}

	public void SendUdpMessage(string message)
	{
		byte[] data = Encoding.UTF8.GetBytes(message);
		_udpClient.Send(data, data.Length);
		GD.Print("Sent UDP message: ", message);
	}
}
