/// @description  ()
global.netlog_socket = undefined;
global.netlog_buffer = undefined;
global.netlog_ready = false;
//#netlog_is_ready = (global.netlog_ready)

/// @description  (url, port): Attempts connecting to netlog server (if available).
/// @param url
/// @param  port
var l_skt = network_create_socket(network_socket_tcp);
network_set_config(network_config_use_non_blocking_socket, true);
if (network_connect_raw(l_skt, "localhost", 5101) >= 0) {
    global.netlog_socket = l_skt;
    global.netlog_buffer = buffer_create(16, buffer_grow, 1);
    exit;
}
network_destroy(l_skt);