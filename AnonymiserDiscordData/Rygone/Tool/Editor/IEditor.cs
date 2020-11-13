namespace Rygone.Tool.Editor
{
    public interface IEditor
    {
        void Write(string data);
        bool HasNext();
        char Read();
        string ReadLine();
        void Close();
    }
}
