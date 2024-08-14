using System.Net;

namespace AlzCapture.Models.Http;

public class HttpResponsePacket : HttpPacket
{
    public HttpStatusCode StatusCode { get; private set; }

    public string ResponseDescription { get; private set; } = string.Empty;

    protected override void ParseCommunication(string communicationLine)
    {
        var communicationInfos = communicationLine.Split("");
        this.StatusCode = (HttpStatusCode)int.Parse(communicationInfos[1]);
        this.ResponseDescription = communicationInfos[2];
    }
}