using System.Net;
using System.Net.Sockets;
using System.Runtime.Versioning;
using System.Text;
using Newtonsoft.Json;

[SupportedOSPlatform("windows6.1")]
class Socket 
{

    readonly Impresora impresora;

    public Socket()
    {
        impresora = new Impresora();
    }

    public void StartConnection() 
    {
        const int port = 7000;
        TcpListener server = new TcpListener(IPAddress.Any, port);
        server.Start();
        Console.WriteLine("Server started on port " + port);

        while (true) 
        {
            TcpClient client = server.AcceptTcpClient();

            using (var stream = client.GetStream())
            using (var reader = new StreamReader(stream, Encoding.UTF8))
            using (var writer = new StreamWriter(stream, new UTF8Encoding(false)) { AutoFlush = true })
            {
                try 
                {
                    string jsonMessage = reader.ReadLine()!;
                    var data = JsonConvert.DeserializeObject<dynamic>(jsonMessage)!;
                    var response = impresora.HandleCommand(data);
                    
                    Console.WriteLine(JsonConvert.SerializeObject(response));

                    string jsonResponse = JsonConvert.SerializeObject(response);
                    writer.WriteLine(jsonResponse);
                }
                catch (Exception e) 
                {
                    Console.WriteLine(e.Message);
                }
            }
        
        }
    }
}