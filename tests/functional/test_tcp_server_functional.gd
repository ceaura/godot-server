# tests/functional/test_tcp_server_functional.gd
extends "res://addons/gut/test.gd"

func test_tcp_server_accepts_clients():
	var tcp_server = preload("res://path/to/TcpServer.cs").new(9999, null, null)
	add_child(tcp_server)
	
	yield(tcp_server, "ready")
	
	# Simulez un client TCP se connectant au serveur
	var client = preload("res://path/to/TcpClient.cs").new()
	client.connect_to_host("127.0.0.1", 9999)
	
	yield(client, "connected")
	
	# Vérifiez que le client est accepté
	assert_eq(tcp_server._tcpClients.size(), 1, "Un client doit être connecté au serveur TCP")
