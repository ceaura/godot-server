using Godot;

public class PlayerInfo
{
	public Area2D Spaceship { get; set; }
	public string Name { get; set; }

	public PlayerInfo(Area2D spaceship, string name = "")
	{
		Spaceship = spaceship;
		Name = name;
	}
}
