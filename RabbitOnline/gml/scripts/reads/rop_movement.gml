var pid = buffer_read(argument0,buffer_u16);
var faker = players[?pid];
if(is_undefined(faker)){
    //do nothing, it's probably on the main menu
    exit;
}
faker.x = buffer_read(argument0,buffer_f32);
faker.y = buffer_read(argument0,buffer_f32);
faker.rm = buffer_read(argument0,buffer_u32);
//netlog_pid(string(pid)+"'s position changed! ("+string(faker.x)+","+string(faker.y)+")");
