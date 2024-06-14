extends Node

var spaceship_scene = preload("res://scenes/spaceship/Spaceship.tscn")
var server
var amount_obstacles = 30

#### EXPORT ####
@export var obstacle : PackedScene

func _ready():
	server = $Server
	var signal_manager = $SignalManager
	signal_manager.connect("client_connected", Callable(self, "_on_ClientConnected"))
	spawn_obstacles()

func _on_ClientConnected(client_identifier):
	var spaceship_instance = spaceship_scene.instantiate()
	spaceship_instance.position = get_viewport().size / 2
	spaceship_instance.z_index = 1
	add_child(spaceship_instance)
	server.call("AddClientSpaceship", client_identifier, spaceship_instance)
	print("New spaceship instantiated for client ", client_identifier)
	
func spawn_obstacles():
	var spawned_obstacles = []
	for i in range(amount_obstacles):
		var new_obstacle = obstacle.instantiate()
		var position_found = false
		while not position_found:
			var random_position = Vector2(randi() % get_viewport().get_size().x, randi() % get_viewport().get_size().y)
			new_obstacle.position = random_position
			position_found = true
			for other_obstacle in spawned_obstacles:
				if new_obstacle.global_position.distance_to(other_obstacle.global_position) < get_obstacle_radius(new_obstacle) + get_obstacle_radius(other_obstacle):
					position_found = false
					break
		spawned_obstacles.append(new_obstacle)
		add_child(new_obstacle)

func get_obstacle_radius(obstacle):
	var collision_shape = obstacle.get_node("CollisionShape2D")
	return max(collision_shape.shape.extents.x, collision_shape.shape.extents.y)
