using AlzCapture.Models.Http;

namespace AlzCapture.Models;

public class HttpCommunicationPacket
{
    public HttpRequestPacket? RequestPacket { get; set; }
    
    public HttpResponsePacket? ResponsePacket { get; set; }
}