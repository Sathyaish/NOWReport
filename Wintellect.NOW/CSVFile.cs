using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Wintellect.Extensions;
using System.Linq;

namespace Wintellect.NOW
{
    public sealed class CSVFile : IDisposable
    {
        private const string Quote = "\"";
        private const string EscapedQuote = "\"\"";

        private static char[] CharactersThatMustBeQuoted = { ',', '"', '\n' };
        private static Regex CsvSplitterRegEx = new Regex(@",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))");
        private static Regex RunOnLineRegEx = new Regex(@"^[^""]*(?:""[^""]*""[^""]*)*""[^""]*$");

        private string _fileName = null;
        private long _rowIndex = 0;
        private TextReader _reader;
        
        public class Row : IEnumerable<string>
        {
            private string[] _values = null;
            private long _rowIndex = -1;
            private string _fileName = null;
            
            public Row(string fileName, string[] values, long rowIndex)
            {
                _fileName = fileName;

                _values = values;

                _rowIndex = rowIndex;
            }

            public IEnumerator<string> GetEnumerator()
            {
                return _values.AsEnumerable<string>().GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            public string File
            {
                get
                {
                    return _fileName;
                }
            }

            public long Index
            {
                get
                {
                    return _rowIndex;
                }
            }

            public string this[int columnIndex]
            {
                get
                {
                    return _values[columnIndex];
                }
            }
        }

        public CSVFile(string fileName)
        {
            _fileName = new FileInfo(fileName).FullName;

            _reader = new StreamReader(new FileStream(_fileName, FileMode.Open, FileAccess.Read));
        }

        public IEnumerable<CSVFile.Row> Rows
        {
            get
            {
                _rowIndex = -1; string line; string nextLine;

                while ((line = _reader.ReadLine()).IsNotNull())
                {
                    while (RunOnLineRegEx.IsMatch(line) && (nextLine = _reader.ReadLine()).IsNotNull())
                        line = line.Plus("\n", nextLine);

                    _rowIndex++;
                    string[] values = CsvSplitterRegEx.Split(line);

                    for (int i = 0; i < values.Length; i++)
                        values[i] = Unescape(values[i]);

                    yield return new CSVFile.Row(_fileName, values, _rowIndex);
                }

                _reader.Close();
            }
        }

        public void Dispose()
        {
            if (_reader.IsNotNull()) _reader.Dispose();
        }

        public static string Escape(string s)
        {
            if (s.Contains(Quote))
                s = s.Replace(Quote, EscapedQuote);

            if (s.IndexOfAny(CharactersThatMustBeQuoted) > -1)
                s = Quote.Plus(s).Plus(Quote);

            return s;
        }

        public static string Unescape(string s)
        {
            if (s.StartsWith(Quote) && s.EndsWith(Quote))
            {
                s = s.Substring(1, s.Length - 2);

                if (s.Contains(EscapedQuote))
                    s = s.Replace(EscapedQuote, Quote);
            }

            return s;
        }
    }
}