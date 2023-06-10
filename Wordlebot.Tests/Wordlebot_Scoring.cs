namespace Wordlebot.Tests
{
    public class Wordlebot_Scoring
    {
        [Fact]
        public void IsTileScore_Miss()
        {
            var marks = new Marks();
            string result = marks.TileScore(0);

            Assert.True(result == "miss", "should be a miss");
        }

        [Fact]
        public void IsTileScore_Hint()
        {
            var marks = new Marks();
            string result = marks.TileScore(1);

            Assert.True(result == "hint", "should be a hint");
        }

        [Fact]
        public void IsTileScore_Match()
        {
            var marks = new Marks();
            string result = marks.TileScore(2);

            Assert.True(result == "match", "should be a match");
        }

        [Fact]
        public void IsTileScore_UnusedMatch()
        {
            var marks = new Marks();
            string result = marks.TileScore(3);

            Assert.True(result == "unused match", "should be an unused match");
        }

        [Fact]
        public void IsTileScore_LowError()
        {
            var marks = new Marks();
            string resultLow = marks.TileScore(-1);

            Assert.True(resultLow == "error", "should be an error");
        }

        [Fact]
        public void IsTileScore_HighError()
        {
            var marks = new Marks();
            string resultLow = marks.TileScore(4);

            Assert.True(resultLow == "error", "should be an error");
        }        

        [Fact]
        public void Score_Arose_Brass()
        {
            var marks = new Marks();
            var expected = "12020";

            marks.ScoreWord("arose", "brass");
            
            var result = string.Join("", marks.marks);
            Assert.True(result == expected, $"arose -> brass should be {expected} not {result}");
        }
    }
}