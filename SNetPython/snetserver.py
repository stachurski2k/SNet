import snetclient as sc
import spacket as sp
import socket
import struct
import threading
class SNetServer:
    def __init__(self, my_port=8080, ips=None, ports=None,names=None):
        if ips is None:
            ips = []
        if ports is None:
            ports = []
        if names is None:
            names = []
        self.my_port = my_port
        self.clients = {}
        self.named_clients={}
        self.server_thread = threading.Thread(target=self.server_recv_thread)
        self.is_working = False
        self.sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)

        for i in range(len(ips)):
            endpoint = (ips[i], ports[i])
            self.clients[endpoint] = sc.SNetClient(endpoint)
            self.named_clients[names[i]] = self.clients[endpoint]

    def start(self):
        self.is_working = True
        self.sock.bind(('127.0.0.1', self.my_port))
        self.server_thread.start()

    def stop(self):
        self.is_working = False
        self.sock.close()
        self.server_thread.join()

    def server_recv_thread(self):
        while self.is_working:
            print(self.my_port)
            try:
                data, sender = self.sock.recvfrom(1024)
                if sender in self.clients:
                    self.clients[sender].receive_bytes(data)
            except:
                pass
            
            # Add any additional handling for unknown clients if needed
            # For example, you might want to create a new SNetClient for unknown senders.
            # self.clients[sender] = SNetClient(sender)

    def send_to_client(self, endpoint, packet):
        bytes_data = packet.to_bytes()
        try:
            if isinstance(endpoint,str):
                self.sock.sendto(bytes_data,self.named_clients[endpoint].endpoint)
            else:
                self.sock.sendto(bytes_data, endpoint)
        except:
            pass