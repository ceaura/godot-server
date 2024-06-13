using Godot;

public class ServerFactory
{
	private int _tcpPort;
	private int _udpPort;
	private ICommandHandler _commandHandler;
	private Node _signalManager;

	public ServerFactory(int tcpPort, int udpPort, ICommandHandler commandHandler, Node signalManager)
	{
		_tcpPort = tcpPort;
		_udpPort = udpPort;
		_commandHandler = commandHandler;
		_signalManager = signalManager;
	}

	public INetworkServer CreateTcpServer()
	{
		return new TcpServer(_tcpPort, _commandHandler, _signalManager);
	}

	public INetworkServer CreateUdpServer()
	{
		return new UdpServer(_udpPort, _commandHandler);
	}
}
