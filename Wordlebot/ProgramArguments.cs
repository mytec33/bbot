namespace Wordlebot
{
    public record ProgramArguments
    {
        public bool ResultOnly { get; init; }
        public required string StartingWord { get; init; }
        public required string Wordle { get; init; }
        public required string WordListFile { get; init; }
    }
}