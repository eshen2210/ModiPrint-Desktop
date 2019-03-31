﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModiPrint.Models.RealTimeStatusModels;
using ModiPrint.Models.RealTimeStatusModels.RealTimeStatusPrintheadModels;
using ModiPrint.Models.SerialCommunicationModels;
using ModiPrint.Models.PrinterModels.PrintheadModels.PrintheadTypeModels;
using ModiPrint.ViewModels.RealTimeStatusViewModels.RealTimeStatusPrintheadViewModels;
using ModiPrint.ViewModels.RealTimeStatusViewModels.RealTimeStatusAxisViewModels;

namespace ModiPrint.ViewModels.RealTimeStatusViewModels
{
    public class RealTimeStatusDataViewModel : ViewModel
    {
        #region Fields and Properties
        //Contains information on the Printer during operation.
        RealTimeStatusDataModel _realTimeStatusDataModel;

        //Fires events that updates the Position property of RealTimeStatusDataModel equipment.
        SerialCommunicationCommandSetsModel _serialCommunicationCommandSetsModel;

        //Contains lists logging the various types of serial incoming messages.
        RealTimeStatusMessageListsViewModel _realTimeStatusMessageListsViewModel;
        public RealTimeStatusMessageListsViewModel RealTimeStatusMessageListsViewModel
        {
            get { return _realTimeStatusMessageListsViewModel; }
        }

        //Parameters of the Printer.

        //Parameters of each Axis.
        private RealTimeStatusAxisViewModel _xRealTimeStatusAxisViewModel;
        public RealTimeStatusAxisViewModel XRealTimeStatusAxisViewModel
        {
            get { return _xRealTimeStatusAxisViewModel; }
        }

        private RealTimeStatusAxisViewModel _yRealTimeStatusAxisViewModel;
        public RealTimeStatusAxisViewModel YRealTimeStatusAxisViewModel
        {
            get { return _yRealTimeStatusAxisViewModel; }
        }

        private RealTimeStatusAxisViewModel _zRealTimeStatusAxisViewModel;
        public RealTimeStatusAxisViewModel ZRealTimeStatusAxisViewModel
        {
            get { return _zRealTimeStatusAxisViewModel; }
        }

        //Parameters of the active Printhead.

        //Current type of the active Printhead.
        public PrintheadType ActivePrintheadType
        {
            get { return _realTimeStatusDataModel.ActivePrintheadType; }
        }

        //Parameters of the active Printhead.
        private RealTimeStatusPrintheadViewModel _activePrintheadViewModel;
        public RealTimeStatusPrintheadViewModel ActivePrintheadViewModel
        {
            get { return _activePrintheadViewModel; }
        }
        #endregion

