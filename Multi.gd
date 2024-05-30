extends Node2D

var _server_scene = preload("res://Server.tscn")
var _client_scene = preload("res://Client.tscn")

func _on_button_pressed():
	self.add_child(_server_scene.instantiate())


func _on_button_2_pressed():
	self.add_child(_client_scene.instantiate())	
