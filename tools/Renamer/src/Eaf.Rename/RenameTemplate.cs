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
            if (arguments == null)
                return;

            string rootDir = arguments.RootDir;
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
            int totalDirectories = directories.Length <= 0 ? 1 : directories.Length;

            int totalProgress = 0;
            foreach (string path in directories)
            {
                totalProgress++;
                if (worker.CancellationPending)
                {
                    e.Cancel = true;
                    break;
                }

                if (IsIgonreDir(path))
                    continue;

                arguments.RootDir = path;
                RenameAllDir(worker, e, arguments);

                int percentProgress = (totalProgress * 100) / totalDirectories;
                RenameDirectoryIfNeeded(worker, arguments, path, percentProgress);
            }
        }

        private static void RenameDirectoryIfNeeded(BackgroundWorker worker, Arguments arguments, string path, int percentProgress)
        {
            DirectoryInfo directoryInfo = new(path);
            if (!ShouldRename(directoryInfo.Name, arguments))
                return;

            string newName = ReplaceTokens(directoryInfo.Name, arguments);
            string destDirName = Path.Combine(directoryInfo.Parent.FullName, newName);
            if (directoryInfo.FullName != destDirName)
            {
                worker.ReportProgress(percentProgress, (directoryInfo.FullName + "\r\n => \r\n" + destDirName + "\r\n\r\n"));
                directoryInfo.MoveTo(destDirName);
            }
        }

        private static bool ShouldRename(string name, Arguments arguments)
        {
            return name.Contains(arguments.OldCompanyName)
                || name.Contains(arguments.OldProjectName)
                || (arguments.ChangeAreaName && name.Contains(arguments.OldAreaName));
        }

        private static string ReplaceTokens(string value, Arguments arguments)
        {
            if (!string.IsNullOrEmpty(arguments.OldCompanyName))
                value = value.Replace(arguments.OldCompanyName, arguments.NewCompanyName);
            value = value.Replace(arguments.OldProjectName, arguments.NewProjectName);
            if (arguments.ChangeAreaName)
                value = value.Replace(arguments.OldAreaName, arguments.NewAreaName);
            return value;
        }

        private void RenameAllFileNameAndContent(
            BackgroundWorker worker,
            DoWorkEventArgs e,
            Arguments arguments)
        {
            List<FileInfo> files = GetFilteredFiles(arguments.RootDir, arguments.Filter);
            int totalFiles = files.Count <= 0 ? 1 : files.Count;

            int totalProgress = 0;
            foreach (FileInfo fileInfo in files)
            {
                totalProgress++;
                if (worker.CancellationPending)
                {
                    e.Cancel = true;
                    break;
                }

                int percentProgress = (totalProgress * 100) / totalFiles;
                if (IsIgonreDir(fileInfo.DirectoryName))
                    continue;

                UpdatePathFileUiApi(fileInfo.FullName);
                RenameFileContentAndName(worker, arguments, fileInfo, percentProgress);
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

        private static List<FileInfo> GetFilteredFiles(string rootDir, string filter)
        {
            return new DirectoryInfo(rootDir).GetFiles()
                .Where(m => filter.Contains(m.Extension))
                .ToList();
        }

        private void UpdatePathFileUiApi(string fullName)
        {
            if (fullName.Contains("package.json"))
                PathFileUiApi["UI"] = Path.GetDirectoryName(fullName);
            if (fullName.Contains(".sln"))
                PathFileUiApi["API"] = fullName;
        }

        private static void RenameFileContentAndName(BackgroundWorker worker, Arguments arguments, FileInfo fileInfo, int percentProgress)
        {
            string contents = GetRenamedFileContents(fileInfo.FullName, arguments);
            if (ShouldRename(fileInfo.Name, arguments))
            {
                string newName = ReplaceTokens(fileInfo.Name, arguments);
                string path = Path.Combine(fileInfo.DirectoryName, newName);
                if (path != fileInfo.FullName)
                {
                    worker.ReportProgress(percentProgress, ("\r\n" + fileInfo.FullName + "\r\n=>\r\n" + path + "\r\n\r\n"));
                    File.Delete(fileInfo.FullName);
                }
                File.WriteAllText(path, contents, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
            }
            else
            {
                File.WriteAllText(fileInfo.FullName, contents, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
            }
            worker.ReportProgress(percentProgress, (fileInfo.Name + " => Complete\r\n"));
        }

        private static string GetRenamedFileContents(string filePath, Arguments arguments)
        {
            return ReplaceTokens(File.ReadAllText(filePath, Encoding.UTF8), arguments);
        }

        #endregion RenameAll

        private bool IsIgonreDir(string path)
        {
            return IgorneDir.Any(x => path.EndsWith(x) || path.Contains(x));
        }
    }
}