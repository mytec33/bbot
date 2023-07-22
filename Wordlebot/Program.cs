using System;
using Wordlebot;

/*
dotnet run --wordlist-file ./5_letter_words_official.txt  --starting-word magic --wordle covet --result-only
*/

namespace Wordlebot
{
    internal class Program
    {
        private static bool ResultOnly = false;
        private static string StartingWord = "";
        private static string Wordle = "";
        private static string WordListFile = "";
        private static WordleWordList? WordList;

        static void Main(string[] args)
        {
            var processedArgsResult = ProcessedArgs(args);
            if (processedArgsResult != "")
            {
                PrintUsage(processedArgsResult);
                Environment.Exit(Constants.EXIT_INVALID_ARGS);
            }

            string loggingLevel = "normal";
            if (ResultOnly)
            {
                loggingLevel = "quiet";
            }
            var logger = new ConsoleLogger(loggingLevel);

            try
            {
                WordList = new WordleWordList(WordListFile, logger);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }

            var game = new WordleGame(logger, WordList, StartingWord, Wordle, ResultOnly);
            var result = game.PlayWordle();

            Console.WriteLine(result);
        }

        static void PrintUsage(string result)
        {
            Console.WriteLine(result);

            Console.WriteLine("\nUsage:");
            Console.WriteLine("\tdotnet run [--result-only] --wordlist-file file --starting-word guess --wordle wordle");
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
