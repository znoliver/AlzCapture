using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using AlzCapture.Models.Http;
using PacketDotNet;

namespace AlzCapture.Extensions;

internal static class TcpPacketExtension
{
    public static bool IsHttpRequest(this TcpPacket tcpPacket)
    {
        var data = Encoding.ASCII.GetString(tcpPacket.PayloadData);
        if (data.Length <= 0) return false;

        return (data.StartsWith("GET") || data.StartsWith("POST") || data.StartsWith("PUT") ||
                data.StartsWith("DELETE") || data.StartsWith("HEAD") || data.StartsWith("OPTIONS") ||
                data.StartsWith("PATCH") || data.StartsWith("TRACE") || data.StartsWith("CONNECT")) &&
               data.Contains("HTTP/");
    }

    public static bool IsHttpResponse(this TcpPacket tcpPacket)
    {
        var data = Encoding.ASCII.GetString(tcpPacket.PayloadData);
        if (data.Length <= 0) return false;

        return data.StartsWith("HTTP/");
    }

    public static HttpPacket? ConverterToHttpPacket(this TcpPacket tcpPacket, IPPacket ipPacket, bool isRequest)
    {
        HttpPacket result = isRequest ? new HttpRequestPacket() : new HttpResponsePacket();
        var data = Encoding.ASCII.GetString(tcpPacket.PayloadData);
        if (data.Length <= 0) return null;

        result.SourceIp = ipPacket.SourceAddress.ToString();
        result.SourcePort = tcpPacket.SourcePort.ToString();
        result.DestinationIp = ipPacket.DestinationAddress.ToString();
        result.DestinationPort = tcpPacket.DestinationPort.ToString();
        result.SetPayload(tcpPacket.PayloadData);
        
        return result;
    }
}