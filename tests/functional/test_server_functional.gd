# tests/unit/test_server_creation.gd
extends "res://addons/gut/test.gd"

# Charger les scripts que vous voulez tester
var TcpServer = preload("res://scenes/server/TcpServer.cs")
var UdpServer = preload("res://scenes/server/UdpServer.cs")
var CommandHandler = preload("res://scenes/server/CommandHandler.cs")

func test_tcp_server_creation():
	var command_handler = CommandHandler._new()
	var signal_manager = Node.new()
	var tcp_server = TcpServer._new(9999, command_handler, signal_manager)
	assert_not_null(tcp_server, "Le serveur TCP doit être créé")
	tcp_server.Start()
	assert_true(tcp_server.is_listening(), "Le serveur TCP doit être en écoute sur le port 9999")

func test_udp_server_creation():
	var command_handler = CommandHandler._new()
	var udp_server = UdpServer._new(10000, command_handler)
	assert_not_null(udp_server, "Le serveur UDP doit être créé")
	udp_server.Start()
	assert_true(udp_server.is_listening(), "Le serveur UDP doit être en écoute sur le port 10000")
