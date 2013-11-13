using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.IO;

namespace NtlmGitTest
{
    [RpcSmartSubtransport]
    public class HttpClientSmartSubtransport : SmartSubtransport
    {
        private HttpClient httpClient = null;

        protected override SmartSubtransportStream Action(string url, GitSmartSubtransportAction action)
        {
            string postContentType = null;
            Uri serviceUri;
            switch (action)
            {
                case GitSmartSubtransportAction.UploadPackList:
                    serviceUri = new Uri(url + "/info/refs?service=git-upload-pack");
                    break;
                case GitSmartSubtransportAction.UploadPack:
                    serviceUri = new Uri(url + "/git-upload-pack");
                    postContentType = "application/x-git-upload-pack-request";
                    break;
                case GitSmartSubtransportAction.ReceivePackList:
                    serviceUri = new Uri(url + "/info/refs?service=git-receive-pack");
                    break;
                case GitSmartSubtransportAction.ReceivePack:
                    serviceUri = new Uri(url + "/git-receive-pack");
                    postContentType = "application/x-git-receive-pack-request";
                    break;
                default:
                    throw new InvalidOperationException();
            }
            if (httpClient == null)
            {
                httpClient = BuildHttpClientForUri(serviceUri);
            }

            return new HtppClientSmartSubtransportStream(this, httpClient, serviceUri, postContentType);
        }

        private HttpClient BuildHttpClientForUri(Uri serviceUri)
        {
            HttpClientHandler handler = new HttpClientHandler();
            handler.UseDefaultCredentials = true;

            var client = new HttpClient(handler);
            client.Timeout = TimeSpan.FromMinutes(5.0);
            return client;
        }

    }

    public class HtppClientSmartSubtransportStream : SmartSubtransportStream
    {
        private readonly HttpClient httpClient;
        private readonly Uri serviceUri;
        private readonly string postContentType;
        private Stream responseStream;
        private MemoryStream requestSream = null;

        public HtppClientSmartSubtransportStream(SmartSubtransport smartSubtransport, HttpClient httpClient, Uri serviceUri, string postContentType)
            : base(smartSubtransport)
        {
            this.httpClient = httpClient;
            this.serviceUri = serviceUri;
            this.postContentType = postContentType;
        }

        private void EnsureResponseStream()
        {
            if (this.responseStream != null)
                return;

            HttpResponseMessage result;
            if (requestSream != null) // we also have something to send
            {
                requestSream.Seek(0, SeekOrigin.Begin);
                var streamContent = new StreamContent(requestSream);

                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, serviceUri);
                if (!string.IsNullOrEmpty(postContentType))
                    streamContent.Headers.Add("Content-Type", postContentType);
                request.Content = streamContent;
                result = httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead).Result;
            }
            else
            {
                result = httpClient.GetAsync(serviceUri, HttpCompletionOption.ResponseHeadersRead).Result;
            }
            responseStream = result.EnsureSuccessStatusCode().Content.ReadAsStreamAsync().Result;
        }

        public override int Read(Stream dataStream, long length, out long bytesRead)
        {
            bytesRead = 0L;
            var buffer = new byte[65536];
            this.EnsureResponseStream();

            int count;
            while (length > 0 && (count = this.responseStream.Read(buffer, 0, (int)Math.Min(buffer.Length, length))) > 0)
            {
                dataStream.Write(buffer, 0, count);
                bytesRead += (long)count;
                length -= (long)count;
            }
            return 0;
        }

        public override int Write(Stream dataStream, long length)
        {
            if (requestSream == null)
                requestSream = new MemoryStream();

            var buffer = new byte[65536];
            int count;
            while (length > 0 && (count = dataStream.Read(buffer, 0, (int)Math.Min(buffer.Length, length))) > 0)
            {
                requestSream.Write(buffer, 0, count);
                length -= (long)count;
            }
            return 0;
        }

        protected override void Dispose()
        {
            if (this.responseStream != null)
                this.responseStream.Dispose();
            if (this.requestSream != null)
                this.requestSream.Dispose();
            base.Dispose();
        }
    }
}
