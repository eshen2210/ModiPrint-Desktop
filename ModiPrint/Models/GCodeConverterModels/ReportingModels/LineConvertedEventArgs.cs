using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModiPrint.Models.GCodeConverterModels.ReportingModels
{
    /// <summary>
    /// The GCodeConverter will pass this class as an argument when triggering events that update number of lines converted to the GUI.
    /// </summary>
    public class LineConvertedEventArgs : EventArgs
    {
        //Short description that will be displayed in the progress bar.
        private string _taskName;
        public string TaskName
        {
            get { return _taskName; }
        }

        //The percentage of the task that has been processed.
        private int _percentCompleted;
        public int PercentCompleted
        {
            get { return _percentCompleted; }
        }

        public LineConvertedEventArgs(string TaskName, int PercentCompleted)
        {
            _taskName = TaskName;
            _percentCompleted = PercentCompleted;
        }
    }
}
