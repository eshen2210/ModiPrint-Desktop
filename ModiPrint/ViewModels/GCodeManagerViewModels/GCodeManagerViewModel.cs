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
        private GCodeModel _repRapGCodeModel;
        public GCodeModel RepRapGCodeModel
        {
            get { return _repRapGCodeModel; }
        }
        private GCodeModel _modiPrintGCodeModel;
        public GCodeModel ModiPrintGCodeModel
        {
            get { return _modiPrintGCodeModel; }
        }
        private GCodeFileManagerModel _gCodeFileManagerModel;
        private GCodeConverterBGWModel _gCodeConverterBGWModel;

        //GCode of the RepRap flavor.
        public string RepRapGCode
        {
            get { return _repRapGCodeModel.GCodeStr; }
            set
            {
                _repRapGCodeModel.GCodeStr = value;
                OnPropertyChanged("RepRapGCode");
            }
        }

        //GCode converted from RepRap GCode.
        public string ModiPrintGCode
        {
            get { return _modiPrintGCodeModel.GCodeStr; }
            set
            {
                _modiPrintGCodeModel.GCodeStr = value;
                OnModiPrintGCodeChanged();
                OnPropertyChanged("ModiPrintGCode");
            }
        }

        //GCode file name of the most recently uploaded or uploaded GCode file.
        private string _gCodeFileName = "";
        public string GCodeFileName
        {
            get { return _gCodeFileName; }
            set
            {
                _gCodeFileName = value;
                OnPropertyChanged("GCodeFileName");
            }
        }

        //These properties indicate the progress made during GCode conversion.
        
        //Name of the task being done.
        private string _taskName;
        public string TaskName
        {
            get { return _taskName; }
        }

        //Percentage of the task completed.
        private int _percentCompleted;
        public int PercentCompleted
        {
            get { return _percentCompleted; }
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
        public GCodeManagerViewModel(GCodeModel RepRapGCodeModel, GCodeModel ModiPrintGCodeModel, GCodeFileManagerModel GCodeFileManagerModel, GCodeConverterBGWModel GCodeConverterBGWModel)
        {
            _repRapGCodeModel = RepRapGCodeModel;
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
            _taskName = lineConvertedEventArgs.TaskName;
            _percentCompleted = lineConvertedEventArgs.PercentCompleted;
            OnPropertyChanged("TaskName");
            OnPropertyChanged("PercentCompleted");
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
        #endregion

        #region Commands
        /// <summary>
        /// Reads and saves a .gcode file into RepRapGCode.
        /// </summary>
        private RelayCommand<object> _uploadGCodeFileCommand;
        public ICommand UploadGCodeFileCommand
        {
            get
            {
                if (_uploadGCodeFileCommand == null)
                { _uploadGCodeFileCommand = new RelayCommand<object>(ExecuteUploadGCodeFileCommand, CanExecuteUploadGCodeFileCommand); }
                return _uploadGCodeFileCommand;
            }
        }

        public bool CanExecuteUploadGCodeFileCommand(object notUsed)
        {
            return true;
        }

        public void ExecuteUploadGCodeFileCommand(object notUsed)
        {
            _gCodeFileName = _gCodeFileManagerModel.UploadGCodeFile();
            OnPropertyChanged("GCodeFileName");
            OnPropertyChanged("RepRapGCode");
            OnPropertyChanged("ModiPrintGCode");
            _convertGCodeCommand.RaiseCanExecuteChanged();
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
            _gCodeFileManagerModel.SaveGCodeFile(_modiPrintGCodeModel, ".mdpt", "ModiPrint GCode Files (.mdpt)|*.mdpt", ref _gCodeFileName);
            OnPropertyChanged("ModiPrintGCodeFileName");
            OnPropertyChanged("ModiPrintGCode");
        }

        /// <summary>
        /// Converts RepRap GCode into ModiPrint GCode based on user input parameters.
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
            return (!String.IsNullOrWhiteSpace(_gCodeFileName))
                && (!String.IsNullOrWhiteSpace(_repRapGCodeModel.GCodeStr)
                && (_gCodeConverterBGWModel.IsBusy == false)) ? true : false;
        }

        public void ExecuteConvertGCodeCommand(object notUsed)
        {
            _gCodeConverterBGWModel.StartConversion(_repRapGCodeModel.GCodeStr);
            while (_gCodeConverterBGWModel.GCodeConverterMainModel == null) { } //Wait for the class to instantiate before trying to subscribe to its event.
            _gCodeConverterBGWModel.GCodeConverterMainModel.ParametersModel.LineConverted += new GCodeConverterLineConvertedEventHandler(GCodeConverterBGWReportProgressLineCount);
        }
        #endregion
    }
}

