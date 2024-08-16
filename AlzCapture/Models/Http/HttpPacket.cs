using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlzCapture.Models.Http;

public class HttpPacket
{
    public string SourceIp { get; set; } = string.Empty;

    public string SourcePort { get; set; } = string.Empty;

    public string DestinationIp { get; set; } = string.Empty;

    public string DestinationPort { get; set; } = string.Empty;

    public HttpContentType ContentType { get; private set; }

    public int ContentLength { get; private set; }

    public IDictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();

    public byte[] PayloadData { get; private set; } = [];

    public string? Body { get; private set; }

    /// <summary>
    /// Method to set the payload of the http packet and parse the data to the http packet properties
    /// </summary>
    public void SetPayload(byte[] payloadData)
    {
        this.PayloadData = payloadData;
        var data = Encoding.ASCII.GetString(payloadData);

        Console.WriteLine(data);
        var lines = data.Split(Environment.NewLine).ToList();
        var index = lines.FindIndex(string.IsNullOrWhiteSpace);

        var communicationLine = lines[0];
        var headerLines = lines.GetRange(1, index - 1);
        var bodyLines = lines.GetRange(index + 1, lines.Count - index - 1);

        ParseCommunication(communicationLine);
        ParseHttpPacketHeader(headerLines);
        this.Body = string.Join(Environment.NewLine, bodyLines);
    }

    protected virtual void ParseCommunication(string communicationLine)
    {
    }

    private void ParseHttpPacketHeader(List<string> headerLines)
    {
        foreach (var headerContent in headerLines.Select(headerLine => headerLine.Split(":", 2))
                     .Where(headerContent => headerContent.Length == 2))
        {
            this.Headers.Add(headerContent[0], headerContent[1].TrimEnd());
            if (headerContent[0] == "Content-Length")
            {
                this.ContentLength = int.Parse(headerContent[1].Trim());
            }
            else if (headerContent[0] == "Content-Type")
            {
                // this.ContentType = (HttpContentType)Enum.Parse(typeof(HttpContentType), headerContent[1].Trim());
            }
        }
    }
}