using System.Collections.Generic;
using System.Net;
using System.Text;

namespace AlzCapture.Models.Http;

public class HttpResponsePacket : HttpPacket
{
    public HttpStatusCode StatusCode { get; private set; }

    public string ResponseDescription { get; private set; } = string.Empty;

    protected override void ParseCommunication(string communicationLine)
    {
        var communicationInfos = communicationLine.Split(" ", 3);
        this.StatusCode = (HttpStatusCode)int.Parse(communicationInfos[1]);
        this.ResponseDescription = communicationInfos[2].TrimEnd();
    }

    protected override string ParseBody(List<string> bodyLines)
    {
        if (this.Headers.ContainsKey("Content-Length"))
        {
           return base.ParseBody(bodyLines);
        }

        if(Headers.ContainsKey("Transfer-Encoding") && Headers["Transfer-Encoding"] == "chunked")
        {
            return ParseChunkedResponse(bodyLines);
        }

        return "";
    }
    
    private string ParseChunkedResponse(List<string> bodyLines)
    {
        var result = new StringBuilder();
        for (int i = 0; i < bodyLines.Count; i++)
        {
            if (bodyLines[i] == "0")
            {
                break;
            }

            if ((i & 1) == 0)
            {
                continue;
            }

            result.AppendLine(bodyLines[i]);
        }
            
        return result.ToString();
    }
}