using MonoSymbolicateHelper.Core;
using NUnit.Framework;

namespace MonoSymbolicateHelper.Tests
{
    [TestFixture]
    public class ArchiveTests
    {
        [Test]
        public void TestScanner()
        {
            var scanner = new ArchiveScanner(@"C:\Users\Michael\AppData\Local\Xamarin\Mono for Android\Archives");
            var result = scanner.Scan();
            Assert.True(result);
        }
    }
}
