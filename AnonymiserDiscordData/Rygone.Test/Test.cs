using Rygone.Test.Tool;
using Rygone.Tool;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using static Rygone.Tool.CSVEditor;

namespace Rygone.Test
{
    [TestClass]
    public class Test
    {
        #region Ziper
        [TestMethod]
        public void ZiperTest()
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
        #endregion

        #region CSVEditorZiper
        [TestMethod]
        public void CSVEditorTest()
        {
            EditorTest et = new EditorTest(
                "A,B,C,D" + "\r\n" +
                "E,,F,G" + "\r\n" +
                "H,,I,J" + "\r\n" +
                "K,,L,\"M,N\"" + "\r\n" +
                "\r\n"
            );
            ForLine fl = (Line, nb) => new string[] { Line["A"], Line["D"], Line["B"], "C" };
            Edit(et, fl).Wait();
            AssertThat.AreEqual(et.Out, IS.a(
                "A,D,B,C" + "\r\n" +
                "E,G,,C" + "\r\n" +
                "H,J,,C" + "\r\n" +
                "K,\"M,N\",,C" + "\r\n"
            ));
        }
        public class EditorTest : IEditor
        {
            int i;
            public string In;
            public string Out;
            public EditorTest(string In)
            {
                i = 0;
                this.In = In;
                Out = "";
            }
            public void Close() { }
            public bool HasNext() => In.Length > i;
            public char Read() => In[i++];
            public void Write(string data) => Out += data;
        }
        #endregion
    }
}
