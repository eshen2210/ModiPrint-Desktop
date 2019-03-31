using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ModiPrint.DataTypes.GlobalValues;
using ModiPrint.Models.PrinterModels;
using ModiPrint.Models.PrinterModels.AxisModels;
using ModiPrint.Models.PrinterModels.PrintheadModels;
using ModiPrint.Models.PrinterModels.PrintheadModels.PrintheadTypeModels;
using ModiPrint.Models.RealTimeStatusModels;
using ModiPrint.Models.RealTimeStatusModels.RealTimeStatusPrintheadModels;
using ModiPrint.Models.SerialCommunicationModels;
using ModiPrint.Models.GCodeConverterModels;
using ModiPrint.Models.GCodeConverterModels.ProcessModels.WriteSetEquipmentModels;
using ModiPrint.Models.GCodeConverterModels.ProcessModels.ProcessG00Models;
using ModiPrint.ViewModels;

namespace ModiPrint.Models.ManualControlModels
{
    public class ManualControlModel
    {
        #region Fields and Methods
        //Contains information on the Printer.
        PrinterModel _printerModel;

        //Contains functions to queue outgoing messages for the serial port.
        SerialCommunicationOutgoingMessagesModel _serialCommunicationOutgoingMessagesModel;

        //Contains information on the state of printer information during operation.
        RealTimeStatusDataModel _realTimeStatusDataModel;

        //Contains functions to generate commands.
        WriteSetPrintheadModel _writeSetPrintheadModel;
        WriteSetAxisModel _writeSetAxisModel;
        #endregion

