using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtlmGitTest
{
    class Program
    {
        static void Main(string[] args)
        {
            const string cloneUrl = "https://tfs02.techtalk.at/tfs/Playground/_git/SpecLog.TestProject.Git";
            var localPath = Environment.ExpandEnvironmentVariables(@"%TMP%\NtlmGitTest");
            
            // ensure that local folder does not exist
            if (Directory.Exists(localPath))
                Directory.Delete(localPath, true);


            Repository.Clone(cloneUrl, localPath);
        }
    }
}
