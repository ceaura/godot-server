using System.Net.Sockets;

public interface ICommandHandler
{
	string ProcessCommand(TcpClient client, string command);
}
