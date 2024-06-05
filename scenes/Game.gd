extends Node

var spaceship
var _server_scene = preload("res://scenes/server/Server.tscn")

func _ready():
	spaceship = get_node("Spaceship")
	self.add_child(_server_scene.instantiate())

func _process(delta):
	pass

