namespace Wordlebot
{
    internal class WordList
    {
        public List<string> Words { get; private set;}

        public WordList(string filename)
        {
            Words = ReadWordList(filename);
        }

        private static List<string> ReadWordList(string path)
        {
            var list = new List<string>();

            if (!File.Exists(path))
            {
                throw new WordListReadingException($"Could not find file: {path}");
            }

            try
            {
                // Should load a user defined library of words
                StreamReader sr = new(path);

                string? line;
                while ((line = sr.ReadLine()) != null)
                {
                    list.Add(line);
                }
            }
            catch (Exception ex)
            {
                throw new WordListReadingException("Error reading word list file.", ex);
            }

            return list;
        }

    }

    public class WordListReadingException : Exception
{
    public WordListReadingException(string message) : base(message)
    {
    }

    public WordListReadingException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
}