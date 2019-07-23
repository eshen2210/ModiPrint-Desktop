using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using ModiPrint.DataTypes.GlobalValues;
using ModiPrint.Models.PrinterModels;
using ModiPrint.Models.PrinterModels.AxisModels;
using ModiPrint.Models.PrinterModels.PrintheadModels;
using ModiPrint.Models.RealTimeStatusModels;
using ModiPrint.Models.SerialCommunicationModels;
using ModiPrint.Models.GCodeConverterModels.ProcessModels.WriteSetEquipmentModels;
using ModiPrint.Models.GCodeConverterModels;
using ModiPrint.Models.GCodeConverterModels.ProcessModels.ProcessG00Models;
using ModiPrint.ViewModels;

namespace ModiPrint.Models.ManualControlModels
{
    /// <summary>
    /// Contains functions that automatically calibrate the Printer's position and range.
    /// </summary>
    public class CalibrationModel
    {
        #region Fields and Properties
        //Contains information on the Printer during operation.
        RealTimeStatusDataModel _realTimeStatusDataModel;

        //Contains information on the Printer.
        PrinterModel _printerModel;

        //Contains functions to output commands.
        WriteSetAxisModel _writeSetAxisModel;

        //Contains functions to queue outgoing messages for the serial port.
        SerialCommunicationOutgoingMessagesModel _serialCommunicationOutgoingMessagesModel;
        #endregion

