using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XafSmartEditors.Razor.NqlDotNet
{
    using ClosedXML.Excel;
    using DevExpress.Office.Utils;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Threading.Tasks;

    public class AsyncBackgroundWorker<T>
    {
        Dictionary<int,object> Results=new Dictionary<int, object>();
        private readonly BackgroundWorker _worker;
        private readonly List<Func<Task<T>>> _tasks;
        private readonly Action<int, string, T> _onProgressChanged;
        private readonly Action<Dictionary<int, object>> _onCompleted;

        public AsyncBackgroundWorker(
            IEnumerable<Func<Task<T>>> tasks,
            Action<int, string, T> onProgressChanged,
            Action<Dictionary<int, object>> onCompleted)
        {
            _tasks = new List<Func<Task<T>>>(tasks);
            _onProgressChanged = onProgressChanged;
            _onCompleted = onCompleted;

            _worker = new BackgroundWorker { WorkerReportsProgress = true };
            _worker.DoWork += Worker_DoWork;
            _worker.ProgressChanged += Worker_ProgressChanged;
            _worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
        }

        public void Start() => _worker.RunWorkerAsync();

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                for (int i = 0; i < _tasks.Count; i++)
                {
                    // Execute the async task synchronously and get the result
                    T result = _tasks[i]().GetAwaiter().GetResult();
                    Results.Add(i, result);
                    // Calculate progress percentage based on task count
                    int progress = (int)((i + 1) / (double)_tasks.Count * 100);
                    //_worker.ReportProgress(progress, $"Task {i + 1} complete", result);
                    _worker.ReportProgress(progress, result);
                }

                e.Result = Results;
            }
            catch (Exception ex)
            {
                e.Result = $"Error: {ex.Message}";
            }
        }

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            var result = (T)e.UserState;
            _onProgressChanged?.Invoke(e.ProgressPercentage, $"Progress: {e.ProgressPercentage}%", result);
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Dictionary<int, object>? obj = e.Result as Dictionary<int, object>;
            var test = Results;
            _onCompleted?.Invoke(obj);
        }
    }

}
