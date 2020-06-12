/// @description  (): Should be added to Async - Networking event
if (async_load[?"type"] == network_type_data
    && async_load[?"id"] == global.netlog_socket
) {
    global.netlog_ready = buffer_read(async_load[?"buffer"], buffer_string) == "NETLOG OK";
}