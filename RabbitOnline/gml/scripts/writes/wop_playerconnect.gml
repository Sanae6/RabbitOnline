var buffer = setup_buffer();
buffer_write(buffer,buffer_u16,selfpid);
buffer_write(buffer,buffer_bool,false);
buffer_write(buffer,buffer_bool,true);
buffer_write(buffer,buffer_string,username);
buffer_write(buffer,buffer_f32,0);//x
buffer_write(buffer,buffer_f32,0);//y
buffer_write(buffer,buffer_u32,room);//x
send_message(3,buffer);