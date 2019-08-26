using RLBotDotNet;
using System.IO;

namespace Geoff
{
    class Program
    {
        static void Main()
        {
            // Read the port from port.cfg.
            const string file = "port.cfg";
            string text = File.ReadAllLines(file)[0];
            int port = int.Parse(text);

            // BotManager is a generic which takes in your bot as its T type.
            BotManager<GeoffBot> botManager = new BotManager<GeoffBot>();
            // Start the server on the port given in the port.cfg file.
            botManager.Start(port);
        }
    }
}
