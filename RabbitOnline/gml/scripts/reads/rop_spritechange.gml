var pid = buffer_read(argument0,buffer_u16);
var faker = players[?pid];
if(is_undefined(faker)){
    //don't request a new faker, it's probably on the main menu
    // netlog_pid(string(pid)+"'s sprite changed,");
    // netlog_pid("but apparently they don't exist i guess");
    // show_message(string(pid))
    // wop_fakereq(pid);
    exit;
}
faker.sprite_index = buffer_read(argument0,buffer_u16);
faker.image_index = buffer_read(argument0,buffer_u16);
faker.image_speed = buffer_read(argument0,buffer_f32);
faker.image_xscale = buffer_read(argument0,buffer_f32);
faker.palette = buffer_read(argument0,buffer_u16);
faker.isRabbit = buffer_read(argument0,buffer_u8) == 1;
faker.rabbitType = buffer_read(argument0,buffer_u8);