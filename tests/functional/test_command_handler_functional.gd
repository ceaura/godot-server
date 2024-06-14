# tests/functional/test_command_handler_functional.gd
extends "res://addons/gut/test.gd"

func test_command_handler_processes_commands():
	var client_spaceships = {}
	var command_handler = preload("res://scenes/server/CommandHandler.cs").new(client_spaceships)
	
	# Simulez une commande NAME
	var response = command_handler.ProcessCommand(null, "NAME=test")
	assert_eq(response, "Name changed to: test", "La commande NAME doit être traitée correctement")
	
	# Ajoutez un vaisseau client et simulez une commande de mise à jour des moteurs
	client_spaceships["client1"] = PlayerInfo.new(Area2D.new())
	response = command_handler.ProcessCommand(null, "MotL=1.0#MotR=1.0")
	assert_eq(response, "Command processed", "La commande de mise à jour des moteurs doit être traitée correctement")
