using Godot;

public class PlayerInfo
{
	public Area2D Spaceship { get; set; }
	public string Name { get; set; }
	public string Message { get; set; } // Add a field to store the message

	public PlayerInfo(Area2D spaceship, string name = "")
	{
		Spaceship = spaceship;
		Name = name;
		Message = "EMPTY"; // Initialize with "EMPTY"
	}
}
