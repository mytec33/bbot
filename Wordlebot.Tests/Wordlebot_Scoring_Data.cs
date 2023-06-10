namespace Wordlebot.Tests
{
    public class GetTileScoreTestData : TheoryData<int, string, string, int>
    {
        public GetTileScoreTestData()
        {
            // No matches
            Add(0, "slate", "crumb", 0);
            Add(1, "slate", "crumb", 0);
            Add(2, "slate", "crumb", 0);
            Add(3, "slate", "crumb", 0);
            Add(4, "slate", "crumb", 0);

            // All hints
            Add(0, "slate", "tales", 1);
            Add(1, "slate", "tales", 1);
            Add(2, "slate", "tales", 1);
            Add(3, "slate", "tales", 1);
            Add(4, "slate", "tales", 1);

            // All matches
            Add(0, "crane", "crane", 2);
            Add(1, "crane", "crane", 2);
            Add(2, "crane", "crane", 2);
            Add(3, "crane", "crane", 2);
            Add(4, "crane", "crane", 2);

            // Mixed hints and matches
            Add(0, "arose", "brass", 1);
            Add(1, "arose", "brass", 2);
            Add(2, "arose", "brass", 0);
            Add(3, "arose", "brass", 2);
            Add(4, "arose", "brass", 0);

            // Mixed with unused match
            Add(0, "ditto", "slate", 0);
            Add(1, "ditto", "slate", 0);
            Add(2, "ditto", "slate", 3);
            Add(3, "ditto", "slate", 2);
            Add(4, "ditto", "slate", 0);
        }
    }

    public class ScoreWordTestData : TheoryData<string, string, string>
    {
        public ScoreWordTestData()
        {
            // No match
            Add("arose", "climb", "00000");
            Add("arose", "guppy", "00000");
            Add("slate", "broom", "00000");

            // All hints
            Add("slate", "tales", "11111");
            Add("steal", "least", "11111");

            // All matches
            Add("arose", "arose", "22222");
            Add("crane", "crane", "22222");
            
            // Mixed
            Add("arose", "bagel", "10001");
            Add("arose", "brass", "12020");
            Add("slate", "stale", "21212");
            
            // Mixed with unused match
            Add("ditto", "slate", "00320");
        }
    }
}