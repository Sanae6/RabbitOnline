selfpid = buffer_read(argument0,buffer_u16);
updatetextpop("Connected! Player id: "+string(selfpid),0xadd8e6,4,0);
netlog_pid("Connected!")
wop_playerconnect();