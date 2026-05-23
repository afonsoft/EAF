using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Eaf.Renamer.Lib
{
    public class RenameTemplate
    {
        private readonly string[] IgorneDir = new[] { "node_modules", "packages", "bin", "obj" };

        private readonly Dictionary<string, string> PathFileUiApi;

        private BackgroundWorker Worker;

        public event DoWorkEventHandler DoWork;

        public event RunWorkerCompletedEventHandler WorkerCompleted;

        public event ProgressChangedEventHandler ProgressChanged;

        /// <summary>
        /// Log Event
        /// </summary>
        public event EventHandler<string> Log;

        public string GetPathUI
        {
            get
            {
                return PathFileUiApi["UI"];
            }
        }

        public string GetPathAPI
        {
            get
            {
                return PathFileUiApi["API"];
            }
        }

        public RenameTemplate()
        {
            Worker ??= new BackgroundWorker();
            PathFileUiApi = new Dictionary<string, string>
            {
                { "UI", null },
                { "API", null }
            };
        }

        public RenameTemplate(BackgroundWorker backgroundWorker) : this()
        {
            Worker = backgroundWorker;
        }

        private void Logger(string e)
        {
            Log?.Invoke(this, e);
        }

        #region RunWorker

        public void RunWorker(Arguments e)
        {
            RunWorker(Worker, e);
        }

        public void RunWorker(BackgroundWorker sender, Arguments e)
        {
            Worker = sender;
            Worker.DoWork += Worker_DoWork;
            Worker.ProgressChanged += Worker_ProgressChanged;
            Worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
            Worker.WorkerReportsProgress = true;
            Worker.WorkerSupportsCancellation = true;
            Worker.RunWorkerAsync(e);
        }

        public void StopWorker()
        {
            if (!Worker.IsBusy)
                return;
            Worker.CancelAsync();
        }

        #endregion RunWorker

        #region BackgroundWorkerRename

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            WorkerCompleted?.Invoke(this, e);
        }

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            Logger(e.UserState?.ToString());
            ProgressChanged?.Invoke(this, e);
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = (BackgroundWorker)sender;
            Arguments arguments = e.Argument as Arguments;
            string rootDir = arguments?.RootDir;
            Stopwatch stopwatch = new();
            stopwatch.Start();
            RenameAllDir(worker, e, arguments);
            stopwatch.Stop();
            long elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
            Logger(string.Format("================= Directory renaming completed =================\r\ntime spent: {0}(s)\r\n", elapsedMilliseconds));
            stopwatch.Reset();
            stopwatch.Start();
            arguments.RootDir = rootDir;
            RenameAllFileNameAndContent(worker, e, arguments);
            stopwatch.Stop();
            Logger(string.Format("================= File name and content renaming completed =================\r\ntime spent: {0}(s)\r\n", stopwatch.ElapsedMilliseconds));
            Logger(string.Format("================= Completed =================\r\nTime-spent catalog:{0}s\r\nFile time spent: {1}s\r\n", elapsedMilliseconds, stopwatch.ElapsedMilliseconds));

            DoWork?.Invoke(this, e);
        }

        #endregion BackgroundWorkerRename

        #region RenameAll

        private void RenameAllDir(BackgroundWorker worker, DoWorkEventArgs e, Arguments arguments)
        {
            string[] directories = Directory.GetDirectories(arguments.RootDir);
            int totalProgress = 0;
            int totalDirectories = directories.Length <= 0 ? 1 : directories.Length;

            foreach (string path in directories)
            {
                totalProgress++;
                if (worker.CancellationPending)
                {
                    e.Cancel = true;
                    break;
                }

                int percentProgress = (totalProgress * 100) / totalDirectories;

                if (IsIgonreDir(path))
                    continue;

                arguments.RootDir = path;
                RenameAllDir(worker, e, arguments);
                DirectoryInfo directoryInfo = new(path);
                if (directoryInfo.Name.Contains(arguments.OldCompanyName) || directoryInfo.Name.Contains(arguments.OldProjectName) || arguments.ChangeAreaName && directoryInfo.Name.Contains(arguments.OldAreaName))
                {
                    string str = directoryInfo.Name;
                    if (!string.IsNullOrEmpty(arguments.OldCompanyName))
                        str = str.Replace(arguments.OldCompanyName, arguments.NewCompanyName);
                    string path2 = str.Replace(arguments.OldProjectName, arguments.NewProjectName);
                    if (arguments.ChangeAreaName)
                        path2 = path2.Replace(arguments.OldAreaName, arguments.NewAreaName);
                    string destDirName = Path.Combine(directoryInfo.Parent.FullName, path2);
                    if (directoryInfo.FullName != destDirName)
                    {
                        worker.ReportProgress(percentProgress, (directoryInfo.FullName + "\r\n => \r\n" + destDirName + "\r\n\r\n"));
                        directoryInfo.MoveTo(destDirName);
                    }
                }
            }
        }

        private void RenameAllFileNameAndContent(
          BackgroundWorker worker,
          DoWorkEventArgs e,
          Arguments arguments)
        {
            List<FileInfo> list = ((IEnumerable<FileInfo>)new DirectoryInfo(arguments.RootDir).GetFiles()).Where<FileInfo>((Func<FileInfo, bool>)(m => arguments.Filter.Contains(m.Extension))).ToList<FileInfo>();
            int percentProgress = 0;

            int totalProgress = 0;
            int totalfiles = list.Count <= 0 ? 1 : list.Count;

            foreach (FileInfo fileInfo in list)
            {
                totalProgress++;
                if (worker.CancellationPending)
                {
                    e.Cancel = true;
                    break;
                }

                percentProgress = (totalProgress * 100) / totalfiles;

                if (IsIgonreDir(fileInfo.DirectoryName))
                    continue;

                if (fileInfo.FullName.Contains("package.json"))
                    PathFileUiApi["UI"] = fileInfo.DirectoryName;
                if (fileInfo.FullName.Contains(".sln"))
                    PathFileUiApi["API"] = fileInfo.FullName;

                string str1 = File.ReadAllText(fileInfo.FullName, Encoding.UTF8);
                if (!string.IsNullOrEmpty(arguments.OldCompanyName))
                    str1 = str1.Replace(arguments.OldCompanyName, arguments.NewCompanyName);
                string contents = str1.Replace(arguments.OldProjectName, arguments.NewProjectName);
                if (arguments.ChangeAreaName)
                    contents = contents.Replace(arguments.OldAreaName, arguments.NewAreaName);
                if (fileInfo.Name.Contains(arguments.OldCompanyName) || fileInfo.Name.Contains(arguments.OldProjectName) || arguments.ChangeAreaName && fileInfo.Name.Contains(arguments.OldAreaName))
                {
                    string str2 = fileInfo.Name;
                    if (!string.IsNullOrEmpty(arguments.OldCompanyName))
                        str2 = str2.Replace(arguments.OldCompanyName, arguments.NewCompanyName);
                    string path2 = str2.Replace(arguments.OldProjectName, arguments.NewProjectName);
                    if (arguments.ChangeAreaName)
                        path2 = path2.Replace(arguments.OldAreaName, arguments.NewAreaName);
                    string path = Path.Combine(fileInfo.DirectoryName, path2);
                    if (path != fileInfo.FullName)
                    {
                        worker.ReportProgress(percentProgress, ("\r\n" + fileInfo.FullName + "\r\n=>\r\n" + path + "\r\n\r\n"));
                        File.Delete(fileInfo.FullName);
                    }
                    File.WriteAllText(path, contents, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
                }
                else
                    File.WriteAllText(fileInfo.FullName, contents, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
                worker.ReportProgress(percentProgress, (fileInfo.Name + " => Complete\r\n"));
            }
            foreach (string directory in Directory.GetDirectories(arguments.RootDir))
            {
                if (worker.CancellationPending)
                {
                    e.Cancel = true;
                    break;
                }
                arguments.RootDir = directory;
                RenameAllFileNameAndContent(worker, e, arguments);
            }
        }

        #endregion RenameAll

        private bool IsIgonreDir(string path)
        {
            return IgorneDir.Any(x => path.EndsWith(x) || path.Contains(x));
        }
    }
}