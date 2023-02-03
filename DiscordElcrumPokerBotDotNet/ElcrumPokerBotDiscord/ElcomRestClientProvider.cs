using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Discord.Net.Rest;


#nullable enable
namespace ElcrumPokerBotDiscord
{
  internal struct MultipartFile
  {
    public Stream Stream { get; }

    public string Filename { get; }

    public string ContentType { get; }

    public MultipartFile(Stream stream, string filename, string contentType = null)
    {
      this.Stream = stream;
      this.Filename = filename;
      this.ContentType = contentType;
    }
  }
  
  public static class ElcomRestClientProvider
  {
    public static readonly RestClientProvider Instance = ElcomRestClientProvider.Create();

    public static RestClientProvider Create(bool useProxy = false) => (RestClientProvider) (url =>
    {
      try
      {
        return (IRestClient) new ElcomRestClient(url, "192.168.0.10", "8080", "fedanda", "5u738mbcyopE4", useProxy);
      }
      catch (PlatformNotSupportedException ex)
      {
        throw new PlatformNotSupportedException("The default RestClientProvider is not supported on this platform.", (Exception) ex);
      }
    });
  }
  
  internal sealed class ElcomRestClient : IRestClient, IDisposable
  {
    private const int HR_SECURECHANNELFAILED = -2146233079;
    private readonly 
    #nullable disable
    HttpClient _client;
    private readonly string _baseUrl;
    private readonly JsonSerializer _errorDeserializer;
    private CancellationToken _cancelToken;
    private bool _isDisposed;
    private static readonly HttpMethod Patch = new HttpMethod("PATCH");

    public ElcomRestClient(string baseUrl, string proxyHost, string proxyPort, string proxyUserName, string proxyPassword, bool useProxy = false)
    {
      this._baseUrl = baseUrl;
      
      var proxy = new WebProxy
      {
        Address = new Uri($"http://{proxyHost}:{proxyPort}"),
        BypassProxyOnLocal = false,
        UseDefaultCredentials = false,

        // *** These creds are given to the proxy server, not the web server ***
        Credentials = new NetworkCredential(
          userName: proxyUserName,
          password: proxyPassword)
      };
      // Now create a client handler which uses that proxy
      var httpClientHandler = new HttpClientHandler
      {
        Proxy = proxy,
        UseProxy = true,
      };
      this._client = new HttpClient(httpClientHandler);
      
      //this._client = new HttpClient((HttpMessageHandler) new HttpClientHandler()
      //{
      //  AutomaticDecompression = (DecompressionMethods.GZip | DecompressionMethods.Deflate),
      //  UseCookies = false,
      //  UseProxy = useProxy
      //});
      this.SetHeader("accept-encoding", "gzip, deflate");
      this._cancelToken = CancellationToken.None;
      this._errorDeserializer = new JsonSerializer();
    }

    private void Dispose(bool disposing)
    {
      if (this._isDisposed)
        return;
      if (disposing)
        this._client.Dispose();
      this._isDisposed = true;
    }

    public void Dispose() => this.Dispose(true);

    public void SetHeader(string key, string value)
    {
      this._client.DefaultRequestHeaders.Remove(key);
      if (value == null)
        return;
      this._client.DefaultRequestHeaders.Add(key, value);
    }

    public void SetCancelToken(CancellationToken cancelToken) => this._cancelToken = cancelToken;

    public async Task<RestResponse> SendAsync(
      string method,
      string endpoint,
      CancellationToken cancelToken,
      bool headerOnly,
      string reason = null,
      IEnumerable<KeyValuePair<string, IEnumerable<string>>> requestHeaders = null)
    {
      string requestUri = Path.Combine(this._baseUrl, endpoint);
      RestResponse restResponse;
      using (HttpRequestMessage restRequest = new HttpRequestMessage(this.GetMethod(method), requestUri))
      {
        if (reason != null)
          restRequest.Headers.Add("X-Audit-Log-Reason", Uri.EscapeDataString(reason));
        if (requestHeaders != null)
        {
          foreach (KeyValuePair<string, IEnumerable<string>> requestHeader in requestHeaders)
            restRequest.Headers.Add(requestHeader.Key, requestHeader.Value);
        }
        restResponse = await this.SendInternalAsync(restRequest, cancelToken, headerOnly).ConfigureAwait(false);
      }
      return restResponse;
    }

    public async Task<RestResponse> SendAsync(
      string method,
      string endpoint,
      string json,
      CancellationToken cancelToken,
      bool headerOnly,
      string reason = null,
      IEnumerable<KeyValuePair<string, IEnumerable<string>>> requestHeaders = null)
    {
      string requestUri = Path.Combine(this._baseUrl, endpoint);
      RestResponse restResponse;
      using (HttpRequestMessage restRequest = new HttpRequestMessage(this.GetMethod(method), requestUri))
      {
        if (reason != null)
          restRequest.Headers.Add("X-Audit-Log-Reason", Uri.EscapeDataString(reason));
        if (requestHeaders != null)
        {
          foreach (KeyValuePair<string, IEnumerable<string>> requestHeader in requestHeaders)
            restRequest.Headers.Add(requestHeader.Key, requestHeader.Value);
        }
        restRequest.Content = (HttpContent) new StringContent(json, Encoding.UTF8, "application/json");
        restResponse = await this.SendInternalAsync(restRequest, cancelToken, headerOnly).ConfigureAwait(false);
      }
      return restResponse;
    }

