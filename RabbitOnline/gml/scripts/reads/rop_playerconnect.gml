quitting = buffer_read(argument0, buffer_bool);
str = buffer_read(argument0, buffer_string);
if(quitting){
    updatetextpop("Connected! Player id: "+string(pid),0xadd8e6,12,0);
}else{
    var faker = instance_create_layer(buffer_read(argument0, buffer_f32),buffer_read(argument0, buffer_f32), "Instances", obj_faker);
    faker.rm = buffer_read(argument0, buffer_u32);
    faker.palette = buffer_read(argument0, buffer_u16);
    faker.map = ds_map_create();
    
}