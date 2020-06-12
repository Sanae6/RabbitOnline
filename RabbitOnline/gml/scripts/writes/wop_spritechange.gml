var buf = setup_buffer();
buffer_write(buf, buffer_u16, argument0);//sprite_index
buffer_write(buf, buffer_u16, argument1);//image_index
buffer_write(buf, buffer_f32, argument2);//image_speed
buffer_write(buf, buffer_f32, argument3);//image_xscale
buffer_write(buf, buffer_u16, argument4);//palette
buffer_write(buf, buffer_u8 , argument5 ? 1 : 0);
send_message(4,buf);