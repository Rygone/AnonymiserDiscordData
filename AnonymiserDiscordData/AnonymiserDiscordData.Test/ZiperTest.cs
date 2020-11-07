using AnonymiserDiscordData.Test.Tool;

using AnonymiserDiscordData.Util;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace AnonymiserDiscordData.Test
{
    [TestClass]
    public class ZiperTest
    {
        [TestMethod]
        public void Test()
        {
            string str = null;
            AssertThat.DoesNotThrowException(() => {str = Ziper.TempPath;});
            AssertThat.IsTrue(Directory.Exists(str));
            Directory.CreateDirectory("Test");
            File.WriteAllText(@"Test\Test", "Test");
            AssertThat.DoesNotThrowException(() => { Ziper.Zip("Test.zip", "Test"); });
            AssertThat.DoesNotThrowException(() => { Ziper.UnZip("Test.zip", "Test-copy"); });
            AssertThat.AreEqual(File.ReadAllText(@"Test-copy\Test"), IS.a("Test"));
            File.Delete("Test.zip");
            Directory.Delete("Test", true);
            Directory.Delete("Test-copy", true);
        }
    }
}
