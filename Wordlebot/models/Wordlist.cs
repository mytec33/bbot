namespace Wordlebot
{
    public class WordList
    {
        public List<string> Words { get; private set; }

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

        static public List<string> RemoveWordsWithLetter(char letter, List<string> list)
        {
            var deletes = list.Where(word => word.Contains(letter)).ToList();
            list.RemoveAll(item => deletes.Contains(item));

            return list;
        }

        static public List<string> RemoveWordsWithLetterByIndex(int index, char letter, List<string> list)
        {
            var deletes = list.Where(word => word[index] == letter).ToList();
            list.RemoveAll(item => deletes.Contains(item));

            return list;
        }

        static public List<string> RemoveWordsWithoutLetter(char letter, List<string> list)
        {
            var keepers = list.Where(word => word.Any(c => c == letter)).ToList();

            return keepers;
        }

        static public List<string> RemoveWordsWithoutLetterByIndex(int index, char letter, List<string> list)
        {
            var deletes = list.Where(word => word[index] != letter).ToList();
            list.RemoveAll(item => deletes.Contains(item));

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