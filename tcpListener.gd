extends Node

var server := TCPServer.new()
var clients := []
var port = 8000
var peers: Array[PacketPeerStream] = []

func _ready():
	start_server()

func start_server():
	var result = server.listen(port)
	if result == OK:
		print("Server is listening on port %d" % port)
	else:
		print("Failed to start server: %s" % result)

func _process(_delta):
	if server.is_connection_available():
		var client = server.take_connection()
		if client:
			var peer = PacketPeerStream.new()
			peer.stream_peer = client
			peers.append(peer)
			peer.put_packet("Hello from server".to_utf8_buffer())
			print("New client connected")
	
	for peer in peers:
		if peer.get_available_packet_count() > 0:
			var packet = peer.get_packet().get_string_from_utf8()
			print("Received: %s" % packet)
			analyze_command(packet, peer)

func analyze_command(command: String, peer: PacketPeerStream):
	print("Command received: %s" % command)
	send_data_to_client(peer, "Command processed: %s" % command)

func send_data_to_client(peer: PacketPeerStream, data: String):
	peer.put_packet(data.to_utf8_buffer())
