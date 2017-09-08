using System.Diagnostics;
using System.IO;
using System.Text;

namespace MonoSymbolicateHelper.Core
{
    public class SymbolicateHelper
    {
        private readonly string _commandPath;
        private readonly ArchiveScanner _archiveScanner;

        public SymbolicateHelper(string archivePath, string commandPath)
        {
            _commandPath = commandPath;
            _archiveScanner = new ArchiveScanner(archivePath);
            _archiveScanner.Scan();
        }

        public string Symbolicate(string packageName, string versionCode, string stackTrace)
        {
            var tempFileName = Path.GetTempFileName();
            File.WriteAllText(tempFileName, stackTrace);

            var results = SymbolicateFromFile(packageName, versionCode, tempFileName);

            File.Delete(tempFileName);

            return results;
        }

        public string SymbolicateFromFile(string packageName, string versionCode, string stackTraceFileName)
        {
            var info = _archiveScanner.GetArchiveInfo(packageName, versionCode);

            var results = RunSymoblicateProcess(info, stackTraceFileName);
            return results;
        }


        private string RunSymoblicateProcess(ArchiveInfo info, string stackTraceFileName)
        {
            var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = _commandPath,
                    Arguments = $"-q \"{info.mSymPath}\" \"{stackTraceFileName}\"",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };

            var output = new StringBuilder();

            proc.Start();
            while (!proc.StandardOutput.EndOfStream)
            {
                output.AppendLine(proc.StandardOutput.ReadLine());
            }

            return output.ToString();
        }
    }
}