/// @description  (message): Sends a message either to server (if connected) or to show_debug_message.
/// @param message
var l_msg = argument0;
if (global.netlog_ready) {
    var l_buf = global.netlog_buffer;
    buffer_seek(l_buf, buffer_seek_start, 0);
    buffer_write(l_buf, buffer_text, l_msg);
    buffer_write(l_buf, buffer_u8, 10);
    if (network_send_raw(global.netlog_socket, l_buf, buffer_tell(l_buf)) >= 0) exit;
}
exit;show_debug_message("Netlog is off!")
game_end();
//show_debug_message(l_msg); no debug messages
