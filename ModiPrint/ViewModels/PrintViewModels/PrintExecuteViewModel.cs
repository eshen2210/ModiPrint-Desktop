using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using ModiPrint.Models.GCodeModels;
using ModiPrint.Models.SerialCommunicationModels;
using ModiPrint.Models.RealTimeStatusModels;
using ModiPrint.Models.PrinterModels.PrintheadModels.PrintheadTypeModels;
using ModiPrint.ViewModels.PrintViewModels;
using ModiPrint.ViewModels.GCodeManagerViewModels;
using ModiPrint.ViewModels.SerialCommunicationViewModels;

namespace ModiPrint.ViewModels.PrintViewModels
{
    /// <summary>
    /// Associated with PrintExecuteViewModel.
    /// Represents the current actions of the Printer.
    /// </summary>
    public enum PrinterStatus
    {
        Manual, //Idle or executing manual/calibration commands.
        Printing, //In the process of printing.
        Paused //In the process of printing but paused.
    }
    
    /// <summary>
    /// Contains functions to begin Printing and perform pre-Print checks.
    /// </summary>
    public class PrintExecuteViewModel : ViewModel
    {
        #region Fields and Properties
        //Contains information about the state of the Printer, Print, and Serial Communication settings.
        //This information will be used to determine if the Printer is ready to print.
        private GCodeModel _modiPrintGCodeModel;
        private GCodeManagerViewModel _gCodeManagerViewModel;
        private SerialCommunicationViewModel _serialCommunicationViewModel;
        private RealTimeStatusDataModel _realTimeStatusDataModel;

        //Contains function for outputting and receiving messages from the serial communicator.
        private SerialCommunicationOutgoingMessagesModel _serialCommunicationOutgoingMessagesModel;
        private SerialMessageDisplayViewModel _serialMessageDisplayViewModel;

        //Current state of operations of the Printer.
        private PrinterStatus _printerStatus = PrinterStatus.Manual;
        public PrinterStatus PrinterStatus
        {
            get { return _printerStatus; }
            set
            {
                _printerStatus = value;
                OnPropertyChanged("PrinterStatus");
                OnPropertyChanged("IsReadyToPrint");
            }
        }

        //Set to true if all serial communication should be halted.
        public bool ShouldPause
        {
            get { return _serialCommunicationViewModel.SerialCommunicationMainModel.ShouldPause; }
            set { _serialCommunicationViewModel.SerialCommunicationMainModel.ShouldPause = value; }
        }

        //Set to true to resume hardware operations after pausing.
        public bool ShouldResume
        {
            get { return _serialCommunicationViewModel.SerialCommunicationMainModel.ShouldResume; }
            set { _serialCommunicationViewModel.SerialCommunicationMainModel.ShouldResume = value; }
        }

