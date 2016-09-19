using System;
using System.Globalization;
using Wintellect.Extensions;

namespace Wintellect.NOW
{
    public class VideoActivity
    {
        public enum ColumnIndex
        {
            When,
            Email,
            Video,
            CompletionPercentage
        }

        public DateTime When { get; set; }
        
        public string Email { get; set; }

        public string Video { get; set; }

        public int CompletionPercentage { get; set; }


        public VideoActivity() { }


        public static explicit operator VideoActivity(CSVFile.Row row)
        {
            return Validate(row);
        }

        public static VideoActivity Validate(CSVFile.Row row)
        {
            // TO DO: Make all of these validations configurable

            DateTime when;

            var success = DateTime.TryParseExact(row[(int)ColumnIndex.When], "u", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out when);

            success.ThrowIfFalse("The value at row #{0} in the {1} column in file {2} is not a valid UTC date and time.", 
                row.Index + 1, ColumnIndex.When.ToString(), row.File);

            // TO DO: Make this check configurable
            var email = row[(int)ColumnIndex.Email];
            email.IsValidEmail().ThrowIfFalse("The value at row #{0} in the {1} column in file {2} does not appear to be a valid email address.", 
                row.Index + 1, ColumnIndex.Email.ToString(), row.File);

            var video = row[(int)ColumnIndex.Video];
            video.IsNullOrEmpty().ThrowIfTrue("The value at row #{0} in the {1} column in file {2} does not contain any video information.", 
                row.Index + 1, ColumnIndex.Video.ToString(), row.File);

            int completionPercentage;

            int.TryParse(row[(int)ColumnIndex.CompletionPercentage], out completionPercentage)
                .ThrowIfFalse("The value at row #{0} in the {1} column in file {2} is not an integer.",
                row.Index + 1, ColumnIndex.CompletionPercentage.ToString(), row.File);

            return new VideoActivity
            {
                When = when,
                Email = email.Trim(),
                Video = video.Trim(),
                CompletionPercentage = completionPercentage
            };
        }

        public override string ToString()
        {
            return string.Format("{0}\t{1}\t{2}\t{3}%", When, Email, Video, CompletionPercentage);
        }
    }
}