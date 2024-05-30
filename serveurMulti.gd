extends Node

var network = ENetMultiplayerPeer.new()
var is_server = false
var server_peer_id = 1

@onready var message_container = $VBoxContainer
@onready var input_line = $VBoxContainer/LineEdit
@onready var send_button = $VBoxContainer/Button

func _ready():
	setup_ui()
	start_server()
	connect_to_server()

func setup_ui():
	send_button.connect("pressed", self, "_on_send_button_pressed")

func start_server():
	var port = 4242
	network.create_server(port, 32)  # 32 clients max
	get_tree().network_peer = network
	network.connect("peer_connected", self, "_on_peer_connected")
	network.connect("peer_disconnected", self, "_on_peer_disconnected")
	is_server = true
	print("Server started on port %d" % port)
	add_message("Server started on port %d" % port)

func connect_to_server():
	var server_ip = "127.0.0.1"
	var port = 4242
	network.create_client(server_ip, port)
	get_tree().network_peer = network
	network.connect("connection_succeeded", self, "_on_connection_succeeded")
	network.connect("connection_failed", self, "_on_connection_failed")

func _on_peer_connected(id):
	print("Client connected with ID: %d" % id)
	add_message("Client connected with ID: %d" % id)

func _on_peer_disconnected(id):
	print("Client disconnected with ID: %d" % id)
	add_message("Client disconnected with ID: %d" % id)

func _on_connection_succeeded():
	print("Connected to server")
	add_message("Connected to server")
	rpc_id(server_peer_id, "send_message_to_server", "Hello from client")

func _on_connection_failed():
	print("Failed to connect to server")
	add_message("Failed to connect to server")

func send_message_to_server(message: String):
	print("Received message from client: %s" % message)
	add_message("Received message from client: %s" % message)
	rpc_id(get_tree().get_rpc_sender_id(), "receive_message_from_server", "Hello from server")

func receive_message_from_server(message: String):
	print("Received message from server: %s" % message)
	add_message("Received message from server: %s" % message)

func _on_send_button_pressed():
	var message = input_line.text
	if message != "":
		input_line.clear()
		send_message(message)

func send_message(message: String):
	if is_server:
		send_message_to_server(message)
	else:
		rpc_id(server_peer_id, "send_message_to_server", message)

func add_message(text: String):
	var label = Label.new()
	label.text = text
	message_container.add_child(label)
