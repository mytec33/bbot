using System.Diagnostics;
using System.Text;

namespace Wordlebot
{
    public class Scoring
    {
        private ILogger Logger { get; init; }

        public int[] marks = new int[5] { 0, 0, 0, 0, 0 };

        public Scoring(ILogger fileLogger)
        {
            Logger = fileLogger;
        }

        private void ResetMarks()
        {
            marks = new int[5] { 0, 0, 0, 0, 0 };
        }

        public int GetTileScore(int index)
        {
            if (index >= 0 && index < 5)
            {
                return marks[index];
            }

            throw new InvalidOperationException("Tile score: index out of range.");
        }

        public bool NoMisses()
        {
            return marks.All(mark => mark == Constants.SCORE_MATCH || mark == Constants.SCORE_HINT);
        }

        public static string GetTileScoreDescription(int score) =>
            score switch
            {
                0 => "miss",
                1 => "hint",
                2 => "match",
                3 => "unused hint",
                4 => "unused match",
                < 0 => "error",
                > 4 => "error"
            };

        public static bool WordleHasLetterHint(char letter, string wordle)
        {
            return wordle.Contains(letter);
        }

        public void ScoreWord(string guess, string wordle)
        {
            var requiredLetters = new List<char>();

            ResetMarks();

            // Establish matches first so we can later determine if multiple occurences
            // of a letter are a hint or a miss based on what is matched
            for (int x = 0; x < 5; x++)
            {
                // Look for exact matches
                if (guess[x] == wordle[x])
                {
                    marks[x] = 2;
                    requiredLetters.Add(guess[x]);
                }
            }

            // Once we establish matches we can see how many times a given
            // letter occurs and if it's a hint or a miss. E.g., 1 s is found
            // and there is only 1 s in the word, additional s would be a miss
            for (int x = 0; x < 5; x++)
            {
                // Look for hints this spot or to the right
                if (WordleHasLetterHint(guess[x], wordle))
                {
                    if (requiredLetters.Contains(guess[x]))
                    {
                        int foundCount = 0;
                        int occurrences = 0;

                        // LINQ??
                        foreach (char c in requiredLetters)
                        {
                            if (c == guess[x])
                            {
                                foundCount++;
                            }
                        }

                        foreach (char c in wordle)
                        {
                            if (c == guess[x])
                            {
                                occurrences++;
                            }
                        }

                        if (marks[x] == 2)
                        {
                            Logger.WriteLine($"\tLetter '{guess[x]}' exact match");
                        }
                        else if (foundCount == occurrences && marks[x] == 2)
                        {
                            Logger.WriteLine($"\tLetter '{guess[x]}' exact match");
                        }
                        else if (foundCount < occurrences && marks[x] != 2)
                        {
                            Logger.WriteLine($"\tLetter '{guess[x]}' hint match");
                            marks[x] = 1;
                        }
                        else
                        {
                            Logger.WriteLine($"\tLetter '{guess[x]}' is an unused match");
                            marks[x] = 4;
                        }

                        Logger.WriteLine($"Occurances of '{guess[x]}' is {occurrences} / {foundCount}");
                    }
                    else
                        marks[x] = 1;
                }
            }
        }

        public void PrintMarks()
        {
            // Spaces at the end intentional
            string darkGrayBlock = "\u001b[90m█\u001b[0m ";
            string greenBlock = "\u001b[32m█\u001b[0m ";
            string yellowBlock = "\u001b[33;1m█\u001b[0m ";

            var tiles = new StringBuilder();
            var values = new StringBuilder();

            foreach (int m in marks)
            {
                if (m == 1)
                {
                    tiles.Append(yellowBlock);
                }
                else if (m == 2)
                {
                    tiles.Append(greenBlock);
                }
                else
                {
                    tiles.Append(darkGrayBlock);
                }

                values.Append($"{m} ");
            }

            Logger.WriteLine(tiles.ToString());
            Logger.WriteLine(values.ToString());
        }
    }
}
