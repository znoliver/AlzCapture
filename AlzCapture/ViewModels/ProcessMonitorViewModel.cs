using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using AlzCapture.Businesses;
using AlzCapture.Businesses.Interfaces;
using AlzCapture.Extensions;
using AlzCapture.Models;
using AlzCapture.Models.Http;
using PacketDotNet;
using SharpPcap;

namespace AlzCapture.ViewModels;

public class ProcessMonitorViewModel : ViewModelBase
{
    private readonly Process _monitorProcess;

    private readonly IProcessManager _processManager;

    private readonly Dictionary<Tuple<string, ushort, string, ushort, uint>, int> _httpPacketKeyDictionary;

    public ObservableCollection<HttpCommunicationPacket> HttpCommunicationPackets { get; set; }

    public ProcessMonitorViewModel(Process monitorProcess)
    {
        _monitorProcess = monitorProcess;

        _processManager = ProcessManagerFactory.Create();

        _httpPacketKeyDictionary = new Dictionary<Tuple<string, ushort, string, ushort, uint>, int>();

        HttpCommunicationPackets = new ObservableCollection<HttpCommunicationPacket>();
    }

    public void StartCapture()
    {
        var ver = SharpPcap.Pcap.SharpPcapVersion;
        Console.WriteLine("SharpPcap {0}", ver);

        var devices = CaptureDeviceList.Instance;

        // If no devices were found print an error
        if (devices.Count < 1)
        {
            Console.WriteLine("No devices were found on this machine");
            return;
        }

        for (int i = 0; i < devices.Count; i++)
        {
            var device = CaptureDeviceList.New()[i];

            try
            {
                // Register our handler function to the 'packet arrival' event
                device.OnPacketArrival += DeviceOnOnPacketArrival;

                // device.Filter = "ip dest 192.168.110.86";
                // Open the device for capturing
                try
                {
                    int readTimeoutMilliseconds = 1000;
                    device.Open(read_timeout: readTimeoutMilliseconds);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

                device.Filter = "host 192.168.110.68 ";

                // Start the capturing process
                device.StartCapture();

                Console.WriteLine($"开始监听{device.Name}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"监听失败：{device.Name},{e.Message}");
            }
        }
    }

    private void DeviceOnOnPacketArrival(object sender, PacketCapture e)
    {
        var rawCapture = e.GetPacket();
        var packet = Packet.ParsePacket(rawCapture.LinkLayerType, rawCapture.Data);
        var ipPacket = packet.Extract<IPPacket>();
        if (ipPacket == null) return;

        var tcpPacket = packet.Extract<TcpPacket>();
        if (tcpPacket == null) return;

        var payloadData = Encoding.ASCII.GetString(tcpPacket.PayloadData);
        if (payloadData.Length <= 0) return;

        if (tcpPacket.IsHttpRequest())
        {
            var key = Tuple.Create(ipPacket.SourceAddress.ToString(), tcpPacket.SourcePort,
                ipPacket.DestinationAddress.ToString(), tcpPacket.DestinationPort, tcpPacket.SequenceNumber);

            var httpCommunicationPacket = new HttpCommunicationPacket
            {
                RequestPacket = (HttpRequestPacket)tcpPacket.ConverterToHttpPacket(ipPacket, true)!
            };

            this.HttpCommunicationPackets.Add(httpCommunicationPacket);
            this._httpPacketKeyDictionary.Add(key, this._httpPacketKeyDictionary.Count);
            return;
        }

        if (tcpPacket.IsHttpResponse())
        {
            var ackKey = Tuple.Create(ipPacket.DestinationAddress.ToString(), tcpPacket.DestinationPort,
                ipPacket.SourceAddress.ToString(), tcpPacket.SourcePort, tcpPacket.AcknowledgmentNumber - 1);
            var hasRequest = this._httpPacketKeyDictionary.TryGetValue(ackKey, out var communicationIndex);
            if (hasRequest)
            {
                this.HttpCommunicationPackets[communicationIndex].ResponsePacket =
                    (HttpResponsePacket)tcpPacket.ConverterToHttpPacket(ipPacket, false)!;
            }
        }
    }
}