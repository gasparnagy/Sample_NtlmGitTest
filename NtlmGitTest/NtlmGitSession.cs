using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NtlmGitTest
{
    class NtlmGitSession : IDisposable
    {
        IDisposable httpsDefinition;

        public NtlmGitSession(ICredentials credentials = null)
        {
            HttpClientSmartSubtransport.Credentials = credentials;
            httpsDefinition = new SmartSubtransportDefinition<HttpClientSmartSubtransport>("https://", 2);
        }

        public void Dispose()
        {
            httpsDefinition.Dispose();
            HttpClientSmartSubtransport.Credentials = null;
        }
    }
}