    /// <exception cref="T:System.InvalidOperationException">Unsupported param type.</exception>
    public async Task<RestResponse> SendAsync(
      string method,
      string endpoint,
      IReadOnlyDictionary<string, object> multipartParams,
      CancellationToken cancelToken,
      bool headerOnly,
      string reason = null,
      IEnumerable<KeyValuePair<string, IEnumerable<string>>> requestHeaders = null)
    {
      string requestUri = Path.Combine(this._baseUrl, endpoint);
      RestResponse restResponse1;
      using (HttpRequestMessage restRequest = new HttpRequestMessage(this.GetMethod(method), requestUri))
      {
        if (reason != null)
          restRequest.Headers.Add("X-Audit-Log-Reason", Uri.EscapeDataString(reason));
        if (requestHeaders != null)
        {
          foreach (KeyValuePair<string, IEnumerable<string>> requestHeader in requestHeaders)
            restRequest.Headers.Add(requestHeader.Key, requestHeader.Value);
        }
        MultipartFormDataContent content = new MultipartFormDataContent("Upload----" + DateTime.Now.ToString((IFormatProvider) CultureInfo.InvariantCulture));
        MemoryStream memoryStream = (MemoryStream) null;
        if (multipartParams != null)
        {
          foreach (KeyValuePair<string, object> p in (IEnumerable<KeyValuePair<string, object>>) multipartParams)
          {
            switch (p.Value)
            {
              case string content3:
                content.Add((HttpContent) new StringContent(content3, Encoding.UTF8, "text/plain"), p.Key);
                continue;
              case byte[] content4:
                content.Add((HttpContent) new ByteArrayContent(content4), p.Key);
                continue;
              case Stream content5:
                content.Add((HttpContent) new StreamContent(content5), p.Key);
                continue;
              case MultipartFile multipartFile:
                MultipartFile fileValue = multipartFile;
                Stream content1 = fileValue.Stream;
                if (!content1.CanSeek)
                {
                  memoryStream = new MemoryStream();
                  await content1.CopyToAsync((Stream) memoryStream).ConfigureAwait(false);
                  memoryStream.Position = 0L;
                  content1 = (Stream) memoryStream;
                }
                StreamContent content2 = new StreamContent(content1);
                ((IEnumerable<string>) fileValue.Filename.Split('.')).Last<string>();
                if (fileValue.ContentType != null)
                  content2.Headers.ContentType = new MediaTypeHeaderValue(fileValue.ContentType);
                content.Add((HttpContent) content2, p.Key, fileValue.Filename);
                continue;
              default:
                throw new InvalidOperationException("Unsupported param type \"" + p.Value.GetType().Name + "\".");
            }
          }
        }
        restRequest.Content = (HttpContent) content;
        RestResponse restResponse2 = await this.SendInternalAsync(restRequest, cancelToken, headerOnly).ConfigureAwait(false);
        memoryStream?.Dispose();
        restResponse1 = restResponse2;
      }
      return restResponse1;
    }

    private async Task<RestResponse> SendInternalAsync(
      HttpRequestMessage request,
      CancellationToken cancelToken,
      bool headerOnly)
    {
      RestResponse restResponse;
      using (CancellationTokenSource cancelTokenSource = CancellationTokenSource.CreateLinkedTokenSource(this._cancelToken, cancelToken))
      {
        cancelToken = cancelTokenSource.Token;
        HttpResponseMessage response = await this._client.SendAsync(request, cancelToken).ConfigureAwait(false);
        Dictionary<string, string> headers = response.Headers.ToDictionary<KeyValuePair<string, IEnumerable<string>>, string, string>((Func<KeyValuePair<string, IEnumerable<string>>, string>) (x => x.Key), (Func<KeyValuePair<string, IEnumerable<string>>, string>) (x => x.Value.FirstOrDefault<string>()), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        Stream stream;
        if (!headerOnly || !response.IsSuccessStatusCode)
          stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
        else
          stream = (Stream) null;
        restResponse = new RestResponse(response.StatusCode, headers, stream);
      }
      return restResponse;
    }

    private HttpMethod GetMethod(string method)
    {
      if (method == "DELETE")
        return HttpMethod.Delete;
      if (method == "GET")
        return HttpMethod.Get;
      if (method == "PATCH")
        return ElcomRestClient.Patch;
      if (method == "POST")
        return HttpMethod.Post;
      if (method == "PUT")
        return HttpMethod.Put;
      throw new ArgumentOutOfRangeException(nameof (method), "Unknown HttpMethod: " + method);
    }
  }
}
