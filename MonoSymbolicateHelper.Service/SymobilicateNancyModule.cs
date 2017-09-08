using System;
using System.Configuration;
using System.Runtime.InteropServices;
using MonoSymbolicateHelper.Core;
using Nancy;

namespace MonoSymbolicateHelper.Service
{
    public class SymobilicateNancyModule : NancyModule
    {
        private readonly SymbolicateHelper _helper;

        public SymobilicateNancyModule()
        {
            var archivePath = ConfigurationManager.AppSettings["archivePath"];
            var commandPath = ConfigurationManager.AppSettings["commandPath"];
            _helper = new SymbolicateHelper(archivePath, commandPath);

            Post("/symbolicate", _ =>
            {
                string packageName = Request.Form["packageName"];
                string versionCode = Request.Form["versionCode"];
                string stackTrace = Request.Form["stackTrace"];

                if (string.IsNullOrEmpty(packageName) || string.IsNullOrEmpty(versionCode) ||
                    string.IsNullOrEmpty(stackTrace))
                {
                    return HttpStatusCode.BadRequest;
                }

                stackTrace = stackTrace.Replace("@@@", "\r\n");
                Console.WriteLine($"REQ: {packageName} {versionCode}");
                var result = _helper.Symbolicate(packageName, versionCode, stackTrace);
                return result;
            });
        }
    }
}