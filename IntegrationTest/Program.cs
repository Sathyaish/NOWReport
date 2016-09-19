using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Wintellect.Extensions;

namespace Wintellect.NOW.Report.IntegrationTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Run(args);
        }

        private static void Run(string[] args)
        {
            try
            {
                if (args.ThereAreNone() || args[0].Trim().Equals("/?", StringComparison.InvariantCulture))
                {
                    PrintUsage();

                    goto AwaitTerminationKeyChar;
                }

                var groupConstraintSpecifiedPredicate = (Func<string, bool> )(arg => arg.StartsWith("/g:", StringComparison.InvariantCultureIgnoreCase));

                var sourceFile = new FileInfo(args[0]).FullName;
                
                var groupByConstraintArgs = args.Where(groupConstraintSpecifiedPredicate);
                var groupByArg = groupByConstraintArgs.FirstOrDefault(groupConstraintSpecifiedPredicate);
                string groupByValue = null;
                if (groupByArg != null) groupByValue = groupByArg.Substring(groupByArg.IndexOf("/g:") + "/g:".Length).Trim();

                var argsOtherThanSourceAndGroupConstraint = args.Except(groupByConstraintArgs).Skip(1);

                string destinationFilePath = null;
                FileInfo destinationFileInfo = null;

                if (argsOtherThanSourceAndGroupConstraint.ThereAreSome())
                {
                    destinationFileInfo = new FileInfo(argsOtherThanSourceAndGroupConstraint.First());

                    EnsureDirectory(destinationFileInfo.Directory);

                    destinationFilePath = destinationFileInfo.FullName;
                }
                else
                {
                    if (groupByValue.IsNullOrEmpty() || groupByValue.IsNullOrWhitespace())
                    {
                        DisplayAllActivitiesOnConsole(sourceFile);
                    }
                    else
                    {
                        if (groupByValue.Equals("video", StringComparison.InvariantCultureIgnoreCase))
                        {
                            DisplayActivityGroupedByVideoOnConsole(sourceFile);
                        }
                        else if (groupByValue.Equals("email", StringComparison.InvariantCultureIgnoreCase))
                        {
                            DisplayActivityGroupedByEmailOnConsole(sourceFile);
                        }
                        else
                        {
                            PrintUsage();
                        }
                    }

                    goto AwaitTerminationKeyChar;
                }

                var reportFormat = DetermineReportFormat(destinationFileInfo);

                if (groupByValue.IsNullOrEmpty() || groupByValue.IsNullOrWhitespace())
                {
                    if (reportFormat == ReportFormat.PDF)
                    {
                        DisplayAllActivitiesPDFReport(sourceFile, destinationFilePath);
                    }
                    else
                    {
                        DisplayAllActivitiesTextReport(sourceFile, destinationFilePath);
                    }
                }
                else
                {
                    if (groupByValue.Equals("video", StringComparison.InvariantCultureIgnoreCase))
                    {
                        if (reportFormat == ReportFormat.PDF) DisplayActivityGroupedByVideoPDFReport(sourceFile, destinationFilePath);
                        else DisplayActivityGroupedByVideoTextReport(sourceFile, destinationFilePath);
                    }
                    else if (groupByValue.Equals("email", StringComparison.InvariantCultureIgnoreCase))
                    {
                        if (reportFormat == ReportFormat.PDF) DisplayActivityGroupedByEmailPDFReport(sourceFile, destinationFilePath);
                        else DisplayActivityGroupedByEmailTextReport(sourceFile, destinationFilePath);
                    }
                    else
                    {
                        PrintUsage();

                        goto AwaitTerminationKeyChar;
                    }
                }

                Console.WriteLine("Report saved: {0}", destinationFilePath);
            }
            catch (Exception exception)
            {
                Console.WriteLine("An error occurred:\n{0}", exception.ToString());
            }

        AwaitTerminationKeyChar:
#if DEBUG
            Console.Write("\nPress any key to exit the program...");
            Console.ReadKey();
#endif
        }

        private static ReportFormat DetermineReportFormat(FileInfo fileInfo)
        {
            return fileInfo.Extension == ".pdf" ? ReportFormat.PDF : ReportFormat.Text;
        }

        private static void EnsureDirectory(DirectoryInfo directoryInfo)
        {
            Directory.CreateDirectory(directoryInfo.FullName);
        }

        private static void PrintUsage()
        {
            var builder = new StringBuilder();

            //This program reads WintellectNOW video activity data from CSV files and writes it to PDF or text files or displays it on the screen."

            //Usage:
            //Report.exe "Path\To\Source\File.csv" ["Destination\File\Path.pdf|txt"] [/g:video|email]

            //Notes:
            //1) All arguments except the path to the source CSV file are optional
            //2) If only the path to the source CSV file is specified, all the records in the CSV file are printed as is to the console window
            //3) To print the data to a file, specify a destination file path as an additional argument.
            //4) If the destination file does not exist, it is created.
            //5) All the folders in the destination path are created if they do not exist.
            //6) If a file already exists with the same name, the existing file is overwritten by the newly created destination file.
            //7) You may even specify partial or relative paths for either the source or the destination file.
            //8) If you specify a destination file with the extension ".pdf", a PDF file is generated, otherwise a text file is generated.
            //9) To see data grouped by either video or by email, specify an additional argument as "/g:video" for viewing data grouped by video, and "/g:email" for viewing data grouped by email.
            //10) You may specify only one group constraint at a time.
            //11) If you specify an invalid group constraint, the data is not grouped. Instead, all the data from the CSV is displayed in the chosen file format.
            //12) The order of the group constraint argument and the destination file does not matter. Only the first argument must be the path of the CSV source file. You can switch the order of the rest of the arguments.

            builder.AppendLine();

            builder.AppendLine("This program reads WintellectNOW video activity data from CSV files and writes it to PDF or text files or displays it on the screen.");
            builder.AppendLine();

            builder.AppendLine("Usage:");
            builder.AppendLine("Report.exe \"Path\\To\\Source\\File.csv\" [\"Destination\\File\\Path.pdf|txt\"] [/g:video|email]");
            builder.AppendLine();

            builder.AppendLine("Notes:");
            builder.AppendLine("1) All arguments except the path to the source CSV file are optional");
            builder.AppendLine();

            builder.AppendLine("2) If only the path to the source CSV file is specified, all the records in the CSV file are printed as is to the console window");
            builder.AppendLine();

            builder.AppendLine("3) To print the data to a file, specify a destination file path as an additional argument.");
            builder.AppendLine();

            builder.AppendLine("4) If the destination file does not exist, it is created.");
            builder.AppendLine();

            builder.AppendLine("5) All the folders in the destination path are created if they do not exist.");
            builder.AppendLine();

            builder.AppendLine("6) If a file already exists with the same name, the existing file is overwritten by the newly created destination file.");
            builder.AppendLine();

            builder.AppendLine("7) You may even specify partial or relative paths for either the source or the destination file.");
            builder.AppendLine();

            builder.AppendLine("8) If you specify a destination file with the extension \".pdf\", a PDF file is generated, otherwise a text file is generated.");
            builder.AppendLine();

            builder.AppendLine("9) To see data grouped by either video or by email, specify an additional argument as \"/g:video\" for viewing data grouped by video, and \"/g:email\" for viewing data grouped by email.");
            builder.AppendLine();

            builder.AppendLine("10) You may specify only one group constraint at a time.");
            builder.AppendLine();

            builder.AppendLine("11) If you specify an invalid group constraint, the data is not grouped. Instead, all the data from the CSV is displayed in the chosen file format.");
            builder.AppendLine();

            builder.AppendLine("12) The order of the group constraint argument and the destination file does not matter. Only the first argument must be the path of the CSV source file. You can switch the order of the rest of the arguments.");
            builder.AppendLine();

            Console.WriteLine(builder.ToString());
        }


        private static void DisplayActivityGroupedByVideoPDFReport(string sourceFile, string destinationFilePath)
        {
            var model = GetActivityGroupedByVideoModel(sourceFile);

            using (var report = new VideoActivityGroupedByVideoPDFReport("Video Lesson Activity", model, "Grouped by Video", destinationFilePath))
            {
            }

            OpenFile(destinationFilePath);
        }

        private static void DisplayActivityGroupedByVideoTextReport(string sourceFile, string destinationFilePath)
        {
            var model = GetActivityGroupedByVideoModel(sourceFile);

            using (var report = new GroupedTextReport<string, EmailAndCompletionPercentage>("VIDEO ACTIVITY REPORT", model, "Grouped by Video", destinationFilePath))
            {
            }

            OpenFile(destinationFilePath);
        }

        private static IEnumerable<IGrouping<string, EmailAndCompletionPercentage>> GetActivityGroupedByVideoModel(string sourceFile)
        {
            var allActivities = from row in new CSVFile(sourceFile).Rows
                                where row.Index > 0
                                select (VideoActivity)row;

            var model = new List<VideoActivityGroupedByVideo>();

            var videoGroups = allActivities.OrderBy(a => a.Video)
                .GroupBy(a => a.Video.ToTitleCase('-', ' '),
                a => new EmailAndCompletionPercentage
                {
                    Email = a.Email,
                    CompletionPercentage = a.CompletionPercentage
                });

            foreach (var videoGroup in videoGroups)
            {
                var activitiesGroupedByVideo = new VideoActivityGroupedByVideo(videoGroup.Key);

                var emailsAndCompletionPercentages = videoGroup
                    .GroupBy(g => g.Email, g => new { g.CompletionPercentage })
                    .Select(a => new EmailAndCompletionPercentage { Email = a.Key, CompletionPercentage = a.Max(b => b.CompletionPercentage) });

                foreach (var emailAndCompletionPercentage in emailsAndCompletionPercentages)
                {
                    activitiesGroupedByVideo.EmailAndCompletionPercentages.Add(emailAndCompletionPercentage);
                }

                model.Add(activitiesGroupedByVideo);
            }

            return model;
        }

        private static void DisplayActivityGroupedByVideoOnConsole(string sourceFile)
        {
            Console.WriteLine("VIDEO ACTIVITY REPORT");
            Console.WriteLine("Grouped by Video");
            Console.WriteLine();

            var model = GetActivityGroupedByVideoModel(sourceFile);

            foreach (var videoGroup in model)
            {
                Console.WriteLine(videoGroup.Key);

                foreach (var emailAndCompletionPercentage in videoGroup)
                {
                    Console.WriteLine("\t{0}", emailAndCompletionPercentage);
                }

                Console.WriteLine();
            }
        }


        private static void DisplayActivityGroupedByEmailPDFReport(string sourceFile, string destinationFilePath)
        {
            var model = GetActivityGroupedByEmailModel(sourceFile);

            using (var report = new VideoActivityGroupedByEmailPDFReport("Video Lesson Activity", model, "Grouped by Email", destinationFilePath))
            {
            }

            OpenFile(destinationFilePath);
        }

        private static void DisplayActivityGroupedByEmailTextReport(string sourceFile, string destinationFilePath)
        {
            var model = GetActivityGroupedByEmailModel(sourceFile);

            using (var report = new GroupedTextReport<string, VideoAndCompletionPercentage>("VIDEO ACTIVITY REPORT", model, "Grouped by Email", destinationFilePath))
            {
            }

            OpenFile(destinationFilePath);
        }

        private static IEnumerable<IGrouping<string, VideoAndCompletionPercentage>> GetActivityGroupedByEmailModel(string sourceFile)
        {
            var allActivities = from row in new CSVFile(sourceFile).Rows
                                where row.Index > 0
                                select (VideoActivity)row;

            var model = new List<VideoActivityGroupedByEmail>();

            var emailGroups = allActivities.OrderBy(a => a.Email)
                .GroupBy(a => a.Email, a => new VideoAndCompletionPercentage { Video = a.Video, CompletionPercentage = a.CompletionPercentage });

            foreach (var emailGroup in emailGroups)
            {
                var videoActivityGroupedByEmail = new VideoActivityGroupedByEmail(emailGroup.Key, null);

                var videosAndCompletionPercentages = emailGroup
                    .GroupBy(g => g.Video, g => new { g.CompletionPercentage })
                    .Select(a => new VideoAndCompletionPercentage
                    {
                        Video = a.Key.ToTitleCase('-', ' '),
                        CompletionPercentage = a.Max(b => b.CompletionPercentage)
                    });

                foreach (var videoAndCompletionPercentage in videosAndCompletionPercentages)
                {
                    videoActivityGroupedByEmail.VideoAndCompletionPercentages.Add(videoAndCompletionPercentage);
                }

                model.Add(videoActivityGroupedByEmail);
            }

            return model;
        }

        private static void DisplayActivityGroupedByEmailOnConsole(string sourceFile)
        {
            Console.WriteLine("VIDEO ACTIVITY REPORT");
            Console.WriteLine("Grouped by Email");
            Console.WriteLine();

            var model = GetActivityGroupedByEmailModel(sourceFile);

            foreach (var emailGroup in model)
            {
                Console.WriteLine(emailGroup.Key);

                foreach (var videoAndCompletionPercentage in emailGroup)
                {
                    Console.WriteLine("\t{0}", videoAndCompletionPercentage);
                }
                
                Console.WriteLine();
            }
        }


        private static void DisplayAllActivitiesPDFReport(string sourceFile, string destinationFilePath)
        {
            var activities = from row in new CSVFile(sourceFile).Rows
                             where row.Index > 0
                             select (VideoActivity)row;

            using (var report = new FlatPDFReport<VideoActivity>("Video Activity Report", activities, null, destinationFilePath))
            {
            }

            OpenFile(destinationFilePath);
        }

        private static void DisplayAllActivitiesTextReport(string sourceFile, string destinationFilePath)
        {
            var activities = from row in new CSVFile(sourceFile).Rows
                             where row.Index > 0
                             select (VideoActivity)row;

            using (var report = new FlatTextReport<VideoActivity>("Video Activity Report", activities, null, destinationFilePath))
            {
            }

            OpenFile(destinationFilePath);
        }

        private static void DisplayAllActivitiesOnConsole(string sourceFile)
        {
            var activities = from row in new CSVFile(sourceFile).Rows
                             where row.Index > 0
                             select (VideoActivity)row;

            int i = 0;

            Console.WriteLine("VIDEO ACTIVITY REPORT");
            Console.WriteLine();

            foreach (var activity in activities)
            {
                ++i;
                Console.WriteLine("{0}, {1}, {2}", activity.Email, activity.Video, activity.CompletionPercentage);
            }

            Console.WriteLine("\n{0} activities read.", i);
        }

        private static void OpenFile(string destinationFilePath)
        {
            var process = Process.Start(new ProcessStartInfo(destinationFilePath));

            process.Close();
        }

        private static void RunOld(string[] args)
        {
            try
            {
                if (args.IsNull() || args.ThereAreNot(3))
                {
                    PrintUsageOld();

                    goto AwaitTerminationKeyChar;
                }

                var destinationFormat = args[0];
                var csvFile = new FileInfo(args[1]).FullName;
                var destinationFile = new FileInfo(args[2]).FullName;

                var lesson = VideoLesson.FromCSV(csvFile);

                if (destinationFormat == "pdf")
                    lesson.ToPDF(destinationFile);
                else
                    lesson.ToFlatFile(destinationFile);

                Console.WriteLine("Your file is saved at {0}", destinationFile);
            }
            catch (Exception exception)
            {
                Console.WriteLine("An error occurred:\n{0}", exception.Message);
            }
            
            AwaitTerminationKeyChar:
#if DEBUG
            Console.Write("\nPress any key to exit the program...");
            Console.ReadKey();
#endif
        }

        private static void PrintUsageOld()
        {
            var builder = new StringBuilder();

            // This program reads WintellectNOW video lessons from CSV files and writes the information in those files to PDF files or text files."
            // Usage:
            // Lesson.exe "pdf|txt" "CSV\FileToConvert\Path.csv" "Destination\File\Path.pdf|.txt";

            // Notes:
            // 1) You must supply all the 3 arguments
            // 2) If you supply any value other than "pdf" for the first argument, a text file will be generated.
            // 3) If you specify an existing file for the third argument, as the present implementation stands, the existing file will be over-written. This program will be improved to allow you to save both, the existing and the newly created file.

            builder.AppendLine("This program reads WintellectNOW video lessons from CSV files and writes the information in those files to PDF files or text files.");
            builder.AppendLine();
            builder.AppendLine("Usage:");
            builder.AppendLine("Lesson.exe \"pdf|txt\" \"CSV\\FileToConvert\\Path.csv\" \"Destination\\File\\Path.pdf|.txt\";");
            builder.AppendLine();
            builder.AppendLine("Notes:");
            builder.AppendLine("1) You must supply all the 3 arguments");
            builder.AppendLine("2) If you supply any value other than \"pdf\" for the first argument, a text file will be generated.");
            builder.AppendLine("3) If you specify an existing file for the third argument, as the present implementation stands, the existing file will be over-written. This program will be improved to allow you to save both, the existing and the newly created file.");

            Console.WriteLine(builder.ToString());
        }
    }
}