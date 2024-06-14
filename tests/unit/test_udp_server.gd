# tests/unit/test_udp_server.gd
extends "res://addons/gut/test.gd"

# Charger le script que vous voulez tester
var udp_server = preload("res://scenes/server/UdpServer.cs")

func test_start_udp_server():
	var instance = udp_server.new(10000, null)
	instance.Start()
	assert_not_null(instance._udpClient, "Le serveur UDP doit démarrer")
