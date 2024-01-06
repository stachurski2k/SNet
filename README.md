# SNet
### Simple Networking Library
---
## Protocol
SNet folows following comunication protocol:
|    | Packet Type | Data Count (int bytes) | Data |
|---|---|---|---|
|Size| 4B          |  4B                    | X    |

For packet type see possible options in packet file

## Usage
Create a server and give it a list of other programs to comunicate with
`myserver = server(my_port=8080 ips=["127.0.0.1"], ports=[8081],names=["other"])`

To recive data subscribe to recive event on apropriate client
`server.named_clients["other"].on_packet_received=function(packet)`

To send data use send to client 
`server.send_to_client("other",packet)`