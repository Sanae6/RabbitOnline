inputting = true;//lock the menu? i might need to make something to make sure that happens 
with(obj_net){
    if (url == "" || port == "" || username == ""){
        updatetextpop("Enter the IP, port, and your username.",0xff0000,6,0);
        exit;
    }
    if (alreadyStarting) {
        if (connected){
            wop_disconnect("Attempting to reconnect");
            connected = false;
        }
        updatetextpop("Reattempting a connection...",0xadd8e6,12,0);
        network_destroy(socket);
        socket = network_create_socket(network_socket_tcp);
    }
    network_connect_raw(socket,url,real(port));
    if (!alreadyStarting)updatetextpop("Connecting...",0xadd8e6,12,0);
    alreadyStarting = true;
}