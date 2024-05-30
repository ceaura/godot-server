import socket
import sys

def tcp_client(server_address, server_port):
    # Crée un socket TCP/IP
    sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)

    # Se connecte au serveur
    server_address = (server_address, server_port)
    print(f"Connecting to {server_address[0]}:{server_address[1]}")
    sock.connect(server_address)

    try:
        while True:
            # Lit l'entrée utilisateur
            message = input("Enter command: ")
            if message.lower() == "exit":
                print("Closing connection.")
                break

            # Envoie la commande au serveur
            sock.sendall(message.encode('utf-8'))

            # Attendre la réponse du serveur
            data = sock.recv(1024)
            print(f"Server response: {data.decode('utf-8')}")
    except Exception as e:
        print(f"An error occurred: {e}")
    finally:
        print("Closing socket.")
        sock.close()

def udp_client(server_address, server_port):
    # Crée un socket UDP
    sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)

    server_address = (server_address, server_port)
    print(f"Connecting to UDP server at {server_address[0]}:{server_address[1]}")

    try:
        while True:
            # Lit l'entrée utilisateur
            message = input("Enter command: ")
            if message.lower() == "exit":
                print("Closing connection.")
                break

            # Envoie la commande au serveur
            sock.sendto(message.encode('utf-8'), server_address)

            # Attendre la réponse du serveur
            data, _ = sock.recvfrom(1024)
            print(f"Server response: {data.decode('utf-8')}")
    except Exception as e:
        print(f"An error occurred: {e}")
    finally:
        print("Closing socket.")
        sock.close()

if __name__ == "__main__":
    if len(sys.argv) < 4:
        print("Usage: python client.py <tcp|udp> <server_address> <server_port>")
        sys.exit(1)

    protocol = sys.argv[1].lower()
    server_address = sys.argv[2]
    server_port = int(sys.argv[3])

    if protocol == "tcp":
        tcp_client(server_address, server_port)
    elif protocol == "udp":
        udp_client(server_address, server_port)
    else:
        print("Invalid protocol specified. Use 'tcp' or 'udp'.")
        sys.exit(1)
