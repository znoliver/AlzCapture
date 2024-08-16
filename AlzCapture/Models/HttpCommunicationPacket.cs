using System.Net;
using System.Net.Http;
using AlzCapture.Models.Http;
using AlzCapture.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AlzCapture.Models;

public partial class HttpCommunicationPacket : ViewModelBase
{
    public HttpRequestPacket RequestPacket { get; set; } = null!;

    public HttpResponsePacket? ResponsePacket { get; set; }

    [ObservableProperty] private HttpMethod? _httpMethod;

    [ObservableProperty] private string _requestRouter = "";

    [ObservableProperty] private HttpStatusCode? _httpStatusCode = null;

    [ObservableProperty] private string _responseDescription = "";

    [ObservableProperty] private string _responseTime = "";
}