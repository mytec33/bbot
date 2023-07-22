using System.Diagnostics;
using System.Text;

namespace Wordlebot
{
    public class WordleGame
    {
        static bool ResultOnly = false;
        static string FirstGuess = "";
        static string Wordle = "";

        private ILogger Logger { get; init; }
        private readonly WordleWordList WordList;

        public WordleGame(ILogger logger, WordleWordList wordList, string startingWord, string wordle, bool resultsOnly)
        {
            Logger = logger;
            ResultOnly = resultsOnly;
            FirstGuess = startingWord;
            Wordle = wordle;
            WordList = wordList;
        }

        public string PlayWordle()
        {
            Logger.WriteLine($"Words: {WordList.Words.Count:#,##0}");
            Logger.WriteLine($"First guess: {FirstGuess}");
            Logger.WriteLine($"Wordle: {Wordle}\n");

            int attempts = 1;
            string? guess;
            var scoring = new WordleScoring(Logger);

            while (attempts <= Constants.GAME_MAX_GUESSES)
            {
                if (attempts == 1)
                {
                    guess = FirstGuess;
                }
                else
                {
                    guess = WordList.Words[0];
                }

                Logger.WriteLine($"Guess {attempts}: {guess}");

                if (guess == Wordle)
                {
                    if (ResultOnly)
                    {
                        return $"{attempts} tries";
                    }
                    else
                    {
                        return $"You found the wordle in {attempts} tries!";
                    }
                }
                else if (attempts == Constants.GAME_MAX_GUESSES)
                {
                    return $"You didn't find the Wordle. The Wordle is: {Wordle}.";
                }

                Logger.WriteLine($"Scoring word: {guess}");
                scoring.ScoreWord(guess, Wordle);
                scoring.PrintMarks();

                WordList.ReduceWordsBaseOnScore(scoring, guess);
                Logger.WriteLine($"Found {WordList.Words.Count:#,##0} potential words:");
                WordList.PrintWorldList();

                if (WordList.Words.Count > 1)
                {
                    Logger.WriteLine("Scoring with misses");
                    WordList.SortListByMisses();
                }
                else if (WordList.Words.Count < 1)
                {
                    return "No words remaining";
                }

                if (guess == null)
                {
                    return "null";
                }

                attempts++;
            }

            return "Reached unexepected return point";
        }
    }
}