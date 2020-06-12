/// @description  ()
var l_skt = global.netlog_socket;
if (l_skt != undefined) {
    network_destroy(l_skt);
    global.netlog_socket = undefined;
}
var l_buf = global.netlog_buffer;
if (l_buf != undefined) {
    buffer_delete(l_buf);
    global.netlog_buffer = undefined;
}
global.netlog_ready = false;