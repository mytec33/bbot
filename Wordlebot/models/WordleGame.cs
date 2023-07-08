using System.Diagnostics;
using System.Text;

namespace Wordlebot
{
    public class WordleGame
    {
        static bool ResultOnly = false;
        static string StartingWord = "";
        static string Wordle = "";

        private ILogger Logger { get; init; }
        static List<string> Words = new();
        private readonly WordleWordList WordList;

        public WordleGame(ILogger logger, WordleWordList wordList, string startingWord, string wordle, bool resultsOnly)
        {
            Logger = logger;
            ResultOnly = resultsOnly;
            StartingWord = startingWord;
            Wordle = wordle;
            Words = wordList.Words;
            WordList = wordList;
        }

        public string PlayWordle()
        {
            Logger.WriteLine($"Words: {Words.Count:#,##0}");
            Logger.WriteLine($"First word: {StartingWord}");
            Logger.WriteLine($"Wordle: {Wordle}\n");

            int attempts = 1;
            string? guess;
            var scoring = new WordleScoring(Logger);

            while (attempts <= Constants.GAME_MAX_GUESSES)
            {
                if (attempts == 1)
                {
                    guess = StartingWord;
                }
                else
                {
                    // TODO: if more than one item, this should have some heuristic rather
                    // than picking first.
                    //ReduceSet(Words);

                    guess = Words[0];
                }
                WordleWordList.RemoveFrequentLettersByGuess(guess);

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

                // Work through each tile based on current score
                for (int x = 0; x < 5; x++)
                {
                    Logger.WriteLine($"Tile: {x + 1}");

                    int score = scoring.GetTileScore(x);
                    string action = WordleScoring.GetTileScoreDescription(score);

                    if (score == Constants.SCORE_NOT_IN_WORD)
                    {
                        Logger.WriteLine($"\t{guess[x]} is a {action}. Removing from all words");
                        Words = WordleWordList.RemoveWordsWithLetter(guess[x], Words);
                    }
                    else if (score == Constants.SCORE_HINT || score == Constants.SCORE_MATCH_UNUSED)
                    {
                        Logger.WriteLine($"\t{guess[x]} is a {action}. Removing from all words with this letter in this spot: {x + 1}");

                        // Word cannot have hint in this spot, so remove those before we try to find words
                        // with hint elsewhere otherwise this spot will be a false positive
                        Words = WordleWordList.RemoveWordsWithLetterByIndex(x, guess[x], Words);
                        Words = WordleWordList.RemoveWordsWithoutLetter(guess[x], Words);
                    }
                    else if (score == Constants.SCORE_MATCH)
                    {
                        Logger.WriteLine($"\t{guess[x]} is a {action}. Removing from all words without this letter in this spot: {x + 1}");
                        Words = WordleWordList.RemoveWordsWithoutLetterByIndex(x, guess[x], Words);
                    }
                    else if (score == Constants.SCORE_HINT_UNUSED)
                    {
                        Logger.WriteLine("\tHINT_USED hit");
                    }
                }

                Logger.WriteLine($"Found {Words.Count:#,##0} potential words");
                WordleWordList.PrintWorldList(Words, Logger);

                if (Words.Count > 1)
                {
                    if (scoring.NoMisses())
                    {
                        Words = WordList.SortWordsByNoMisses(scoring.marks, guess, Words);
                    }
                    else
                    {
                        Words = WordList.SortListByMisses(Words);
                    }
                }

                if (guess == null)
                {
                    return "null";
                }
                if (Words.Count < 1)
                {
                    return "No words remaining";
                }

                attempts++;
            }

            return "Reached unexepected return point";
        }
    }
}