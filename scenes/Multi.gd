extends Node2D

var spaceship
var _server_scene = preload("res://scenes/server/Server.tscn")
var _client_scene = preload("res://scenes/client/Client.tscn")

func _ready():
	spaceship = get_node("Spaceship")

func _process(delta):
	if Input.is_action_pressed("ui_left"):
		spaceship.set_motor_left(0)
		spaceship.set_motor_right(1)
	elif Input.is_action_pressed("ui_right"):
		spaceship.set_motor_left(1)
		spaceship.set_motor_right(0)
	else:
		spaceship.set_motor_left(0.5)
		spaceship.set_motor_right(0.5)


func _on_button_pressed():
	self.add_child(_server_scene.instantiate())


func _on_button_2_pressed():
	self.add_child(_client_scene.instantiate())	
