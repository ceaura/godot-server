# tests/functional/test_udp_server_functional.gd
extends "res://addons/gut/test.gd"

func test_udp_server_receives_packets():
	var udp_server = preload("res://path/to/UdpServer.cs").new(10000, null)
	add_child(udp_server)
	
	yield(udp_server, "ready")
	
	# Simulez l'envoi d'un paquet UDP au serveur
	var client = preload("res://path/to/UdpClient.cs").new()
	client.send_packet("127.0.0.1", 10000, "test_message")
	
	yield(udp_server, "packet_received")
	
	# Vérifiez que le paquet est reçu
	assert_true(udp_server.has_received_packet("test_message"), "Le serveur UDP doit recevoir le paquet")
