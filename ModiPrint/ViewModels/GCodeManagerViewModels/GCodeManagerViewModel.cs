using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ModiPrint.Models.GCodeModels;
using ModiPrint.Models.GCodeConverterModels;
using ModiPrint.Models.GCodeConverterModels.ReportingModels;
using ModiPrint.Models.PrintModels;

namespace ModiPrint.ViewModels.GCodeManagerViewModels
{
    //Handles event that is fired when converted GCode is changed.
    public delegate void ModiPrintGCodeChangedEventHandler(object sender);

    /// <summary>
    /// To Do: This should not handle application logic such as converting GCode
    /// </summary>
    public class GCodeManagerViewModel : ViewModel
    {
        #region Fields and Properties
        //GCode and utilities related to the conversion of GCode.
        private GCodeModel _slic3rGCodeModel;
        public GCodeModel Slic3rGCodeModel
        {
            get { return _slic3rGCodeModel; }
        }
        private GCodeModel _modiPrintGCodeModel;
        public GCodeModel ModiPrintGCodeModel
        {
            get { return _modiPrintGCodeModel; }
        }
        private GCodeFileManagerModel _gCodeFileManagerModel;
        private GCodeConverterBGWModel _gCodeConverterBGWModel;

        //GCode from Slic3r.
        //Adds the line count in front of each line.
        public string Slic3rGCode
        {
            get { return AddLineNumberToGCode(_slic3rGCodeModel.GCodeArr); }
            set
            {
                _slic3rGCodeModel.GCodeStr = value;
                OnPropertyChanged("Slic3rGCode");
            }
        }

        //GCode converted from Slic3r GCode.
        public string ModiPrintGCode
        {
            get { return _modiPrintGCodeModel.GCodeStr; }
            set
            {
                _modiPrintGCodeModel.SetGCode(value);
                OnModiPrintGCodeChanged();
                OnPropertyChanged("ModiPrintGCode");
            }
        }

        //Slic3r GCode's file name.
        private string _slic3rGCodeFileName = "";
        public string Slic3rGCodeFileName
        {
            get { return _slic3rGCodeFileName; }
            set
            {
                _slic3rGCodeFileName = value;
                OnPropertyChanged("Slic3rGCodeFileName");
            }
        }

        //ModiPrint GCode's file name.
        private string _modiPrintGCodeFileName = "";
        public string ModiPrintGCodeFileName
        {
            get { return _modiPrintGCodeFileName; }
            set
            {
                _modiPrintGCodeFileName = value;
                OnPropertyChanged("ModiPrintGCodeFileName");
            }
        }

        //These properties indicate the progress made during GCode conversion.
        private int _processedLines;
        public int ProcessedLines
        {
            get { return _processedLines; }
        }

        private int _totalLines;
        public int TotalLines
        {
            get { return _totalLines; }
        }

        //Event that is fired when converted GCode is changed.
        public event ModiPrintGCodeChangedEventHandler ModiPrintGCodeChanged;
        private void OnModiPrintGCodeChanged()
        {
            if (ModiPrintGCodeChanged != null)
            { ModiPrintGCodeChanged(this); }
        }
        #endregion

