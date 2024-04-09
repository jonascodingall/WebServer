using WebServer;

string ip = "172.24.80.1";
int port = 8001;

var server = new Server(ip, port);

await server.Start();