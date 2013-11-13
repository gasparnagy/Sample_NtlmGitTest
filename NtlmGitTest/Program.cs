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
                DirectoryDeleteAll(localPath);

            using (new NtlmGitSession())
            {
                Repository.Clone(cloneUrl, localPath);
            }

            Console.WriteLine("Repository cloned! Top level content:");
            foreach (var item in Directory.GetFileSystemEntries(localPath))
            {
                Console.WriteLine("  {0}", item);
            }
        }

        // source: http://stackoverflow.com/questions/611921/how-do-i-delete-a-directory-with-read-only-files-in-c
        public static void DirectoryDeleteAll(string directoryPath)
        {
            var rootInfo = new DirectoryInfo(directoryPath) { Attributes = FileAttributes.Normal };
            foreach (var fileInfo in rootInfo.GetFileSystemInfos()) fileInfo.Attributes = FileAttributes.Normal;
            foreach (var subDirectory in Directory.GetDirectories(directoryPath, "*", SearchOption.AllDirectories))
            {
                var subInfo = new DirectoryInfo(subDirectory) { Attributes = FileAttributes.Normal };
                foreach (var fileInfo in subInfo.GetFileSystemInfos()) fileInfo.Attributes = FileAttributes.Normal;
            }
            Directory.Delete(directoryPath, true);
        }
    }

}
