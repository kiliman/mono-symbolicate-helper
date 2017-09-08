using System.IO;
using MonoSymbolicateHelper.Core;
using NUnit.Framework;

namespace MonoSymbolicateHelper.Tests
{
    [TestFixture]
    public class SymbolicateTests
    {
        [Test]
        public void TestSymbolicate()
        {
            var archivePath = @"C:\Users\Michael\AppData\Local\Xamarin\Mono for Android\Archives";
            var commandPath = @"C:\Program Files (x86)\MSBuild\Xamarin\Android\mono-symbolicate.exe";
            var helper = new SymbolicateHelper(archivePath, commandPath);

            var packageName = "com.onezerone.fuse.mobile_test";
            var versionCode = "30906";
            var stackTraceFileName = @"C:\Temp\stacktrace.txt";
            var result = helper.SymbolicateFromFile(packageName, versionCode, stackTraceFileName);
            Assert.IsNotEmpty(result);
        }
    }
}