        #region Contructor
        public GCodeManagerViewModel(GCodeModel Slic3rGCodeModel, GCodeModel ModiPrintGCodeModel, GCodeFileManagerModel GCodeFileManagerModel, GCodeConverterBGWModel GCodeConverterBGWModel)
        {
            _slic3rGCodeModel = Slic3rGCodeModel;
            _modiPrintGCodeModel = ModiPrintGCodeModel;
            _gCodeFileManagerModel = GCodeFileManagerModel;
            _gCodeConverterBGWModel = GCodeConverterBGWModel;

            _gCodeConverterBGWModel.GCodeConverterBGWTerminated += new GCodeConverterBGWTerminatedEventHandler(GCodeConverterBGWTerminated);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Called when ProgressUpdatedLineCount event is triggered in the GCodeConverter.
        /// Updates the progress bar that is reporting on GCode conversion progress.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="lineConvertedEventArgs"></param>
        private void GCodeConverterBGWReportProgressLineCount(object sender, LineConvertedEventArgs lineConvertedEventArgs)
        {
            _processedLines = lineConvertedEventArgs.ProcessedLines;
            _totalLines = lineConvertedEventArgs.TotalLines;
            OnPropertyChanged("ProcessedLines");
            OnPropertyChanged("TotalLines");
        }

        /// <summary>
        /// Triggered on the event BGWConverterTerminated.
        /// Updates ModiPrintGCode upon the end of the GCode conversion.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="convertedGCode"></param>
        private void GCodeConverterBGWTerminated(object sender, string convertedGCode)
        {
            _modiPrintGCodeModel.GCodeStr = convertedGCode;
            OnPropertyChanged("ModiPrintGCode");
        }

        /// <summary>
        /// Adds GCode line count to the beginning of each GCode line.
        /// </summary>
        /// <param name="gCodeArr"></param>
        /// <returns></returns>
        private string AddLineNumberToGCode(string[] gCodeArr)
        {
            //Return value.
            string gCodeWithLineNumber = "";

            if (gCodeArr != null)
            {
                //Add GCode line count to the beginning of each GCode line.
                int gCodeLineCount = 1;
                foreach (string gCodeLine in gCodeArr)
                {
                    gCodeWithLineNumber += gCodeLineCount++ + "; ";
                    gCodeWithLineNumber += gCodeLine + "\r\n";
                }
            }

            return gCodeWithLineNumber;
        }
        #endregion

        #region Commands
        /// <summary>
        /// Reads and saves a .gcode file into Slic3rGCode.
        /// </summary>
        private RelayCommand<object> _uploadSlic3rGCodeFileCommand;
        public ICommand UploadSlic3rGCodeFileCommand
        {
            get
            {
                if (_uploadSlic3rGCodeFileCommand == null)
                { _uploadSlic3rGCodeFileCommand = new RelayCommand<object>(ExecuteUploadSlic3rGCodeFileCommand, CanExecuteUploadSlic3rGCodeFileCommand); }
                return _uploadSlic3rGCodeFileCommand;
            }
        }

        public bool CanExecuteUploadSlic3rGCodeFileCommand(object notUsed)
        {
            return true;
        }

        public void ExecuteUploadSlic3rGCodeFileCommand(object notUsed)
        {
            _gCodeFileManagerModel.UploadGCodeFile(_slic3rGCodeModel, "GCode Files (.gcode)|*.gcode|Text Files (.txt)|*.txt|All Files (*.*)|*.*", ref _slic3rGCodeFileName);
            OnPropertyChanged("Slic3rGCodeFileName");
            OnPropertyChanged("Slic3rGCode");
        }

        /// <summary>
        /// Reads and saves a .mdpt file into ModiPrintGCode.
        /// </summary>
        private RelayCommand<object> _uploadModiPrintGCodeFileCommand;
        public ICommand UploadModiPrintGCodeFileCommand
        {
            get
            {
                if (_uploadModiPrintGCodeFileCommand == null)
                { _uploadModiPrintGCodeFileCommand = new RelayCommand<object>(ExecuteUploadModiPrintGCodeFileCommand, CanExecuteUploadModiPrintGCodeFileCommand); }
                return _uploadModiPrintGCodeFileCommand;
            }
        }

        public bool CanExecuteUploadModiPrintGCodeFileCommand(object notUsed)
        {
            return true;
        }

        public void ExecuteUploadModiPrintGCodeFileCommand(object notUsed)
        {
            _gCodeFileManagerModel.UploadGCodeFile(_modiPrintGCodeModel, "ModiPrint GCode Files (.mdpt)|*.mdpt", ref _modiPrintGCodeFileName);
            OnPropertyChanged("ModiPrintGCodeFileName");
            OnPropertyChanged("ModiPrintGCode");
        }

        /// <summary>
        /// Reads and saves a .mdpt file from ModiPrintGCode.
        /// </summary>
        private RelayCommand<object> _saveModiPrintGCodeFileCommand;
        public ICommand SaveModiPrintGCodeFileCommand
        {
            get
            {
                if (_saveModiPrintGCodeFileCommand == null)
                { _saveModiPrintGCodeFileCommand = new RelayCommand<object>(ExecuteSaveModiPrintGCodeFileCommand, CanExecuteSaveModiPrintGCodeFileCommand); }
                return _saveModiPrintGCodeFileCommand;
            }
        }

        public bool CanExecuteSaveModiPrintGCodeFileCommand(object notUsed)
        {
            return true;
        }

        public void ExecuteSaveModiPrintGCodeFileCommand(object notUsed)
        {
            _gCodeFileManagerModel.SaveGCodeFile(_modiPrintGCodeModel, ".mdpt", "ModiPrint GCode Files (.mdpt)|*.mdpt", ref _modiPrintGCodeFileName);
            OnPropertyChanged("ModiPrintGCodeFileName");
            OnPropertyChanged("ModiPrintGCode");
        }

        /// <summary>
        /// Converts Slic3r GCode into ModiPrint GCode based on user input parameters.
        /// </summary>
        private RelayCommand<object> _convertGCodeCommand;
        public ICommand ConvertGCodeCommand
        {
            get
            {
                if (_convertGCodeCommand == null)
                { _convertGCodeCommand = new RelayCommand<object>(ExecuteConvertGCodeCommand, CanExecuteConvertGCodeCommand); }
                return _convertGCodeCommand;
            }
        }

        public bool CanExecuteConvertGCodeCommand(object notUsed)
        {
            return (!String.IsNullOrWhiteSpace(_slic3rGCodeFileName)
                && !String.IsNullOrWhiteSpace(_slic3rGCodeModel.GCodeStr)) ? true : false;
        }

        public void ExecuteConvertGCodeCommand(object notUsed)
        {
            _gCodeConverterBGWModel.StartConversion(_slic3rGCodeModel.GCodeStr);
            while (_gCodeConverterBGWModel.GCodeConverterMainModel == null) { } //Wait for the class to instantiate before trying to subscribe to its event.
            _gCodeConverterBGWModel.GCodeConverterMainModel.LineConverted += new GCodeConverterLineConvertedEventHandler(GCodeConverterBGWReportProgressLineCount);

        }
        #endregion
    }
}

