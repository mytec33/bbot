namespace Wordlebot
{
    public class FileLogger : ILogger
    {
        private string Level = "";

        public FileLogger(string level)
        {
            Level = level;
        }

        public void WriteLine(string message)
        {
            if (Level == "quiet")
            {
                return;
            }

            Console.WriteLine(message);
        }
    }
}