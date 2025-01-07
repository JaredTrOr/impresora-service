using System.Runtime.Versioning;

[SupportedOSPlatform("windows6.1")]
class ImpresoraService 
{
    static void Main() 
    {
        Socket server = new Socket();
        server.StartConnection();
    }
}