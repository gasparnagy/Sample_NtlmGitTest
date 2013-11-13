using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtlmGitTest
{
    class NtlmGitSession : IDisposable
    {
        IDisposable httpsDefinition;

        public NtlmGitSession()
        {
            httpsDefinition = new SmartSubtransportDefinition<HttpClientSmartSubtransport>("https://", 2);
        }

        public void Dispose()
        {
            httpsDefinition.Dispose();
        }
    }
}
