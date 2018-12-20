using lots.Domain.Interfaces;
using lots.Utilities;

namespace lots.Domain.Services
{
    public class ConsoleService : IConsoleService
    {
        public string ReadLine() => C2.ReadLine();

        public void Write(string message) => C2.Write(message);
        public void WriteDebug(string message) => C2.WriteDebug(message);

        public void WriteError(string message) => C2.WriteError(message);

        public void WriteMessage(string message) => C2.WriteMessage(message);

        public void WriteWarning(string message) => C2.WriteWarning(message);
    }
}
