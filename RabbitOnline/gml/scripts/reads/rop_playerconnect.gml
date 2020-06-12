var pid = buffer_read(argument0, buffer_u16);
var quitting = buffer_read(argument0, buffer_u8) == 1;
var inform = buffer_read(argument0, buffer_u8) == 1;
var str = buffer_read(argument0, buffer_string);
if(quitting){
    if (!is_undefined(players[?pid]) && instance_exists(players[?pid]))
    netlog_pid("Player with pid "+string(pid)+" left the game with reason: "+str);
    with(players[?pid]){
        instance_destroy();
        if(inform)updatetextpop(name+" left the game: "+str,0xadd8e6,12,0);
    }
    ds_map_delete(players,pid);
}else{
    netlog_pid(str+" joined the game ("+string(pid)+")");
    if (!is_undefined(players[?pid])) {
        updatetextpop("bug: faker reconnect but there was a faker already there",0xadd8e6,4,0)
        netlog_pid("but they were apparently a faker already");
        exit;
    }
    var faker = instance_create_layer(buffer_read(argument0, buffer_f32),buffer_read(argument0, buffer_f32), obj_player.layer, obj_faker);
    faker.rm = buffer_read(argument0, buffer_u32);
    faker.palette = buffer_read(argument0, buffer_u16);
    faker.name = str;
    faker.pid = pid;
    players[?pid] = faker;
    if(inform)updatetextpop(str+" joined the game. (with pid "+string(pid)+")",0xadd8e6,4,0);
}