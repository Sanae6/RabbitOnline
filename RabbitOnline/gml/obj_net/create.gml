socket = network_create_socket(network_socket_tcp);
alreadyStarting = false;
global._net = id;
connected = false;
username = "Sanae";
url = "localhost";
port = "7773"
version = 1;
packets = [
    [rop_connect,wop_connect],
    [rop_disconnect,wop_disconnect],
    [rop_movement,wop_movement],
    [rop_playerconnect,wop_playerconnect],
    [rop_spritechange,wop_spritechange],
    [rop_ping,wop_ping],
]
sidx = spr_playeridle;
iidx = 0;
iidxtimer = 5;
pal = -1;
players = ds_map_create();