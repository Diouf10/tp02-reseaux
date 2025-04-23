// See https://aka.ms/new-console-template for more information
//Console.WriteLine("Hello, World!");

using System.Net;
using System.Net.Sockets;
using tp02_reseaux;

const int PORT = 8088;

TcpListener listener = new TcpListener(IPAddress.Any, PORT);

HttpServer server = new HttpServer(listener);

server.Start();




































