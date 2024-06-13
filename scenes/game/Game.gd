extends Node

var spaceship_scene = preload("res://scenes/spaceship/Spaceship.tscn")
var server

func _ready():
	server = $Server
	var signal_manager = $SignalManager
	signal_manager.connect("client_connected", Callable(self, "_on_ClientConnected"))

func _on_ClientConnected(client_identifier):
	var spaceship_instance = spaceship_scene.instantiate()
	spaceship_instance.position = get_viewport().size / 2
	spaceship_instance.z_index = 1
	add_child(spaceship_instance)
	server.call("AddClientSpaceship", client_identifier, spaceship_instance)
	print("New spaceship instantiated for client ", client_identifier)
