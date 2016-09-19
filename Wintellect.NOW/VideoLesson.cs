using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Wintellect.Extensions;

namespace Wintellect.NOW
{
    public class VideoLesson
    {
        public string VideoTitle { get; set; }
        public string Instruction { get; set; }
        public IList<Question> Questions { get; set; }

        public class Question
        {
            public enum ColumnIndex
            {
                Marks = 0,
                Concept,
                QuestionText,
                NumberOfOptions,
                AnswerKey,
                Option1,
                Option2,
                Option3,
                Option4,
                Option5,
                TimestampString
            }

            private string _timestampString;

            public int Marks { get; set; }
            public string Concept { get; set; }
            public string QuestionText { get; set; }
            public string NumberOfOptions { get; set; }
            public string AnswerKey { get; set; }
            public IList<string> Options { get; set; }
            public TimeSpan Timestamp { get; protected set; }

            public virtual string TimestampString 
            { 
                get
                {
                    return _timestampString;
                }
                set
                {
                    OnBeforeTimestampStringSetValue(value);

                    _timestampString = value;
                }
            }

            protected void OnBeforeTimestampStringSetValue(string value)
            {
                SetTimespanFromTimestampString(value);
            }

            protected virtual void SetTimespanFromTimestampString(string value)
            {
                try
                {
                    value = EnsureTimestampFormat(value);

                    Timestamp = TimeSpan.ParseExact(value, "hh\\:mm\\:ss", CultureInfo.InvariantCulture);
                }
                catch (FormatException formatException)
                {
                    string message = string.Format("The timestamp value {0} is not in the expected format hh:mm:ss. The hh:mm is optional. You should provide at least a :ss", value);

                    throw new Exception(message, formatException);
                }
                catch (OverflowException overflowException)
                {
                    string message = string.Format("The timestamp value {0} is not in the expected format hh:mm:ss. The hh:mm is optional. You should provide at least a :ss", value);

                    throw new Exception(message, overflowException);
                }
                catch (Exception exception)
                {
                    string message = string.Format("An error occurred while parsing the timestamp value {0}. Please see the inner exception for details", value);

                    throw new Exception(message, exception);
                }
            }

            private string EnsureTimestampFormat(string value)
            {
                (value.IsNull() || value.IsNullOrWhitespace()).ThrowIfTrue("Timestamp value missing.");

                // [[\d:{0,2}][\:]]?[\d{1,2}]:[\d{2}]
                var values = value.Trim().Split(':');

                (values.Length == 0 || values.Length > 3).ThrowIfTrue("The timestamp value {0} is not valid.", value);

                values.All(v =>
                    {
                        int temp;
                        return int.TryParse(v, out temp);
                    });

                // :34
                // 2:34
                // 1:2:34
                return values.Length == 1
                    ?
                    string.Format("00:00:{0:00}", values[0])
                    :
                    (values.Length == 2
                        ?
                        string.Format("00:{0:00}:{1:00}", values[0], values[1])
                        :
                        string.Format("{0:00}:{1:00}:{2:00}", values[0], values[1], values[2]));
            }

            public static explicit operator Question(CSVFile.Row row)
            {
                return new Question
                {
                    Marks = int.Parse(row[(int)ColumnIndex.Marks]),
                    Concept = row[(int)ColumnIndex.Concept],
                    QuestionText = row[(int)ColumnIndex.QuestionText],
                    NumberOfOptions = row[(int)ColumnIndex.NumberOfOptions],
                    AnswerKey = row[(int)ColumnIndex.AnswerKey],

                    Options = new List<string> 
                    { 
                        row[(int)ColumnIndex.Option1], 
                        row[(int)ColumnIndex.Option2], 
                        row[(int)ColumnIndex.Option3], 
                        row[(int)ColumnIndex.Option4], 
                        row[(int)ColumnIndex.Option5] 
                    },

                    TimestampString = row[(int)ColumnIndex.TimestampString]
                };
            }
        }

        public VideoLesson() { }

        public VideoLesson(string fromCsvFile) : this(new Uri(new FileInfo(fromCsvFile).FullName)) { }

        public VideoLesson(Uri fromCsvUri)
        {
            ValidateSourceUri(fromCsvUri);

            Questions = new List<Question>();

            using(var csvFile = new CSVFile(fromCsvUri.LocalPath))
            {
                foreach (var row in csvFile.Rows)
                {
                    if (row.Index == 0)
                    {
                        VideoTitle = row[1];
                    }
                    else if (row.Index == 1)
                    {
                        Instruction = row[1];
                    }
                    else if (row.Index > 2)
                    {
                        Questions.Add((Question)row);
                    }
                }
            }
        }

        public static VideoLesson FromCSV(Uri uri)
        {
            return new VideoLesson(uri);
        }

        public static VideoLesson FromCSV(string path)
        {
            var fileInfo = new FileInfo(path);

            return FromCSV(new Uri(fileInfo.FullName));
        }

        public virtual void ToPDF(string destinationPath)
        {
            var fileInfo = new FileInfo(destinationPath);

            ToPDF(new Uri(fileInfo.FullName));
        }

        public virtual void ToPDF(Uri destinationUri)
        {
            ValidateDestinationUri(destinationUri);

            // To do:
        }

        public virtual void ToFlatFile(string path)
        {
            using(var writer = new StreamWriter(path))
            {
                writer.WriteLine(VideoTitle);
                writer.WriteLine(Instruction);

                if (Questions != null && Questions.Count > 0)
                {
                    writer.WriteLine();

                    foreach(var question in Questions)
                    {
                        writer.WriteLine(question.Marks);
                        writer.WriteLine(question.Concept);
                        writer.WriteLine(question.QuestionText);
                        writer.WriteLine(question.NumberOfOptions);
                        writer.WriteLine(question.AnswerKey);

                        foreach (var option in question.Options)
                            if (!(option.IsNullOrEmpty() || option.IsNullOrWhitespace()))
                                writer.WriteLine(option);

                        writer.WriteLine(question.TimestampString);

                        writer.WriteLine();
                    }
                }
            }
        }

        private static void ValidateSourceUri(Uri uri)
        {
            uri.ThrowIfNull("Null argument: uri");

            uri.IsFile.ThrowIfFalse("Currently, only local files can be converted to and from video lessons.");

            File.Exists(uri.LocalPath).ThrowIfFalse("The file {0} does not exist.", uri.LocalPath);
        }

        private static void ValidateDestinationUri(Uri uri)
        {
            uri.ThrowIfNull("Null argument: uri");

            uri.IsFile.ThrowIfFalse("Currently, only local files can be converted to and from video lessons.");
        }
    }
}