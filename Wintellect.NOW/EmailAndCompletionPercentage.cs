namespace Wintellect.NOW
{
    public class EmailAndCompletionPercentage
    {
        public string Email { get; set; }

        public int CompletionPercentage { get; set; }

        public override string ToString()
        {
            return string.Format("{0}: {1}%", Email, CompletionPercentage);
        }
    }
}
