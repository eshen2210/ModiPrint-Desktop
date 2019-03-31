using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using System.ComponentModel;
using ModiPrint.ViewModels;
using ModiPrint.Models.PrinterModels;
using ModiPrint.Models.PrintModels;
using ModiPrint.Models.RealTimeStatusModels;
using ModiPrint.Models.GCodeConverterModels.ReportingModels;

namespace ModiPrint.Models.GCodeConverterModels
{
    //Event handler for the termination of the GCodeConverter background thread.
    public delegate void GCodeConverterBGWTerminatedEventHandler(object sender, string convertedGCode);

    //Event handler for updating conversion progress of the GCodeConverter.
    public delegate void GCodeConverterLineConvertedEventHandler(object sender, LineConvertedEventArgs lineConvertedEventArgs);

    /// <summary>
    /// Manages the Backgroundworker for the GCodeConverter.
    /// </summary>
    public class GCodeConverterBGWModel
    {
        #region Fields and Properties
        //All GCode conversion is run on a seprate thread.
        private BackgroundWorker _bGWGCodeConverter;

        //Classes required by the GCodeConverter;
        private PrinterModel _printerModel;
        private PrintModel _printModel;
        private RealTimeStatusDataModel _realTimeStatusDataModel;

        //Displays errors to the GUI.
        ErrorListViewModel _errorListViewModel;

        //Converts GCode.
        private GCodeConverterMainModel _gCodeConverterMainModel;
        public GCodeConverterMainModel GCodeConverterMainModel
        {
            get { return _gCodeConverterMainModel; }
        }

        //Event for the termination of the GCodeConverter background thread.
        public event GCodeConverterBGWTerminatedEventHandler GCodeConverterBGWTerminated;
        private void OnGCodeConverterBGWTerminated(string convertedGCode)
        {
            if (GCodeConverterBGWTerminated != null)
            { GCodeConverterBGWTerminated(this, convertedGCode); }
        }

        //The converted GCode at the end of the conversion.
        private string _convertedGCode;

        //Tells the GCodeConverter whether or not to add Slic3r line numbers to the converted GCode.
        private bool _commentLineNumber = true; //To Do: If this is set to true, then stuff breaks.
        public bool CommentLineNumber
        {
            get { return _commentLineNumber; }
            set { _commentLineNumber = value; }
        }
        #endregion

        #region Constructor
        public GCodeConverterBGWModel(PrinterModel PrinterModel, PrintModel PrintModel, RealTimeStatusDataModel RealTimeStatusDataModel, ErrorListViewModel ErrorListViewModel)
        {
            _printerModel = PrinterModel;
            _printModel = PrintModel;
            _realTimeStatusDataModel = RealTimeStatusDataModel;
            _errorListViewModel = ErrorListViewModel;

            _bGWGCodeConverter = new BackgroundWorker()
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true
            };
            _bGWGCodeConverter.DoWork += new DoWorkEventHandler(BGWGCodeConverter_DoWork);
            _bGWGCodeConverter.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BGWGCodeConverter_RunWorkerCompleted);
        }
        #endregion

        #region Methods
        public void StartConversion(string gCode)
        {
            if (_bGWGCodeConverter.IsBusy == false)
            {
                _bGWGCodeConverter.RunWorkerAsync(gCode);
            }
        }

        public void EndConversion()
        {
            _bGWGCodeConverter.CancelAsync();
        }

        public void BGWGCodeConverter_DoWork(object sender, DoWorkEventArgs e)
        {
            string gCode = (string)e.Argument;
            _gCodeConverterMainModel = new GCodeConverterMainModel(_printerModel, _printModel, _realTimeStatusDataModel, _errorListViewModel);
            _convertedGCode = _gCodeConverterMainModel.ConvertGCode(gCode);
        }

        public void BGWGCodeConverter_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            OnGCodeConverterBGWTerminated(_convertedGCode);
            _convertedGCode = null;
        }
        #endregion
    }
}
