import spacket as sp
import socket
import struct
from enum import Enum
class ClientStatus(Enum):
    UNKNOWN=0
    CONNECTED=1
    DISCONNECTED=2
class SNetClient:
    def __init__(self, endpoint):
        self.endpoint = endpoint
        self.recv_buff = bytearray(1024)
        self.buff_size = 0
        # self.client_status=ClientStatus.UNKNOWN
        self.on_packet_received = None

    def receive_bytes(self, bytes_data):
        self.recv_buff[self.buff_size:self.buff_size + len(bytes_data)] = bytes_data
        self.buff_size += len(bytes_data)
        to_read = sp.SPacket.to_read(self.recv_buff, 0, self.buff_size)

        print(to_read)  # Print for testing

        if to_read <= self.buff_size:
            packet = sp.SPacket(-1,self.recv_buff)
            print(packet.id)  # Print for testing
            self.packet_received(packet)
            self.recv_buff[:len(self.recv_buff) - to_read] = self.recv_buff[to_read:]
            self.buff_size -= to_read

    def packet_received(self, packet):
        # if(packet.id==sp.SPacketId.STATUS):
            # self.status=ClientStatus(packet.read_int32())
            # print(self.status)
        if self.on_packet_received is not None:
            self.on_packet_received(packet)