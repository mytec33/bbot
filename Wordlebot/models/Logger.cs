namespace Wordlebot
{
    public class Logger
    {
        private string Level = "";

        public Logger(string level)
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