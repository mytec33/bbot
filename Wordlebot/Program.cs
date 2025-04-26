using System;
using Wordlebot;

/*
dotnet run --wordlist-file ./5_letter_words_official.txt  --starting-word magic --wordle covet --result-only
*/

namespace Wordlebot
{
    internal class Program
    {
        private static WordleWordList? WordList;

        static void Main(string[] args)
        {
            try
            {
                var (processedArgsResult, error) = ProcessedArgs(args);
                if (processedArgsResult == null)
                {
                    PrintUsage(error);
                    Environment.Exit(Constants.EXIT_INVALID_ARGS);
                }

                string loggingLevel = "normal";
                if (processedArgsResult.ResultOnly)
                {
                    loggingLevel = "quiet";
                }
                var logger = new ConsoleLogger(loggingLevel);

                try
                {
                    WordList = new WordleWordList(processedArgsResult.WordListFile, logger);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return;
                }

                var game = new WordleGame(logger, WordList, processedArgsResult);
                var result = game.PlayWordle();

                Console.WriteLine(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error starting bbot: {ex.Message}");
            }
        }

        static void PrintUsage(string result)
        {
            Console.WriteLine(result);

            Console.WriteLine("\nUsage:");
            Console.WriteLine("\tdotnet run [--result-only] --wordlist-file file --starting-word guess --wordle wordle");
        }

        static (ProgramArguments? args, string error) ProcessedArgs(string[] args)
        {
            bool resultOnly = false;
            string? startingWord = "";
            string? wordle = "";
            string? wordListFile = "";

            for (int x = 0; x < args.Length; x++)
            {
                if (args[x] == "--result-only")
                {
                    resultOnly = true;
                }
                else if (args[x] == "--starting-word")
                {
                    startingWord = args[++x];
                }
                else if (args[x] == "--wordle")
                {
                    wordle = args[++x];
                }
                else if (args[x] == "--wordlist-file")
                {
                    wordListFile = args[++x];
                }
            }

            if (startingWord.Length != 5)
            {
                throw new Exception("Invalid starting word or no starting word provided: --starting-word some_five_letter_word");
            }

            if (wordle.Length != 5)
            {
                throw new Exception("Invalid Wordle word provide or no value provided: --wordle some_five_letter_word");
            }

            if (wordListFile == "")
            {
                throw new Exception("Wordlist file not provided: --wordlist-file location_to_file");
            }

            return (new ProgramArguments
            {
                ResultOnly = resultOnly,
                StartingWord = startingWord,
                Wordle = wordle,
                WordListFile = wordListFile
            }, "");
        }
    }
}
