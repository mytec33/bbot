using System;
using System.Net.Mail;
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
                var processedArgs = ProcessArgs(args);

                string loggingLevel = "normal";
                if (processedArgs.ResultOnly)
                {
                    loggingLevel = "quiet";
                }
                var logger = new ConsoleLogger(loggingLevel);

                try
                {
                    WordList = new WordleWordList(processedArgs.WordListFile, logger);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return;
                }

                var game = new WordleGame(logger, WordList, processedArgs);
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

        static string GetNextArg(string[] args, ref int x, string argName)
        {
            x++;
            if (x >= args.Length)
            {
                throw new ArgumentException($"{argName} requires a value.");
            }

            var nextValue = args[x];
            if (nextValue.StartsWith("--"))
            {
                throw new ArgumentException($"{argName} requires a value, but found another flag: {nextValue}");
            }

            return args[x];
        }

        static ProgramArguments ProcessArgs(string[] args)
        {
            bool resultOnly = false;
            string startingWord = "";
            string wordle = "";
            string wordListFile = "";

            for (int x = 0; x < args.Length; x++)
            {
                if (args[x] == "--result-only")
                {
                    resultOnly = true;
                }
                else if (args[x] == "--starting-word")
                {
                    startingWord = GetNextArg(args, ref x, "--starting-word");
                }
                else if (args[x] == "--wordle")
                {
                    wordle = GetNextArg(args, ref x, "--wordle");
                }
                else if (args[x] == "--wordlist-file")
                {
                    wordListFile = GetNextArg(args, ref x, "--wordlist-file");
                }
                else
                {
                    throw new ArgumentException($"Unknown arg: {args[x]}");
                }
            }

            // Required flags
            if (startingWord.Length != 5)
            {
                throw new ArgumentException("Invalid or missing starting word.\nUsage: --starting-word some_five_letter_word");
            }

            if (wordle.Length != 5)
            {
                throw new ArgumentException("Invalid or missing Wordle word.\nUsage: --wordle some_five_letter_word");
            }

            if (string.IsNullOrEmpty(wordListFile))
            {
                throw new ArgumentException("Invalid or missing Wordlist file.\nUsage: --wordlist-file location_to_file");
            }

            return new ProgramArguments
            {
                ResultOnly = resultOnly,
                StartingWord = startingWord,
                Wordle = wordle,
                WordListFile = wordListFile
            };
        }
    }
}