        #region Constructor
        public RealTimeStatusDataViewModel(RealTimeStatusDataModel RealTimeStatusDataModel, SerialCommunicationCommandSetsModel SerialCommunicationCommandSetsModel)
        {
            _realTimeStatusDataModel = RealTimeStatusDataModel;
            _serialCommunicationCommandSetsModel = SerialCommunicationCommandSetsModel;
            _realTimeStatusMessageListsViewModel = new RealTimeStatusMessageListsViewModel(_realTimeStatusDataModel.RealTimeStatusMessageListsModel);

            _xRealTimeStatusAxisViewModel = new RealTimeStatusAxisViewModel(_realTimeStatusDataModel.XRealTimeStatusAxisModel);
            _yRealTimeStatusAxisViewModel = new RealTimeStatusAxisViewModel(_realTimeStatusDataModel.YRealTimeStatusAxisModel);
            _zRealTimeStatusAxisViewModel = new RealTimeStatusAxisViewModel(_realTimeStatusDataModel.ZRealTimeStatusAxisModel);

            UpdateSetPrinthead();

            //Subscribe to events.
            _realTimeStatusDataModel.RecordSetAxisExecuted += new RecordSetAxisExecutedEventHandler(UpdateRecordSetAxis);
            _realTimeStatusDataModel.RecordSetMotorizedPrintheadExecuted += new RecordSetMotorizedPrintheadExecutedEventHandler(UpdateSetPrinthead);
            _realTimeStatusDataModel.RecordSetValvePrintheadExecuted += new RecordSetValvePrintheadExecutedEventHandler(UpdateSetPrinthead);
            _realTimeStatusDataModel.RecordMoveAxesExecuted += new RecordMoveAxesExecutedEventHandler(UpdateRecordMoveAxes);
            _realTimeStatusDataModel.RecordMotorizedPrintWithMovementExecuted += new RecordMotorizedPrintWithMovementExecutedEventHandler(UpdateRecordMotorizedPrintWithMovement);
            _realTimeStatusDataModel.RecordValvePrintWithMovementExecuted += new RecordValvePrintWithMovementExecutedEventHandler(UpdateRecordValvePrintWithMovement);
            _realTimeStatusDataModel.RecordMotorizedPrintWithoutMovementExecuted += new RecordMotorizedPrintWithoutMovementExecutedEventHandler(UpdateRecordMotorizedPrintWithoutMovement);
            _realTimeStatusDataModel.RecordValvePrintWithoutMovementExecuted += new RecordValvePrintWithoutMovementExecutedEventHandler(UpdateRecordValvePrintWithoutMovement);
            _realTimeStatusDataModel.RecordValveCloseExecuted += new RecordValveCloseExecutedEventHandler(UpdateRecordValveClose);
            _realTimeStatusDataModel.TaskQueuedMessagesUpdated += new TaskQueuedMessagesUpdatedEventHandler(UpdateTaskQueuedMessages);
            _realTimeStatusDataModel.ErrorMessagesUpdated += new ErrorMessagesUpdatedEventHandler(UpdateErrorMessages);

            _realTimeStatusDataModel.RecordLimitExecuted += new RecordLimitExecutedEventHandler(UpdatePositions);

            _serialCommunicationCommandSetsModel.CommandSetPositionChanged += new CommandSetPositionChangedEventHandler(UpdatePositions);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Calls the OnPropertyChanged event and updates properties related to the Set Axis command.
        /// </summary>
        private void UpdateRecordSetAxis(char axisID)
        {
            switch (axisID)
            {
                case 'X':
                    _xRealTimeStatusAxisViewModel = new RealTimeStatusAxisViewModel(_realTimeStatusDataModel.XRealTimeStatusAxisModel);
                    OnPropertyChanged("XRealTimeStatusAxisViewModel");
                    break;
                case 'Y':
                    _yRealTimeStatusAxisViewModel = new RealTimeStatusAxisViewModel(_realTimeStatusDataModel.YRealTimeStatusAxisModel);
                    OnPropertyChanged("YRealTimeStatusAxisViewModel");
                    break;
                case 'Z':
                    _zRealTimeStatusAxisViewModel = new RealTimeStatusAxisViewModel(_realTimeStatusDataModel.ZRealTimeStatusAxisModel);
                    OnPropertyChanged("ZRealTimeStatusAxisViewModel");
                    break;
                default:
                    //Should never reach this point.
                    break;
            }
        }

        /// <summary>
        /// Instantiates a new ActivePrintheadViewModel with the appropriate derived class.
        /// </summary>
        private void UpdateSetPrinthead()
        {
            switch (_realTimeStatusDataModel.ActivePrintheadType)
            {
                case PrintheadType.Motorized:
                    _activePrintheadViewModel = new RealTimeStatusMotorizedPrintheadViewModel((RealTimeStatusMotorizedPrintheadModel)_realTimeStatusDataModel.ActivePrintheadModel);
                    break;
                case PrintheadType.Valve:
                    _activePrintheadViewModel = new RealTimeStatusValvePrintheadViewModel((RealTimeStatusValvePrintheadModel)_realTimeStatusDataModel.ActivePrintheadModel);
                    break;
                case PrintheadType.Custom:
                    _activePrintheadViewModel = new RealTimeStatusCustomPrintheadViewModel((RealTimeStatusCustomPrintheadModel)_realTimeStatusDataModel.ActivePrintheadModel);
                    break;
                case PrintheadType.Unset:
                    //Should never reach this point.
                    _activePrintheadViewModel = new RealTimeStatusUnsetPrintheadViewModel((RealTimeStatusUnsetPrintheadModel)_realTimeStatusDataModel.ActivePrintheadModel);
                    break;
            }

            OnPropertyChanged("ActivePrintheadViewModel");
            OnPropertyChanged("ActivePrintheadType");
        }

        /// <summary>
        /// Calls the OnPropertyChanged event and updates properties related to the Move Axes command.
        /// </summary>
        private void UpdateRecordMoveAxes()
        {
            if ((_xRealTimeStatusAxisViewModel != null)
             && (_yRealTimeStatusAxisViewModel != null)
             && (_zRealTimeStatusAxisViewModel != null))
            {
                _xRealTimeStatusAxisViewModel.UpdatePosition();
                _yRealTimeStatusAxisViewModel.UpdatePosition();
                _zRealTimeStatusAxisViewModel.UpdatePosition();
            }
        }

        /// <summary>
        /// Calls the OnPropertyChanged event and updates properties related to the Motorized Print With Movement command.
        /// </summary>
        private void UpdateRecordMotorizedPrintWithMovement()
        {
            if (_realTimeStatusDataModel.ActivePrintheadType == PrintheadType.Motorized)
            {
                RealTimeStatusMotorizedPrintheadViewModel realTimeStatusMotorizedPrintheadViewModel = (RealTimeStatusMotorizedPrintheadViewModel)_activePrintheadViewModel;

                if ((_xRealTimeStatusAxisViewModel != null)
                 && (_yRealTimeStatusAxisViewModel != null)
                 && (_zRealTimeStatusAxisViewModel != null)
                 && (realTimeStatusMotorizedPrintheadViewModel != null))
                {
                    _xRealTimeStatusAxisViewModel.UpdatePosition();
                    _yRealTimeStatusAxisViewModel.UpdatePosition();
                    _zRealTimeStatusAxisViewModel.UpdatePosition();
                    realTimeStatusMotorizedPrintheadViewModel.UpdatePosition();
                }
            }
        }

        /// <summary>
        /// Calls the OnPropertyChanged event and updates properties related to the Valve Print With Movement command.
        /// </summary>
        private void UpdateRecordValvePrintWithMovement()
        {
            if (_realTimeStatusDataModel.ActivePrintheadType == PrintheadType.Valve)
            {
                RealTimeStatusValvePrintheadViewModel realTimeStatusValvePrintheadViewModel = (RealTimeStatusValvePrintheadViewModel)_activePrintheadViewModel;

                if ((_xRealTimeStatusAxisViewModel != null)
                 && (_yRealTimeStatusAxisViewModel != null)
                 && (_zRealTimeStatusAxisViewModel != null)
                 && (realTimeStatusValvePrintheadViewModel != null))
                {
                    _xRealTimeStatusAxisViewModel.UpdatePosition();
                    _yRealTimeStatusAxisViewModel.UpdatePosition();
                    _zRealTimeStatusAxisViewModel.UpdatePosition();
                    realTimeStatusValvePrintheadViewModel.UpdateIsValveOn();
                }
            }
        }

        /// <summary>
        /// Calls the OnPropertyChanged event and updates properties related to the Motorized Print Without Movement command.
        /// </summary>
        private void UpdateRecordMotorizedPrintWithoutMovement()
        {
            if (_realTimeStatusDataModel.ActivePrintheadType == PrintheadType.Motorized)
            {
                RealTimeStatusMotorizedPrintheadViewModel realTimeStatusMotorizedPrintheadViewModel = (RealTimeStatusMotorizedPrintheadViewModel)_activePrintheadViewModel;

                if (realTimeStatusMotorizedPrintheadViewModel != null)
                {
                    realTimeStatusMotorizedPrintheadViewModel.UpdatePosition();
                }
            }
        }

        /// <summary>
        /// Calls the OnPropertyChanged event and updates properties related to the Valve Print Without Movement command.
        /// </summary>
        private void UpdateRecordValvePrintWithoutMovement()
        {
            if (_realTimeStatusDataModel.ActivePrintheadType == PrintheadType.Valve)
            {
                RealTimeStatusValvePrintheadViewModel realTimeStatusValvePrintheadViewModel = (RealTimeStatusValvePrintheadViewModel)_activePrintheadViewModel;

                if (realTimeStatusValvePrintheadViewModel != null)
                {
                    realTimeStatusValvePrintheadViewModel.UpdateIsValveOn();
                }
            }
        }

        /// <summary>
        /// Calls the OnPropertyChanged event and updates properties related to the Valve Close command.
        /// </summary>
        private void UpdateRecordValveClose()
        {
            if (_realTimeStatusDataModel.ActivePrintheadType == PrintheadType.Valve)
            {
                RealTimeStatusValvePrintheadViewModel realTimeStatusValvePrintheadViewModel = (RealTimeStatusValvePrintheadViewModel)_activePrintheadViewModel;

                if (realTimeStatusValvePrintheadViewModel != null)
                {
                    realTimeStatusValvePrintheadViewModel.UpdateIsValveOn();
                }
            }
        }

        /// <summary>
        /// Calls the OnPropertyChanged event and updates the Task Queued Messages list.
        /// </summary>
        private void UpdateTaskQueuedMessages()
        {
            OnPropertyChanged("TaskQueuedMessages");
        }

        /// <summary>
        /// Calls the OnPropertyChanged event and updates the Status Messages list.
        /// </summary>
        private void UpdateStatusMessages()
        {
            OnPropertyChanged("StatusMessages");
        }

        /// <summary>
        /// Calls the OnPropertyChanged event and updates the Error Messages list.
        /// </summary>
        private void UpdateErrorMessages()
        {
            OnPropertyChanged("ErrorMessages");
        }

        /// <summary>
        /// Calls OnPropertyChanged on the Position and IsValveOpen (in case this was called on a Valve movement) for every Axis and Printhead.
        /// Typically called during automated calibration.
        /// </summary>
        public void UpdateLimit()
        {
            UpdatePositions();

            if ((_realTimeStatusDataModel.ActivePrintheadType == PrintheadType.Valve)
             && (_activePrintheadViewModel != null))
            {

            }
        }

        /// <summary>
        /// Calls OnPropertyChanged on the Position properties for every Axis and Motorized Printhead.
        /// Typically called during automated calibration.
        /// </summary>
        public void UpdatePositions()
        {
            if ((_xRealTimeStatusAxisViewModel != null)
             && (_yRealTimeStatusAxisViewModel != null)
             && (_zRealTimeStatusAxisViewModel != null))
            {
                _xRealTimeStatusAxisViewModel.UpdatePosition();
                _yRealTimeStatusAxisViewModel.UpdatePosition();
                _zRealTimeStatusAxisViewModel.UpdatePosition();
            }

            if ((_realTimeStatusDataModel.ActivePrintheadType == PrintheadType.Motorized)
             && (_activePrintheadViewModel != null))
            {
                RealTimeStatusMotorizedPrintheadViewModel realTimeStatusMotorizedPrintheadViewModel = (RealTimeStatusMotorizedPrintheadViewModel)_activePrintheadViewModel;

                realTimeStatusMotorizedPrintheadViewModel.UpdatePosition();
            }
        }
        #endregion
    }
}
