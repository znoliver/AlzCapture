using System;
using System.Net.Http;

namespace AlzCapture.Models.Http;

public class HttpRequestPacket : HttpPacket
{
    public HttpMethod RequestMethod { get; private set; }

    public string RequestRouter { get; private set; } = string.Empty;

    public string HttpVersion { get; private set; } = string.Empty;

    protected override void ParseCommunication(string communicationLine)
    {
        var communicationInfos = communicationLine.Split(" ");

        this.RequestMethod = HttpMethod.Parse(communicationInfos[0]);
        this.RequestRouter = communicationInfos[1];
        this.HttpVersion = communicationInfos[2];
    }
}