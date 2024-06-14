# tests/unit/test_server.gd
extends "res://addons/gut/test.gd"

# Charger le script que vous voulez tester
var server = preload("res://path/to/Server.cs")

func test_add_client_spaceship():
	var instance = server.new()
	var spaceship = Area2D.new()
	instance.AddClientSpaceship("client1", spaceship)
	assert_not_null(instance._clientSpaceships["client1"], "Le client doit être ajouté")
