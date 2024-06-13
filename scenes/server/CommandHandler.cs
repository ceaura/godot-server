using System;
using System.Collections.Generic;
using System.Net.Sockets;
using Godot;

public class CommandHandler : ICommandHandler
{
	private Dictionary<string, PlayerInfo> _clientSpaceships;

	public CommandHandler(Dictionary<string, PlayerInfo> clientSpaceships)
	{
		_clientSpaceships = clientSpaceships;
	}

	public string ProcessCommand(TcpClient client, string command)
	{
		var commands = command.Split('#');
		float? motL = null;
		float? motR = null;
		string response = "Command processed";

		foreach (var cmd in commands)
		{
			var parts = cmd.Split('=');
			var key = parts[0];
			var args = parts.Length > 1 ? parts[1].Split(',') : new string[0];

			switch (key)
			{
				case "NAME":
					response = HandleNameCommand(args, client);
					break;
				case "COL":
					response = HandleColorCommand(args);
					break;
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
				case "NLIST":
					response = HandleNListCommand();
					break;
				case "MSG":
					response = HandleMsgCommand(args, client);
					break;
				case "USRMSG":
					response = HandleUsrMsgCommand(args);
					break;
				case "ORIENT":
					response = HandleOrientCommand(client);
					break;
				case "EXIT":
					response = HandleExitCommand(client);
					break;
				default:
					GD.Print("Unknown command: ", key);
					response = "Unknown command: " + key;
					break;
			}
		}

		if (client != null && (motL.HasValue || motR.HasValue))
		{
			var clientIdentifier = client.Client.RemoteEndPoint.ToString();
			UpdateMotors(clientIdentifier, motL ?? 0.5f, motR ?? 0.5f);
		}

		return response;
	}

	private string HandleNameCommand(string[] args, TcpClient client)
	{
		if (args.Length > 0)
		{
			var playerName = args[0];
			var clientIdentifier = client.Client.RemoteEndPoint.ToString();
			if (_clientSpaceships.ContainsKey(clientIdentifier))
			{
				var playerInfo = _clientSpaceships[clientIdentifier];
				playerInfo.Name = playerName;
				playerInfo.Spaceship.Call("set_player_name", playerName);
				GD.Print($"Set name for client {clientIdentifier} to {playerName}");
			}
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

	private string HandleNListCommand()
	{
		List<string> playerNames = new List<string>();
		foreach (var client in _clientSpaceships.Values)
		{
			playerNames.Add(client.Name);
		}
		string response = string.Join("=", playerNames);
		GD.Print("List of connected players: ", response);
		return response;
	}

	private string HandleMsgCommand(string[] args, TcpClient client)
	{
		if (args.Length > 0)
		{
			var message = args[0];
			var clientIdentifier = client.Client.RemoteEndPoint.ToString();
			if (_clientSpaceships.ContainsKey(clientIdentifier))
			{
				var playerInfo = _clientSpaceships[clientIdentifier];
				playerInfo.Spaceship.Call("set_message", message);
				playerInfo.Message = message; // Store the message in PlayerInfo
				GD.Print($"Set message for client {clientIdentifier} to {message}");
			}
			return "Message set to: " + message;
		}
		return "Invalid MSG command";
	}

	private string HandleUsrMsgCommand(string[] args)
	{
		if (args.Length > 0)
		{
			var playerName = args[0];
			foreach (var playerInfo in _clientSpaceships.Values)
			{
				if (playerInfo.Name == playerName)
				{
					return playerInfo.Message ?? "EMPTY";
				}
			}
		}
		return "EMPTY";
	}

	private string HandleExitCommand(TcpClient client)
	{
		var clientIdentifier = client.Client.RemoteEndPoint.ToString();
		if (_clientSpaceships.ContainsKey(clientIdentifier))
		{
			var playerInfo = _clientSpaceships[clientIdentifier];
			playerInfo.Spaceship.QueueFree();
			_clientSpaceships.Remove(clientIdentifier);
			GD.Print($"Client {clientIdentifier} disconnected and spaceship removed");
		}
		return "Client disconnected";
	}
	
	private string HandleOrientCommand(TcpClient client)
	{
		var clientIdentifier = client.Client.RemoteEndPoint.ToString();
		if (_clientSpaceships.ContainsKey(clientIdentifier))
		{
			var playerInfo = _clientSpaceships[clientIdentifier];
			float orientation = playerInfo.Spaceship.Rotation % (2 * Mathf.Pi);
			if (orientation < 0)
			{
				orientation += 2 * Mathf.Pi;
			}
			GD.Print($"Orientation for client {clientIdentifier}: {orientation}");
			return orientation.ToString();
		}
		return "Invalid ORIENT command";
	}

	private void UpdateMotors(string clientIdentifier, float motL, float motR)
	{
		if (_clientSpaceships.ContainsKey(clientIdentifier))
		{
			var playerInfo = _clientSpaceships[clientIdentifier];
			playerInfo.Spaceship.Call("set_motor_left", motL);
			playerInfo.Spaceship.Call("set_motor_right", motR);
			GD.Print($"Motors updated for client {clientIdentifier}: Left={motL}, Right={motR}");
		}
	}
}
