# tests/unit/test_tcp_server.gd
extends "res://addons/gut/test.gd"

# Charger le script que vous voulez tester
var tcp_server = preload("res://path/to/TcpServer.cs")

func test_start_tcp_server():
    var instance = tcp_server.new(9999, null, null)
    instance.Start()
    assert_not_null(instance._tcpListener, "Le serveur TCP doit démarrer")
