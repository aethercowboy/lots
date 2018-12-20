namespace lots.Domain.Interfaces
{
    public interface IConsoleService
    {
        string ReadLine();
        void Write(string message);
        void WriteMessage(string message);
        void WriteError(string message);
        void WriteWarning(string message);
        void WriteDebug(string message);
    }
}
