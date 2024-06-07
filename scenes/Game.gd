extends Node

var spaceship_scene = preload("res://scenes/spaceship/Spaceship.tscn")
var server

func _ready():
	server = $Server
	var signal_manager = $SignalManager
	signal_manager.connect("client_connected", Callable(self, "_on_ClientConnected"))

func _on_ClientConnected(client_identifier):
	var spaceship_instance = spaceship_scene.instantiate()
	add_child(spaceship_instance)
	server.call("AddClientSpaceship", client_identifier, spaceship_instance)
	print("New spaceship instantiated for client ", client_identifier)

