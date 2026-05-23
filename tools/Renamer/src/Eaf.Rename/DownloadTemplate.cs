using System;
using System.ComponentModel;

namespace Eaf.Renamer.Lib
{
    internal class DownloadTemplate
    {
        private BackgroundWorker Worker;

        public event RunWorkerCompletedEventHandler WorkerCompleted;

        public event ProgressChangedEventHandler ProgressChanged;

        /// <summary>
        /// Log Event
        /// </summary>
        public event EventHandler<string> Log;

        public event EventHandler<Exception> LogError;

        public DownloadTemplate()
        {
            Worker ??= new BackgroundWorker();
        }

        public DownloadTemplate(BackgroundWorker backgroundWorker) : this()
        {
            Worker = backgroundWorker;
        }

        private void Logger(string e)
        {
            Log?.Invoke(this, e);
        }

        private void Logger(Exception e)
        {
            LogError?.Invoke(this, e);
        }

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

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            WorkerCompleted?.Invoke(this, e);
        }

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            ProgressChanged?.Invoke(this, e);
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            //Processar o download e o rename
        }
    }
}