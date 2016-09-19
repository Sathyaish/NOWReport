using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Wintellect.Extensions;

namespace Wintellect.NOW
{
    public class VideoActivityGroupedByVideo : IGrouping<string, EmailAndCompletionPercentage>
    {
        private string _video = null;
        private List<EmailAndCompletionPercentage> _list = null;

        public VideoActivityGroupedByVideo(string video, IEnumerable<EmailAndCompletionPercentage> emailAndCompletionPercentages = null)
        {
            video.IsNullOrEmpty().ThrowIfTrue("Argument null or empty: video");

            _video = video;

            _list = new List<EmailAndCompletionPercentage>();

            if (!emailAndCompletionPercentages.ThereAreNone())
            {
                foreach (var t in emailAndCompletionPercentages)
                    _list.Add(t);
            }
        }

        public string Key
        {
            get { return _video; }
        }

        public List<EmailAndCompletionPercentage> EmailAndCompletionPercentages
        {
            get
            {
                return _list;
            }
        }

        public IEnumerator<EmailAndCompletionPercentage> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
