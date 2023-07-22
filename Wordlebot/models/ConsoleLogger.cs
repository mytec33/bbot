namespace Wordlebot
{
    public class ConsoleLogger : ILogger
    {
        private readonly string Level = "";

        public ConsoleLogger(string level)
        {
            Level = level;
        }

        public void WriteLine(string message)
        {
            if (Level.ToLower() == "quiet")
            {
                return;
            }

            Console.WriteLine(message);
        }
    }
}