using System;
using Wordlebot;

namespace Wordlebot
{
    internal class Program
    {
        static List<string> Words = new();

        static bool ResultOnly = false;
        static string StartingWord = "";
        static string Wordle = "";
        static string WordListFile = "";

        static void Main(string[] args)
        {
            var usedLetters = new List<string>();

            var processedArgsResult = ProcessedArgs(args);
            if (processedArgsResult != "")
            {
                Console.WriteLine(processedArgsResult);

                Console.WriteLine("\nUsage:");
                Console.WriteLine("\tdotnet run [--result-only] --wordlist-file file --starting-word guess --wordle wordle");
                Environment.Exit(1);
            }

            try
            {
                var wordlist = new WordList(WordListFile);
                Words = wordlist.Words;

                if (Words.Count < 1)
                {
                    Console.WriteLine("No data file in file. Exiting.");
                    return;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }

            string loggingLevel = "normal";
            if (ResultOnly)
            {
                loggingLevel = "quiet";
            }
            Logger logger = new(loggingLevel);

            var game = new WordleGame(logger, Words, StartingWord, Wordle, ResultOnly);
            var result = game.PlayWordle();

            Console.WriteLine(result);
        }

        static string ProcessedArgs(string[] args)
        {
            for (int x = 0; x < args.Length; x++)
            {
                if (args[x] == "--result-only")
                {
                    ResultOnly = true;
                }
                else if (args[x] == "--starting-word")
                {
                    StartingWord = args[++x];
                }
                else if (args[x] == "--wordle")
                {
                    Wordle = args[++x];
                }
                else if (args[x] == "--wordlist-file")
                {
                    WordListFile = args[++x];
                }
            }

            if (StartingWord.Length != 5)
            {
                return "Invalid starting word or no starting word provided: --starting-word some_five_letter_word";
            }

            if (Wordle.Length != 5)
            {
                return "Invalid Wordle word provide or no value provided: --wordle some_five_letter_word";
            }

            if (WordListFile == "")
            {
                return "Wordlist file not provided: --wordlist-file location_to_file";
            }

            return "";
        }
    }
}
