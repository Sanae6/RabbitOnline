var buffer = setup_buffer();
buffer_write(buffer,buffer_f32,obj_player.x);//x
buffer_write(buffer,buffer_f32,obj_player.y);//y
buffer_write(buffer,buffer_u32,room);
send_message(2,buffer);