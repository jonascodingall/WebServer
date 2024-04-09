using WebClient;

string ip = "172.24.80.1";
int port = 8001;

var client = new Client(ip, port);

await client.Start();