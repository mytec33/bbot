using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wordlebot
{
    internal class Marks
    {
        public int[] marks;

        public Marks()
        {
            ResetMarks();
        }

        private void ResetMarks()
        {
            marks = new int[5] { 0, 0, 0, 0, 0 };
        }

        public int GetTileScore(int index)
        {
            Debug.Assert(index < 5 && index >= 0);

            if (index >= 0 && index < 5)
            {
                return marks[index];
            }

            Environment.Exit(2);
            return -99;
        }

        public bool IsOnlyBlanks()
        {
            foreach (int mark in marks)
            {
                if (mark != 0)
                {
                    return false;
                }
            }

            return true;
        }

        public bool IsOnlyHints()
        {
            foreach (int mark in marks)
            {
                if (mark == 2)
                {
                    return false;
                }
            }

            return true;
        }

        static int WordleHasLetterHint(char letter, string wordle)
        {
            for (int x = 0; x < 5; x++)
            {
                if (wordle[x] == letter)
                {
                    return x;
                }
            }

            return -1;
        }

        public void ScoreWord(string guess, string wordle)
        {
            var requiredLetters = new List<char>();

            ResetMarks();

            for (int x = 0; x < 5; x++)
            {
                // Look for exact matches
                if (guess[x] == wordle[x])
                {
                    marks[x] = 2;
                    Console.WriteLine($"\tLetter '{guess[x]}' exact match");
                    requiredLetters.Add(guess[x]);

                    continue;
                }

                // Look for hints this spot or to the right
                if (WordleHasLetterHint(guess[x], wordle) >= 0)
                {
                    Console.WriteLine($"\tLetter '{guess[x]}' hint match");

                    if (requiredLetters.Contains(guess[x]))
                        marks[x] = 3;
                    else
                        marks[x] = 1;
                }
            }
        }

        public void PrintMarks()
        {
            Console.WriteLine(string.Join(" ", marks));
        }
    }
}
