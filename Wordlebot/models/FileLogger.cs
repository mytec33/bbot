namespace Wordlebot
{
    public class FileLogger : ILogger
    {
        private readonly string Level = "";

        public FileLogger(string level)
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