        #region Constructor
        public CalibrationModel(RealTimeStatusDataModel RealTimeStatusDataModel, PrinterModel PrinterModel, SerialCommunicationOutgoingMessagesModel SerialCommunicationOutgoingMessagesModel, ErrorListViewModel ErrorListViewModel)
        {
            _realTimeStatusDataModel = RealTimeStatusDataModel;
            _printerModel = PrinterModel;
            _serialCommunicationOutgoingMessagesModel = SerialCommunicationOutgoingMessagesModel;

            ParametersModel parametersModel = new ParametersModel(_printerModel, null);
            _writeSetAxisModel = new WriteSetAxisModel(parametersModel);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Sends outgoing commands that retracts all Z Axes and moves X and Y Axes to the limit switches.
        /// </summary>
        public void CalibrateXYAndZMax()
        {
            //Retract Z Axes.
            RetractAllZ();

            //Set X Axis to max speeds.
            AxisModel xAxis = _printerModel.AxisModelList[0];
            string switchX = _writeSetAxisModel.WriteSetAxis(xAxis);
            _serialCommunicationOutgoingMessagesModel.AppendProspectiveOutgoingMessage(switchX);

            //Hit the min and max limit switches on X.
            string xPositive = GCodeLinesConverter.GCodeLinesListToString(
                WriteG00.WriteAxesMovement(
                xAxis.MmPerStep, 0, 0,
                5000, 0, 0,
                xAxis.IsDirectionInverted, false, false));
            string xNegative = GCodeLinesConverter.GCodeLinesListToString(
                WriteG00.WriteAxesMovement(
                xAxis.MmPerStep, 0, 0,
                -5000, 0, 0,
                xAxis.IsDirectionInverted, false, false));
            _serialCommunicationOutgoingMessagesModel.AppendProspectiveOutgoingMessage(xPositive);
            _serialCommunicationOutgoingMessagesModel.AppendProspectiveOutgoingMessage(xNegative);

            //Move away from the limit switch.
            string xMoveAwayFromLimit = GCodeLinesConverter.GCodeLinesListToString(
                WriteG00.WriteAxesMovement(
                xAxis.MmPerStep, 0, 0,
                GlobalValues.LimitBuffer, 0, 0,
                xAxis.IsDirectionInverted, false, false));
            _serialCommunicationOutgoingMessagesModel.AppendProspectiveOutgoingMessage(xMoveAwayFromLimit);

            //Set Y Axis to max speeds.
            AxisModel yAxis = _printerModel.AxisModelList[1];
            string switchY = _writeSetAxisModel.WriteSetAxis(yAxis);
            _serialCommunicationOutgoingMessagesModel.AppendProspectiveOutgoingMessage(switchY);

            //Hit the min and max limit switches on Y.
            string yPositive = GCodeLinesConverter.GCodeLinesListToString(
                WriteG00.WriteAxesMovement(
                0, yAxis.MmPerStep, 0,
                0, 5000, 0,
                yAxis.IsDirectionInverted, false, false));
            string yNegative = GCodeLinesConverter.GCodeLinesListToString(
                WriteG00.WriteAxesMovement(
                0, yAxis.MmPerStep, 0,
                0, -5000, 0,
                yAxis.IsDirectionInverted, false, false));
            _serialCommunicationOutgoingMessagesModel.AppendProspectiveOutgoingMessage(yPositive);
            _serialCommunicationOutgoingMessagesModel.AppendProspectiveOutgoingMessage(yNegative);

            //Move away from the limit switch.
            string yMoveAwayFromLimit = GCodeLinesConverter.GCodeLinesListToString(
                WriteG00.WriteAxesMovement(
                0, yAxis.MmPerStep, 0,
                0, GlobalValues.LimitBuffer, 0,
                yAxis.IsDirectionInverted, false, false));
            _serialCommunicationOutgoingMessagesModel.AppendProspectiveOutgoingMessage(yMoveAwayFromLimit);
        }

        /// <summary>
        /// Center X and Y and set the new position as the origin for X and Y.
        /// </summary>
        public void CalibrateXYOrigin(double xDistanceFromCenter, double yDistanceFromCenter)
        {
            //Set X Axis to max speeds.
            AxisModel xAxis = _printerModel.AxisModelList[0];
            string switchX = _writeSetAxisModel.WriteSetAxis(xAxis);
            _serialCommunicationOutgoingMessagesModel.AppendProspectiveOutgoingMessage(switchX);

            //Set Y Axis to max speeds.
            AxisModel yAxis = _printerModel.AxisModelList[1];
            string switchY = _writeSetAxisModel.WriteSetAxis(yAxis);
            _serialCommunicationOutgoingMessagesModel.AppendProspectiveOutgoingMessage(switchY);

            //Center the X and Y actuators.
            _serialCommunicationOutgoingMessagesModel.AppendProspectiveOutgoingMessage(SerialMessageCharacters.SerialCommandSetCharacter + "Center X" + xDistanceFromCenter + " Y" + yDistanceFromCenter);
            _serialCommunicationOutgoingMessagesModel.AppendProspectiveOutgoingMessage(SerialMessageCharacters.SerialCommandSetCharacter + "OriginXY");
        }

        /// <summary>
        /// Sends outgoing commands that mark the position of the currently active Printhead's point to printing (centered and closest to the print surface).
        /// Should only be called after Z actuators have hit their top limit switches this session.
        /// All Printheads should be calibrated within a single session as X and Y Offset positions are relative.
        /// Z Offset is simply the zero position of the Z actuator.
        /// </summary>
        public void CalibratePrintheadOffset()
        {
            //Set Offset values for the current Printhead.
            PrintheadModel printheadModel = _printerModel.FindPrinthead(_realTimeStatusDataModel.ActivePrintheadModel.Name);
            printheadModel.XOffset = _realTimeStatusDataModel.XRealTimeStatusAxisModel.Position;
            printheadModel.YOffset = _realTimeStatusDataModel.YRealTimeStatusAxisModel.Position;

            //Set current Z Axis MinPosition to 0.
            string setZMin = SerialMessageCharacters.SerialCommandSetCharacter + "SetMinMaxPos ZN0";
            _serialCommunicationOutgoingMessagesModel.AppendProspectiveOutgoingMessage(setZMin);

            //Maximize Z Axis speeds.
            string setZMaxSpeed = _writeSetAxisModel.WriteSetAxis(printheadModel.AttachedZAxisModel);
            _serialCommunicationOutgoingMessagesModel.AppendProspectiveOutgoingMessage(setZMaxSpeed);

            //The next Z Limit Switch hit will calibrate minmax positions.
            _realTimeStatusDataModel.ShouldZCalibrate = true;

            //Retracts the Z Axis up to a short distance away from the Limit Switch (does not hit the Limit Switch).
            string retractZ = SerialMessageCharacters.SerialCommandSetCharacter + "RetractZLimit";
            _serialCommunicationOutgoingMessagesModel.AppendProspectiveOutgoingMessage(retractZ);
        }

        /// <summary>
        /// Sends outgoing commands that retracts all Z Axes.
        /// </summary>
        private void RetractAllZ()
        {
            foreach (AxisModel zAxisModel in _printerModel.ZAxisModelList)
            {
                //Switch to another Z Axis, hit the upper Limit Switch, and then move to default position.
                string switchZ = _writeSetAxisModel.WriteSetAxis(zAxisModel);
                _serialCommunicationOutgoingMessagesModel.AppendProspectiveOutgoingMessage(switchZ);
                _serialCommunicationOutgoingMessagesModel.AppendProspectiveOutgoingMessage(SerialMessageCharacters.SerialCommandSetCharacter + "RetractZLimit");
;
            }
        }
        #endregion
    }
}
