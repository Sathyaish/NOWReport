namespace Wintellect.NOW
{
    public class VideoAndCompletionPercentage
    {
        public string Video { get; set; }

        public int CompletionPercentage { get; set; }

        public override string ToString()
        {
            return string.Format("{0}: {1}%", Video, CompletionPercentage);
        }
    }
}