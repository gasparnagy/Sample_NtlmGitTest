using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtlmGitTest
{
    class TfsGitSession : IDisposable
    {
        IDisposable httpsDefinition;

        public TfsGitSession()
        {
            // we need to perform this... through reflection
            //httpsDefinition = new SmartSubtransportDefinition<TfsSmartSubtransport>("https://", 2);

            var tfsSmartSubTransportType = Type.GetType("Microsoft.TeamFoundation.Git.CoreServices.TfsSmartSubtransport, Microsoft.TeamFoundation.Git.CoreServices, Version=12.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", true);
            var smartSubtransportDefinitionType = typeof(SmartSubtransportDefinition<>).MakeGenericType(tfsSmartSubTransportType);

            httpsDefinition = (IDisposable)Activator.CreateInstance(smartSubtransportDefinitionType, "https://", 2);
        }

        public void Dispose()
        {
            httpsDefinition.Dispose();
        }
    }
}
