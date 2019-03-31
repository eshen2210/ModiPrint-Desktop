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
        //The number of lines processed so far.
        private int _processedLines;
        public int ProcessedLines
        {
            get { return _processedLines; }
        }

        //The total lines that need to be processed.
        private int _totalLines;
        public int TotalLines
        {
            get { return _totalLines; }
        }

        public LineConvertedEventArgs(int ProcessedLines, int TotalLines)
        {
            _processedLines = ProcessedLines;
            _totalLines = TotalLines;
        }
    }
}
