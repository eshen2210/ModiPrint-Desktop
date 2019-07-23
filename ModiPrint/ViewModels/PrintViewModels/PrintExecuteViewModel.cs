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
    public enum PrintStatus
    {
        Manual, //Idle or executing manual/calibration commands.
        Printing, //In the process of printing.
        MicrocontrollerPaused, //In the process of printing but the microcontroller protocol is on pause. No actions can be taken except unpausing.
        PrintSequencePaused //In the process of printing but this program's protocol is on pause. Manual actions may be taken before resuming.
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
        private PrintStatus _printStatus = PrintStatus.Manual;
        public PrintStatus PrintStatus
        {
            get { return _printStatus; }
            set
            {
                _printStatus = value;
                OnPropertyChanged("PrintStatus");
                OnPropertyChanged("IsReadyToPrint");
            }
        }

        //Set to true if all serial communication should be halted.
        public bool ShouldPauseMicrocontroller
        {
            get { return _serialCommunicationViewModel.SerialCommunicationMainModel.ShouldPauseMicrocontroller; }
            set { _serialCommunicationViewModel.SerialCommunicationMainModel.ShouldPauseMicrocontroller = value; }
        }

        //Set to true to resume hardware operations after pausing.
        public bool ShouldResume
        {
            get { return _serialCommunicationViewModel.SerialCommunicationMainModel.ShouldResumeMicrocontroller; }
            set { _serialCommunicationViewModel.SerialCommunicationMainModel.ShouldResumeMicrocontroller = value; }
        }

        //Are various aspects of this program ready to execute a Print?
        public bool IsReadyToPrint
        {
            get
            {
                if ((IsModiPrintGCodeReadyToPrint == true)
                 && (IsSerialCommunicationReadyToPrint == true)
                 && (IsActivePrintheadReadyToPrint)
                 && (PrintStatus == PrintStatus.Manual))
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
            _serialCommunicationViewModel.SerialCommunicationMainModel.SerialCommunicationPrintSequencePaused += new SerialCommunicationPrintSequencePausedEventHandler(UpdatePrintSequencePaused);
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
            OnPropertyChanged("IsModiPrintGCodeReadyToPrint");
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
        /// Fires when a print sequence is finished.
        /// </summary>
        private void UpdatePrintFinished(object sender)
        {
            PrintStatus = PrintStatus.Manual;
            OnPropertyChanged("PrintStatus");
            if (_printCommand != null)
            { _printCommand.RaiseCanExecuteChanged(); }
        }

        /// <summary>
        /// Linked to SerialCommunicationPrintSequencePausedEventHandler.
        /// Fires when a print sequence is paused.
        /// </summary>
        /// <param name="sender"></param>
        private void UpdatePrintSequencePaused(object sender)
        {
            PrintStatus = PrintStatus.PrintSequencePaused;
            OnPropertyChanged("PrintStatus");
            if (_resumeCommand != null)
            { _resumeCommand.RaiseCanExecuteChanged(); }
        }

        /// <summary>
        /// Linked to RecordLimitExecutedEventHandler.
        /// Fires when a Limit Switch is hit.
        /// </summary>
        private void UpdateLimitHit()
        {
            if (_printStatus == PrintStatus.Printing)
            {
                PrintStatus = PrintStatus.MicrocontrollerPaused;
                if (_pauseCommand != null)
                { _pauseCommand.RaiseCanExecuteChanged(); }
                if (_resumeCommand != null)
                { _resumeCommand.RaiseCanExecuteChanged(); }
            }
            else if (_printStatus == PrintStatus.Manual)
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
            PrintStatus = PrintStatus.Printing;
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
            return (_printStatus == PrintStatus.Printing) ? true : false;
        }

        public void ExecutePauseCommand(object notUsed)
        {
            _serialCommunicationViewModel.SerialCommunicationMainModel.ShouldPauseMicrocontroller = true;
            PrintStatus = PrintStatus.MicrocontrollerPaused;
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
            return ((_printStatus == PrintStatus.MicrocontrollerPaused) || (_printStatus == PrintStatus.PrintSequencePaused)) ? true : false;
        }

        public void ExecuteResumeCommand(object notUsed)
        {
            if (_printStatus == PrintStatus.MicrocontrollerPaused)
            {
                _serialCommunicationViewModel.SerialCommunicationMainModel.ShouldPauseMicrocontroller = false;
            }
            else if (_printStatus == PrintStatus.PrintSequencePaused)
            {
                _serialCommunicationViewModel.SerialCommunicationMainModel.ShouldResumePrintSequence = true;
            }
            
            PrintStatus = PrintStatus.Printing;
        }
        #endregion
    }
}