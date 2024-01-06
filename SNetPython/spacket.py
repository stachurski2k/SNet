from enum import Enum
import struct

class SPacketId(Enum):
    # Server
    STATUS=0
    ENABLE = 1  # no data
    DISABLE = 2
    
    # Client
    ENABLED = 100 + 2  # 1 byte yes or no
    DATA = 100 + 3    # 1 byte yes or no

class SPacket:
    def __init__(self, packet_id, bytes_data=None):
        self.id = packet_id
        self.data = bytearray()
        self.bytes = bytearray()
        self.read_index = 0  # only for incoming packets
        self.size = 0

        if bytes_data is not None:
            self.id = SPacketId(struct.unpack('<I', bytes_data[:4])[0])
            self.size = struct.unpack('<I', bytes_data[4:8])[0]
            self.read_index = 0
            self.bytes = bytearray(bytes_data[:self.size])

    @staticmethod
    def to_read(buff, index, count):
        if count < 8:
            return 10000
        size = struct.unpack('<I', buff[index + 4:index + 8])[0]
        return 8+size

    def to_bytes(self):
        bval = struct.pack('<I', self.size)
        self.data = bytearray(bval) + self.data
        bval = struct.pack('<I', self.id.value)
        self.data = bytearray(bval) + self.data
        self.bytes = self.data
        return bytes(self.bytes)

    def write(self, val):
        bval=None
        if isinstance(val,int):
            bval = struct.pack('<I', val)
        else:
            bval = struct.pack('<f', val)
        self.data.extend(bval)
        self.size += 4

    def read_float(self):
        f = struct.unpack('<f', self.bytes[self.read_index:self.read_index + 4])[0]
        self.read_index += 4
        return f
    def read_int32(self):
        f = struct.unpack('<I', self.bytes[self.read_index:self.read_index + 4])[0]
        self.read_index += 4
        return f

    def dispose(self):
        self.data.clear()