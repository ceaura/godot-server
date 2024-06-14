# tests/unit/test_command_handler.gd
extends "res://addons/gut/test.gd"

# Charger le script que vous voulez tester
var command_handler = preload("res://path/to/CommandHandler.cs")

func test_process_command():
	var client_spaceships = {}
	var instance = command_handler.new(client_spaceships)
	var response = instance.ProcessCommand(null, "NAME=test")
	assert_eq(response, "Name changed to: test", "La commande NAME doit être traitée correctement")
