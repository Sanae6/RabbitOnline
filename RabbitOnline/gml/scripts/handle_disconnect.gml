ds_map_destroy(players);//destroy
with (obj_faker){
    instance_destroy();
}
players = ds_map_create();//create
if (!knewaboutdisconnect) updatetextpop("The connection to the server was unexpectedly closed.",0xadd8e6,12,0);