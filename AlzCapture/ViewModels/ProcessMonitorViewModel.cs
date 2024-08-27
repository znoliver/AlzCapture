using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using AlzCapture.Businesses;
using AlzCapture.Businesses.Interfaces;
using AlzCapture.Extensions;
using AlzCapture.Models;
using AlzCapture.Models.Http;
using AlzCapture.Models.Messages;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using PacketDotNet;
using SharpPcap;

namespace AlzCapture.ViewModels;

public partial class ProcessMonitorViewModel(ProcessMonitorModel monitorProcess) : ViewModelBase
{
    private readonly IProcessManager _processManager = ProcessManagerFactory.Create();

    private readonly List<ILiveDevice> _liveDevices = new();

    private readonly Dictionary<Tuple<string, ushort, string, ushort, uint, string>, int> _httpPacketKeyDictionary =
        new();

    public ObservableCollection<HttpCommunicationPacket> HttpCommunicationPackets { get; set; } = new();

    public ProcessMonitorModel MonitorProcess => monitorProcess;

    public string MonitorProcessInfo => $"{MonitorProcess.ProcessName} ({MonitorProcess.ProcessId})";

    [ObservableProperty] private HttpCommunicationPacket? _selectedPacket;

    [ObservableProperty] private bool _isCapturing;


    public void StartCapture()
    {
        var devices = CaptureDeviceList.Instance;

        // If no devices were found print an error
        if (devices.Count < 1)
        {
            return;
        }

        for (int i = 0; i < devices.Count; i++)
        {
            var device = CaptureDeviceList.New()[i];

            try
            {
                // Register our handler function to the 'packet arrival' event
                device.OnPacketArrival += DeviceOnOnPacketArrival;

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

                // device.Filter = "host 192.168.110.68 ";

                // Start the capturing process
                device.StartCapture();

                _liveDevices.Add(device);
            }
            catch (Exception e)
            {
                device.Dispose();
                Console.WriteLine($"监听失败：{device.Name},{e.Message}");
            }
        }

        if (_liveDevices.Count > 0)
        {
            this.IsCapturing = true;
        }
    }

    public void PauseCapture()
    {
        foreach (var device in _liveDevices)
        {
            device.StopCapture();
        }

        this.IsCapturing = false;
    }

    public void StopCapture()
    {
        foreach (var device in _liveDevices)
        {
            device.StopCapture();
            device.Close();
            device.Dispose();
        }

        this.IsCapturing = false;
    }

    public void ClearMessage()
    {
        this.HttpCommunicationPackets.Clear();
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
            var isProcessRequest = _processManager.IsProcessRequestAsync(MonitorProcess.ProcessId,
                ipPacket.SourceAddress.ToString(), tcpPacket.SourcePort.ToString()).Result;

            if (!isProcessRequest)
            {
                return;
            }

            var requestPacket = (HttpRequestPacket)tcpPacket.ConverterToHttpPacket(ipPacket, true, rawCapture.Timeval)!;

            var httpCommunicationPacket = new HttpCommunicationPacket
            {
                RequestPacket = requestPacket,
                HttpMethod = requestPacket.RequestMethod,
                RequestRouter = requestPacket.RequestRouter,
            };

            var key = Tuple.Create(ipPacket.SourceAddress.ToString(), tcpPacket.SourcePort,
                ipPacket.DestinationAddress.ToString(), tcpPacket.DestinationPort,
                tcpPacket.SequenceNumber + (uint)tcpPacket.PayloadData.Length, e.Device.Name);

            Dispatcher.UIThread.InvokeAsync(() => this.HttpCommunicationPackets.Add(httpCommunicationPacket));
            this._httpPacketKeyDictionary.Add(key, this._httpPacketKeyDictionary.Count);
            return;
        }

        if (tcpPacket.IsHttpResponse())
        {
            var ackKey = Tuple.Create(ipPacket.DestinationAddress.ToString(), tcpPacket.DestinationPort,
                ipPacket.SourceAddress.ToString(), tcpPacket.SourcePort,
                tcpPacket.AcknowledgmentNumber, e.Device.Name);

            var hasRequest = this._httpPacketKeyDictionary.TryGetValue(ackKey, out var communicationIndex);
            if (hasRequest)
            {
                var responsePacket =
                    (HttpResponsePacket)tcpPacket.ConverterToHttpPacket(ipPacket, false, rawCapture.Timeval)!;

                var requestTimeval = this.HttpCommunicationPackets[communicationIndex].RequestPacket.CaptureTime;
                var requestTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                    .AddSeconds(requestTimeval.Seconds)
                    .AddMilliseconds(requestTimeval.MicroSeconds / 1000.0);

                var responseTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                    .AddSeconds(rawCapture.Timeval.Seconds)
                    .AddMilliseconds(rawCapture.Timeval.MicroSeconds / 1000.0);

                var communicationTime = (responseTime - requestTime).Milliseconds;
                if (communicationTime == 0)
                {
                    communicationTime = 1;
                }

                Dispatcher.UIThread.InvokeAsync(() =>
                {
                    this.HttpCommunicationPackets[communicationIndex].ResponsePacket = responsePacket;

                    this.HttpCommunicationPackets[communicationIndex].HttpStatusCode = responsePacket.StatusCode;
                    this.HttpCommunicationPackets[communicationIndex].ResponseDescription =
                        responsePacket.ResponseDescription;

                    this.HttpCommunicationPackets[communicationIndex].ResponseTime = $"{communicationTime}ms";
                });
            }
        }
    }
}