        //Are various aspects of this program ready to execute a Print?
        public bool IsReadyToPrint
        {
            get
            {
                if ((IsModiPrintGCodeReadyToPrint == true)
                 && (IsSerialCommunicationReadyToPrint == true)
                 && (IsActivePrintheadReadyToPrint)
                 && (PrinterStatus == PrinterStatus.Manual))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public bool IsModiPrintGCodeReadyToPrint
        {
            get { return !String.IsNullOrWhiteSpace(_gCodeManagerViewModel.ModiPrintGCode); }
        }

        public bool IsSerialCommunicationReadyToPrint
        {
            get { return _serialCommunicationViewModel.IsPortOpen; }
        }

        public bool IsActivePrintheadReadyToPrint
        {
            get { return (_realTimeStatusDataModel.ActivePrintheadType != PrintheadType.Unset) ? true : false; }
        }
        #endregion

        #region Constructor
        public PrintExecuteViewModel(GCodeModel ModiPrintGCodeModel, GCodeManagerViewModel GCodeManagerViewModel, RealTimeStatusDataModel RealTimeStatusDataModel,
            SerialCommunicationViewModel SerialCommunicationViewModel, SerialCommunicationOutgoingMessagesModel SerialCommunicationOutgoingMessagesModel, SerialMessageDisplayViewModel SerialMessageDisplayViewModel)
        {
            _modiPrintGCodeModel = ModiPrintGCodeModel;
            _gCodeManagerViewModel = GCodeManagerViewModel;
            _realTimeStatusDataModel = RealTimeStatusDataModel;
            _serialCommunicationViewModel = SerialCommunicationViewModel;
            _serialCommunicationOutgoingMessagesModel = SerialCommunicationOutgoingMessagesModel;
            _serialMessageDisplayViewModel = SerialMessageDisplayViewModel;

            _gCodeManagerViewModel.ModiPrintGCodeChanged += new ModiPrintGCodeChangedEventHandler(UpdateModiPrintGCode);
            _serialCommunicationViewModel.SerialCommunicationMainModel.SerialConnectionChanged += new SerialConnectionChangedEventHandler(UpdateSerialConnection);
            _serialCommunicationViewModel.SerialCommunicationMainModel.SerialCommunicationCompleted += new SerialCommunicationCompletedEventHandler(UpdatePrintFinished);
            _realTimeStatusDataModel.RecordSetMotorizedPrintheadExecuted += new RecordSetMotorizedPrintheadExecutedEventHandler(UpdateActivePrintheadType);
            _realTimeStatusDataModel.RecordSetValvePrintheadExecuted += new RecordSetValvePrintheadExecutedEventHandler(UpdateActivePrintheadType);
            _realTimeStatusDataModel.RecordLimitExecuted += new RecordLimitExecutedEventHandler(UpdateLimitHit);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Queues each line of GCode for printing.
        /// </summary>
        public void Print()
        {
            //Queue each line of GCode for printing.
            string[] modiPrintGCodeArr = _modiPrintGCodeModel.GCodeStrToArr();
            for (int line = 0; line < modiPrintGCodeArr.Length; line++)
            {
                _serialCommunicationOutgoingMessagesModel.AppendProspectiveOutgoingMessage(modiPrintGCodeArr[line]);
            }
        }

        /// <summary>
        /// Linked to ModiPrintGCodeChangedEventHandler.
        /// Fires when ModiPrint GCode is converted or loaded.
        /// </summary>
        /// <param name="sender"></param>
        private void UpdateModiPrintGCode(object sender)
        {
            OnPropertyChanged("IsGCodeReadyToPrint");
        }

        /// <summary>
        /// Linked to SerialConnectionChangedEventHandler.
        /// Fires when serial port is connected or disconnected.
        /// </summary>
        /// <param name="sender"></param>
        private void UpdateSerialConnection(object sender)
        {
            OnPropertyChanged("IsSerialCommunicationReadyToPrint");
        }

        /// <summary>
        /// Linked to RecordSetMotorizedPrintheadExecutedEventHandler and RecordSetValvePrintheadExecutedEventHandler.
        /// Fires when a new Printhead is set.
        /// </summary>
        private void UpdateActivePrintheadType()
        {
            OnPropertyChanged("IsActivePrintheadReadyToPrint");
        }

        /// <summary>
        /// Linked to SerialCommunicationFinishedEventHandler.
        /// Fires when a Print sequence is finished,
        /// </summary>
        private void UpdatePrintFinished(object sender)
        {
            PrinterStatus = PrinterStatus.Manual;
            if (_printCommand != null)
            { _printCommand.RaiseCanExecuteChanged(); }
        }

        /// <summary>
        /// Linked to RecordLimitExecutedEventHandler.
        /// Fires when a Limit Switch is hit.
        /// </summary>
        private void UpdateLimitHit()
        {
            if (_printerStatus == PrinterStatus.Printing)
            {
                PrinterStatus = PrinterStatus.Paused;
                if (_pauseCommand != null)
                { _pauseCommand.RaiseCanExecuteChanged(); }
                if (_resumeCommand != null)
                { _resumeCommand.RaiseCanExecuteChanged(); }
            }
            else if (_printerStatus == PrinterStatus.Manual)
            {
                ShouldResume = true;
            }
        }
        #endregion

        #region Commands
        /// <summary>
        /// Begins printing.
        /// </summary>
        private RelayCommand<object> _printCommand;
        public ICommand PrintCommand
        {
            get
            {
                if (_printCommand == null)
                { _printCommand = new RelayCommand<object>(ExecutePrintCommand, CanExecutePrintCommand); }
                return _printCommand;
            }
        }

        //To Do: grey out the print button once printing has begun.
        public bool CanExecutePrintCommand(object notUsed)
        {
            return IsReadyToPrint;
        }

        public void ExecutePrintCommand(object notUsed)
        {
            PrinterStatus = PrinterStatus.Printing;
            Print();
        }

        /// <summary>
        /// Pauses printing.
        /// </summary>
        private RelayCommand<object> _pauseCommand;
        public ICommand PauseCommand
        {
            get
            {
                if (_pauseCommand == null)
                { _pauseCommand = new RelayCommand<object>(ExecutePauseCommand, CanExecutePauseCommand); }
                return _pauseCommand;
            }
        }

        public bool CanExecutePauseCommand(object notUsed)
        {
            return (_printerStatus == PrinterStatus.Printing) ? true : false;
        }

        public void ExecutePauseCommand(object notUsed)
        {
            _serialCommunicationViewModel.SerialCommunicationMainModel.ShouldPause = true;
            PrinterStatus = PrinterStatus.Paused;
        }

        /// <summary>
        /// Resumes a paused print.
        /// </summary>
        private RelayCommand<object> _resumeCommand;
        public ICommand ResumeCommand
        {
            get
            {
                if (_resumeCommand == null)
                { _resumeCommand = new RelayCommand<object>(ExecuteResumeCommand, CanExecuteResumeCommand); }
                return _resumeCommand;
            }
        }

        public bool CanExecuteResumeCommand(object notUsed)
        {
            return (_printerStatus == PrinterStatus.Paused) ? true : false;
        }

        public void ExecuteResumeCommand(object notUsed)
        {
            _serialCommunicationViewModel.SerialCommunicationMainModel.ShouldPause = false;
            PrinterStatus = PrinterStatus.Printing;
        }
        #endregion
    }
}