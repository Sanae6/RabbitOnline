if (async_load[?"id"] != socket) exit;
switch(async_load[?"type"]){
    case network_type_non_blocking_connect:
        netlog("Socket nobc, sending connect packet")
        connected = true;
        wop_connect();
        updatetextpop("Connected, sending username...", 0xADD8E6, 4, false);
        knewaboutdisconnect = false;
        with(obj_mainmenus) {
            global.page = 8;
            inputting = false;
            var gp = global.menu_pages[global.page];
            ds_grid_set(gp,0,4,"DISCONNECT")
            ds_grid_set(gp,1,4,0)
            ds_grid_set(gp,2,4,menu_disconnect);
        }
        break;
    case network_type_disconnect:
        show_message("woah left moment");
        handle_disconnect();
        break;
    case network_type_data:
        var b = async_load[?"buffer"];
        var len = buffer_read(b,buffer_u16);
        var op = buffer_read(b,buffer_u8);
        var pkt = packets[op];
        script_execute(pkt[0], b, len);
        break;
}