namespace Wordlebot.Tests
{
    public class Wordlebot_Scoring_Words
    {
        [Fact]
        public void Score_Arose_Brass()
        {
            var marks = new Marks();
            var startWord = "arose";
            var wordle = "brass";
            var expected = "12020";

            marks.ScoreWord(startWord, wordle);
            
            var result = string.Join("", marks.marks);
            Assert.True(result == expected, $"{startWord} -> {wordle} should be {expected} not {result}");
        }

        [Fact]
        public void Score_Arose_Bagel()
        {
            var marks = new Marks();
            var startWord = "arose";
            var wordle = "bagel";
            var expected = "10001";

            marks.ScoreWord(startWord, wordle);
            
            var result = string.Join("", marks.marks);
            Assert.True(result == expected, $"{startWord} -> {wordle} should be {expected} not {result}");
        }

        [Fact]
        public void Score_Arose_Climb()
        {
            var marks = new Marks();
            var startWord = "arose";
            var wordle = "climb";
            var expected = "00000";

            marks.ScoreWord(startWord, wordle);
            
            var result = string.Join("", marks.marks);
            Assert.True(result == expected, $"{startWord} -> {wordle} should be {expected} not {result}");
        }

        public void Score_Arose_Arose()
        {
            var marks = new Marks();
            var startWord = "arose";
            var wordle = "arose";
            var expected = "22222";

            marks.ScoreWord(startWord, wordle);
            
            var result = string.Join("", marks.marks);
            Assert.True(result == expected, $"{startWord} -> {wordle} should be {expected} not {result}");
        }    
    }
}