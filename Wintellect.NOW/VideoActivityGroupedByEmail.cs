using System.Collections.Generic;
using System.Linq;
using Wintellect.Extensions;

namespace Wintellect.NOW
{
    public class VideoActivityGroupedByEmail : IGrouping<string, VideoAndCompletionPercentage>
    {
        private string _email = null;
        private List<VideoAndCompletionPercentage> _list = null;

        public VideoActivityGroupedByEmail(string email, IEnumerable<VideoAndCompletionPercentage> videoAndCompletionPercentages)
        {
            email.IsNullOrEmpty().ThrowIfTrue("Argument null or empty: email");

            _email = email;

            _list = new List<VideoAndCompletionPercentage>();

            if (!videoAndCompletionPercentages.ThereAreNone())
            {
                foreach (var videoAndCompletionPercentage in videoAndCompletionPercentages)
                    _list.Add(videoAndCompletionPercentage);
            }
        }

        public string Key
        {
            get { return _email; }
        }

        public IEnumerator<VideoAndCompletionPercentage> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public List<VideoAndCompletionPercentage> VideoAndCompletionPercentages
        {
            get
            {
                return _list;
            }
        }
    }
}