        #region Constructor
        public ManualControlModel(PrinterModel PrinterModel, SerialCommunicationOutgoingMessagesModel SerialCommunicationOutgoingMessagesModel,
            RealTimeStatusDataModel RealTimeStatusDataModel)
        {
            _printerModel = PrinterModel;
            _serialCommunicationOutgoingMessagesModel = SerialCommunicationOutgoingMessagesModel;
            _realTimeStatusDataModel = RealTimeStatusDataModel;

            ParametersModel parametersModel = new ParametersModel(_printerModel, null);

            _writeSetPrintheadModel = new WriteSetPrintheadModel(parametersModel);
            _writeSetAxisModel = new WriteSetAxisModel(parametersModel);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Takes manual input parameters for a movement command and outputs the command to the serial port.
        /// </summary>
        /// <param name="xDistance"></param>
        /// <param name="yDistance"></param>
        /// <param name="zDistance"></param>
        public void ProcessManualMovementCommand(double xDistance, double yDistance, double zDistance)
        {
            double xmmPerStep = (xDistance != 0) ? _printerModel.AxisModelList[0].MmPerStep : double.MaxValue;
            double ymmPerStep = (yDistance != 0) ? _printerModel.AxisModelList[1].MmPerStep : double.MaxValue;
            AxisModel zAxisModel = _printerModel.FindAxis(_realTimeStatusDataModel.ZRealTimeStatusAxisModel.Name);
            double zmmPerStep = (zDistance != 0) ? _printerModel.FindAxis(zAxisModel.Name).MmPerStep : double.MaxValue;
            bool zDirectionInverted = (zAxisModel != null) ? zAxisModel.IsDirectionInverted : false;

            string convertedGCode = GCodeLinesConverter.GCodeLinesListToString(
                WriteG00.WriteAxesMovement(xmmPerStep, ymmPerStep, zmmPerStep,
                xDistance, yDistance, zDistance,
                _printerModel.AxisModelList[0].IsDirectionInverted, _printerModel.AxisModelList[1].IsDirectionInverted, zDirectionInverted));

            _serialCommunicationOutgoingMessagesModel.AppendProspectiveOutgoingMessage(convertedGCode);
        }

        /// <summary>
        /// Takes manual input parameters for a motor print with movement command and outputs the command to the serial port.
        /// </summary>
        /// <param name="xDistance"></param>
        /// <param name="yDistance"></param>
        /// <param name="zDistance"></param>
        /// <param name="eDispensePerDistance"></param>
        public void ProcessManualMotorPrintWithMovementCommand(double xDistance, double yDistance, double zDistance, double eDispensePerDistance)
        {
            MotorizedPrintheadTypeModel motorizedPrintheadTypeModel = (MotorizedPrintheadTypeModel)_printerModel.FindPrinthead(_realTimeStatusDataModel.ActivePrintheadModel.Name).PrintheadTypeModel;
            double emmPerStep = motorizedPrintheadTypeModel.MmPerStep;
            double xmmPerStep = (xDistance != 0) ? _printerModel.AxisModelList[0].MmPerStep : double.MaxValue;
            double ymmPerStep = (yDistance != 0) ? _printerModel.AxisModelList[1].MmPerStep : double.MaxValue;
            AxisModel zAxisModel = _printerModel.FindAxis(_realTimeStatusDataModel.ZRealTimeStatusAxisModel.Name);
            double zmmPerStep = (zDistance != 0) ? _printerModel.FindAxis(zAxisModel.Name).MmPerStep : double.MaxValue;
            bool zDirectionInverted = (zAxisModel != null) ? zAxisModel.IsDirectionInverted : false;

            string convertedGCode = GCodeLinesConverter.GCodeLinesListToString(
                WriteG00.WriteMotorizedContinuousPrint(emmPerStep, xmmPerStep, ymmPerStep, zmmPerStep,
                eDispensePerDistance, xDistance, yDistance, zDistance,
                _printerModel.AxisModelList[0].IsDirectionInverted, _printerModel.AxisModelList[1].IsDirectionInverted, zDirectionInverted, motorizedPrintheadTypeModel.IsDirectionInverted,
                null));

            _serialCommunicationOutgoingMessagesModel.AppendProspectiveOutgoingMessage(convertedGCode);
        }

        /// <summary>
        /// Takes manual input parameters for a motor print without movement command and outputs the command to the serial port.
        /// </summary>
        /// <param name="eDistance"></param>
        public void ProcessManualMotorPrintWithoutMovementCommand(double eDistance)
        {
            MotorizedPrintheadTypeModel motorizedPrintheadTypeModel = (MotorizedPrintheadTypeModel)_printerModel.FindPrinthead(_realTimeStatusDataModel.ActivePrintheadModel.Name).PrintheadTypeModel;
            double emmPerStep = motorizedPrintheadTypeModel.MmPerStep;
            RealTimeStatusMotorizedPrintheadModel realTimeStatusMotorizedPrintheadModel = (RealTimeStatusMotorizedPrintheadModel)_realTimeStatusDataModel.ActivePrintheadModel;

            string convertedGCode = GCodeLinesConverter.GCodeLinesListToString(
                WriteG00.WriteMotorizedPrintWithoutMovement(eDistance, emmPerStep, motorizedPrintheadTypeModel.IsDirectionInverted));

            _serialCommunicationOutgoingMessagesModel.AppendProspectiveOutgoingMessage(convertedGCode);
        }

        /// <summary>
        /// Takes manual input parameters for a set of commands for motorized printhead droplet printing and outputs the commands to the serial port.
        /// </summary>
        /// <param name="xDistance"></param>
        /// <param name="yDistance"></param>
        /// <param name="zDistance"></param>
        /// <param name="interpolateDistance"></param>
        /// <param name="interpolateRoundUp"></param>
        /// <param name="eDispensePerDroplet"></param>
        public void ProcessManualMotorDropletPrintCommand(double xDistance, double yDistance, double zDistance, double interpolateDistance, bool interpolateRoundUp, double eDispensePerDroplet)
        {
            PrintheadModel printheadModel = _printerModel.FindPrinthead(_realTimeStatusDataModel.ActivePrintheadModel.Name);
            MotorizedPrintheadTypeModel motorizedPrintheadTypeModel = (MotorizedPrintheadTypeModel)printheadModel.PrintheadTypeModel;
            double emmPerStep = motorizedPrintheadTypeModel.MmPerStep;
            RealTimeStatusMotorizedPrintheadModel realTimeStatusMotorizedPrintheadModel = (RealTimeStatusMotorizedPrintheadModel)_realTimeStatusDataModel.ActivePrintheadModel;
            double eMaxSpeed = realTimeStatusMotorizedPrintheadModel.MaxSpeed;
            double eAcceleration = realTimeStatusMotorizedPrintheadModel.Acceleration;
            double xmmPerStep = (xDistance != 0) ? _printerModel.AxisModelList[0].MmPerStep : double.MaxValue;
            double ymmPerStep = (yDistance != 0) ? _printerModel.AxisModelList[1].MmPerStep : double.MaxValue;
            AxisModel zAxisModel = _printerModel.FindAxis(_realTimeStatusDataModel.ZRealTimeStatusAxisModel.Name);
            double zmmPerStep = (zDistance != 0) ? _printerModel.FindAxis(zAxisModel.Name).MmPerStep : double.MaxValue;
            bool zDirectionInverted = (zAxisModel != null) ? zAxisModel.IsDirectionInverted : false;

            string convertedGCode = GCodeLinesConverter.GCodeLinesListToString(
                WriteG00.WriteMotorizedDropletPrint(
                emmPerStep, xmmPerStep, ymmPerStep, zmmPerStep,
                eDispensePerDroplet, 0, xDistance, 0, yDistance, 0, zDistance,
                _printerModel.AxisModelList[0].IsDirectionInverted, _printerModel.AxisModelList[1].IsDirectionInverted, zDirectionInverted, motorizedPrintheadTypeModel.IsDirectionInverted,
                null, new DropletModel(interpolateDistance)));

            SendCommandSet(convertedGCode);
        }

        /// <summary>
        /// Takes manual input parameters for a valve print with movement command and outputs the command to the serial port.
        /// </summary>
        /// <param name="xDistance"></param>
        /// <param name="yDistance"></param>
        /// <param name="zDistance"></param>
        public void ProcessManualValvePrintWithMovementCommand(double xDistance, double yDistance, double zDistance)
        {
            double xmmPerStep = (xDistance != 0) ? _printerModel.AxisModelList[0].MmPerStep : double.MaxValue;
            double ymmPerStep = (yDistance != 0) ? _printerModel.AxisModelList[1].MmPerStep : double.MaxValue;
            AxisModel zAxisModel = _printerModel.FindAxis(_realTimeStatusDataModel.ZRealTimeStatusAxisModel.Name);
            double zmmPerStep = (zDistance != 0) ? _printerModel.FindAxis(zAxisModel.Name).MmPerStep : double.MaxValue;
            bool zDirectionInverted = (zAxisModel != null) ? zAxisModel.IsDirectionInverted : false;

            string convertedGCode = GCodeLinesConverter.GCodeLinesListToString(
                WriteG00.WriteValveContinuousPrint(xmmPerStep, ymmPerStep, zmmPerStep, 0, xDistance, 0, yDistance, 0, zDistance,
                _printerModel.AxisModelList[0].IsDirectionInverted, _printerModel.AxisModelList[1].IsDirectionInverted, zDirectionInverted));

            _serialCommunicationOutgoingMessagesModel.AppendProspectiveOutgoingMessage(convertedGCode);
        }

        /// <summary>
        /// Takes manual input parameters for a valve print without movement command and outputs the command to the serial port.
        /// </summary>
        /// <param name="openTime"></param>
        public void ProcessManualValvePrintWithoutMovementCommand(int valveOpenTime)
        {
            string convertedGCode = GCodeLinesConverter.GCodeLinesListToString(
                WriteG00.WriteValvePrintWithoutMovement(valveOpenTime));

            _serialCommunicationOutgoingMessagesModel.AppendProspectiveOutgoingMessage(convertedGCode);
        }

        /// <summary>
        /// Takes manual input parameters for a set of commands for valve printhead droplet printing and outputs the commands to the serial port.
        /// </summary>
        /// <param name="xDistance"></param>
        /// <param name="yDistance"></param>
        /// <param name="zDistance"></param>
        /// <param name="interpolateDistance"></param>
        /// <param name="interpolateRoundUp"></param>
        /// <param name="valveOpenTime"></param>
        public void ProcessManualValveDropletPrintCommand(double xDistance, double yDistance, double zDistance, double interpolateDistance, bool interpolateRoundUp, int valveOpenTime)
        {
            double xmmPerStep = (xDistance != 0) ? _printerModel.AxisModelList[0].MmPerStep : double.MaxValue;
            double ymmPerStep = (yDistance != 0) ? _printerModel.AxisModelList[1].MmPerStep : double.MaxValue;
            AxisModel zAxisModel = _printerModel.FindAxis(_realTimeStatusDataModel.ZRealTimeStatusAxisModel.Name);
            double zmmPerStep = (zDistance != 0) ? _printerModel.FindAxis(zAxisModel.Name).MmPerStep : double.MaxValue;
            bool zDirectionInverted = (zAxisModel != null) ? zAxisModel.IsDirectionInverted : false;

            string convertedGCode = GCodeLinesConverter.GCodeLinesListToString(
                WriteG00.WriteValveDropletPrint(xmmPerStep, ymmPerStep, zmmPerStep, valveOpenTime, 
                0, xDistance, 0, yDistance, 0, zDistance,
                _printerModel.AxisModelList[0].IsDirectionInverted, _printerModel.AxisModelList[1].IsDirectionInverted, zDirectionInverted,
                new DropletModel(interpolateDistance)));

            SendCommandSet(convertedGCode);
        }

        /// <summary>
        /// Takes manual input parameters for a valve close command and outputs the command to the serial port.
        /// </summary>
        public void ProcessManualValveCloseCommand()
        {
            string convertedGCode = GCodeLinesConverter.GCodeLinesListToString(WriteG00.WriteValveClose());

            _serialCommunicationOutgoingMessagesModel.AppendProspectiveOutgoingMessage(convertedGCode);
        }

        /// <summary>
        /// Takes manual input parameters for a set axis command and outputs the command to the serial port.
        /// </summary>
        /// <param name="axisName"></param>
        /// <param name="maxSpeed"></param>
        /// <param name="acceleration"></param>
        public void ProcessManualSetAxisCommand(string axisName, double maxSpeed, double acceleration)
        {
            AxisModel axisModel = _printerModel.FindAxis(axisName);

            int limitPinID = (axisModel.AttachedLimitSwitchGPIOPinModel == null) ? GlobalValues.NullPinID : axisModel.AttachedLimitSwitchGPIOPinModel.PinID;

            string convertedGCode = _writeSetAxisModel.WriteSetAxis(axisModel.AxisID, axisModel.AttachedMotorStepGPIOPinModel.PinID, axisModel.AttachedMotorDirectionGPIOPinModel.PinID,
                axisModel.StepPulseTime, limitPinID, maxSpeed, acceleration, axisModel.MmPerStep);

            //If switching Z Axes, then retract the current Z Axis first.
            if (axisModel.AxisID == 'Z')
            { _serialCommunicationOutgoingMessagesModel.AppendProspectiveOutgoingMessage(SerialMessageCharacters.SerialCommandSetCharacter + "RetractZ"); }
            _serialCommunicationOutgoingMessagesModel.AppendProspectiveOutgoingMessage(convertedGCode);
        }

        /// <summary>
        /// Takes manual input parameters for a set motorized printhead command and outputs the command to the serial port.
        /// </summary>
        /// <param name="printheadName"></param>
        /// <param name="maxSpeed"></param>
        /// <param name="acceleration"></param>
        public void ProcessManualSetMotorizedPrintheadCommand(string printheadName, double maxSpeed, double acceleration)
        {
            PrintheadModel printheadModel = _printerModel.FindPrinthead(printheadName);
            MotorizedPrintheadTypeModel motorizedPrintheadTypeModel = (MotorizedPrintheadTypeModel)printheadModel.PrintheadTypeModel;

            int printheadLimitPinID = (motorizedPrintheadTypeModel.AttachedLimitSwitchGPIOPinModel == null) ? GlobalValues.NullPinID : motorizedPrintheadTypeModel.AttachedLimitSwitchGPIOPinModel.PinID;
            string convertedGCode = _writeSetPrintheadModel.WriteSetMotorDrivenPrinthead(motorizedPrintheadTypeModel.AttachedMotorStepGPIOPinModel.PinID, motorizedPrintheadTypeModel.AttachedMotorDirectionGPIOPinModel.PinID,
                motorizedPrintheadTypeModel.StepPulseTime, printheadLimitPinID, maxSpeed, acceleration, motorizedPrintheadTypeModel.MmPerStep);
            _serialCommunicationOutgoingMessagesModel.AppendProspectiveOutgoingMessage(convertedGCode);

            //Send GCode for the Z Axis attached to the Printhead.
            AxisModel axisModel = printheadModel.AttachedZAxisModel;

            int zAxisLimitPinID = (axisModel.AttachedLimitSwitchGPIOPinModel == null) ? GlobalValues.NullPinID : axisModel.AttachedLimitSwitchGPIOPinModel.PinID;
            string convertedGCode2 = _writeSetAxisModel.WriteSetAxis(axisModel.AxisID, axisModel.AttachedMotorStepGPIOPinModel.PinID, axisModel.AttachedMotorDirectionGPIOPinModel.PinID,
                axisModel.StepPulseTime, zAxisLimitPinID, axisModel.MaxSpeed, axisModel.MaxAcceleration, axisModel.MmPerStep);

            //Retract Z Axis before changing Printheads.
            _serialCommunicationOutgoingMessagesModel.AppendProspectiveOutgoingMessage(SerialMessageCharacters.SerialCommandSetCharacter + "RetractZ");
            _serialCommunicationOutgoingMessagesModel.AppendProspectiveOutgoingMessage(convertedGCode2);
        }

        /// <summary>
        /// Takes manual input parameters for a set of valve printhead command and outputs the command to the serial port.
        /// </summary>
        /// <param name="printheadName"></param>
        public void ProcessManualSetValvePrintheadCommand(string printheadName)
        {
            PrintheadModel printheadModel = _printerModel.FindPrinthead(printheadName);
            ValvePrintheadTypeModel valvePrintheadTypeModel = (ValvePrintheadTypeModel)printheadModel.PrintheadTypeModel;
            string convertedGCode = _writeSetPrintheadModel.WriteSetValvePrinthead(valvePrintheadTypeModel.AttachedValveGPIOPinModel.PinID);
            _serialCommunicationOutgoingMessagesModel.AppendProspectiveOutgoingMessage(convertedGCode);

            //Send GCode for the Z Axis attached to the Printhead.
            AxisModel axisModel = printheadModel.AttachedZAxisModel;
            int zAxisLimitPinID = (axisModel.AttachedLimitSwitchGPIOPinModel == null) ? GlobalValues.NullPinID : axisModel.AttachedLimitSwitchGPIOPinModel.PinID;
            string convertedGCode2 = _writeSetAxisModel.WriteSetAxis(axisModel.AxisID, axisModel.AttachedMotorStepGPIOPinModel.PinID, axisModel.AttachedMotorDirectionGPIOPinModel.PinID,
                axisModel.StepPulseTime, zAxisLimitPinID, axisModel.MaxSpeed, axisModel.MaxAcceleration, axisModel.MmPerStep);

            _serialCommunicationOutgoingMessagesModel.AppendProspectiveOutgoingMessage(SerialMessageCharacters.SerialCommandSetCharacter + "RetractZ");
            _serialCommunicationOutgoingMessagesModel.AppendProspectiveOutgoingMessage(convertedGCode2);
        }

        /// <summary>
        /// Sends a set of commands through the serial port.
        /// </summary>
        /// <param name="commandSet"></param>
        private void SendCommandSet(string commandSet)
        {
            string[] commands = commandSet.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            foreach (string command in commands)
            {
                _serialCommunicationOutgoingMessagesModel.AppendProspectiveOutgoingMessage(command);
            }
        }
        #endregion
    }
}
