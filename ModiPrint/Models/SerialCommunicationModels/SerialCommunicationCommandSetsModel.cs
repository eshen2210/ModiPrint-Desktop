using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using ModiPrint.DataTypes.GlobalValues;
using ModiPrint.Models.PrinterModels;
using ModiPrint.Models.PrinterModels.AxisModels;
using ModiPrint.Models.PrinterModels.PrintheadModels;
using ModiPrint.Models.PrinterModels.PrintheadModels.PrintheadTypeModels;
using ModiPrint.Models.PrintModels;
using ModiPrint.Models.PrintModels.MaterialModels;
using ModiPrint.Models.RealTimeStatusModels;
using ModiPrint.Models.RealTimeStatusModels.RealTimeStatusAxisModels;
using ModiPrint.Models.RealTimeStatusModels.RealTimeStatusPrintheadModels;
using ModiPrint.Models.GCodeConverterModels;
using ModiPrint.Models.GCodeConverterModels.ProcessModels.ProcessG00Models;
using ModiPrint.Models.GCodeConverterModels.ProcessModels.ProcessTCommandModels;
using ModiPrint.Models.GCodeConverterModels.ProcessModels.WriteSetEquipmentModels;
using ModiPrint.ViewModels;

namespace ModiPrint.Models.SerialCommunicationModels
{
    //Called whenever Position property values are changed via a command set.
    public delegate void CommandSetPositionChangedEventHandler();

    //Called whenever a Min or Max Position property value is changed via a command set.
    public delegate void CommandSetMinMaxPositionChangedEventHandler();

    /// <summary>
    /// Contains functions to replace command series with commands.
    /// </summary>
    public class SerialCommunicationCommandSetsModel
    {
        #region Fields and Properties
        //Contains parameters on the Printer and Print parameters.
        private RealTimeStatusDataModel _realTimeStatusDataModel;
        public RealTimeStatusDataModel RealTimeStatusDataModel
        {
            get { return _realTimeStatusDataModel; }
        }
        private PrinterModel _printerModel;
        private PrintModel _printModel;

        //Contains functions to output commands.
        WriteSetAxisModel _writeSetAxisModel;
        SetWritePrintheadModel _setWritePrintheadModel;

        /// <summary>
        /// Is a command set ready to be interpreted and sent through the serial port?
        /// Command sets were designed to queue tasks (such as setting position values) after other tasks were complete.
        /// Returns true only if other queued tasks were completed.
        /// </summary>
        public bool CanInterpret
        {
            get
            {
                return (_realTimeStatusDataModel.RealTimeStatusMessageListsModel.TaskQueuedMessagesList.Count == 0) ? true : false;
            }
        }

        #endregion

        #region Events
        //Called whenever Position property values are changed via a command set.
        public event CommandSetPositionChangedEventHandler CommandSetPositionChanged;
        private void OnCommandSetPositionChanged()
        {
            if (CommandSetPositionChanged != null)
            { CommandSetPositionChanged(); }
        }

        //Called whenever a Min or Max Position property value is changed via a command set.
        public event CommandSetMinMaxPositionChangedEventHandler CommandSetMinMaxPositionChanged;
        private void OnCommandSetMinMaxPositionChanged()
        {
            if (CommandSetMinMaxPositionChanged != null)
            { CommandSetMinMaxPositionChanged(); }
        }
        #endregion

        #region Constructor
        public SerialCommunicationCommandSetsModel(RealTimeStatusDataModel RealTimeStatusDataModel, PrinterModel PrinterModel, PrintModel PrintModel)
        {
            _realTimeStatusDataModel = RealTimeStatusDataModel;
            _printerModel = PrinterModel;
            _printModel = PrintModel;

            ParametersModel parametersModel = new ParametersModel(_printerModel, null);
            _writeSetAxisModel = new WriteSetAxisModel(parametersModel);
            _setWritePrintheadModel = new SetWritePrintheadModel(parametersModel);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Interpret a command series and return an array of commands
        /// </summary>
        /// <param name="commandSeries"></param>
        /// <returns></returns>
        public string[] InterpretCommandSet(string commandSet)
        {
            //The first character of the command set is reserved for the command set serial character.
            if ((commandSet.Length >= 7)
             && (commandSet.Substring(1, 6) == "Center"))
            {
                return InterpretCenterAxes(commandSet);
            }
            else if ((commandSet.Length >= 7)
                  && (commandSet.Substring(1, 6) == "Origin"))
            {
                return InterpretSetOrigin(commandSet);
            }
            else if ((commandSet.Length >= 13)
                  && (commandSet.Substring(1, 12) == "SetMinMaxPos"))
            {
                return InterpretSetMinMaxPosition(commandSet);
            }
            else if ((commandSet.Length >= 9)
                  && (commandSet.Substring(1, 8) == "RetractZ"))
            {
                return InterpretRetractZ(commandSet); 
            }
            else if ((commandSet.Length >= 6)
                  && (commandSet.Substring(1, 5) == "Pause"))
            {
                return InterpretPause(commandSet);
            }
            else if ((commandSet.Length >= 15)
                  && (commandSet.Substring(1, 14) == "SwitchMaterial"))
            {
                return InterpretSwitchMaterial(commandSet);
            }

            return null;
        }

        /// <summary>
        /// Interpret a center axes command set and return an array of commands.
        /// </summary>
        /// <param name="commandSet"></param>
        /// <returns></returns>
        private string[] InterpretCenterAxes(string commandSet)
        {
            //Command set to be returned.
            string[] returnCommands = new string[1];

            AxisModel xAxisModel = _printerModel.AxisModelList[0];
            AxisModel yAxisModel = _printerModel.AxisModelList[1];
            RealTimeStatusAxisModel xRealTimeStatusAxisModel = _realTimeStatusDataModel.XRealTimeStatusAxisModel;
            RealTimeStatusAxisModel yRealTimeStatusAxisModel = _realTimeStatusDataModel.YRealTimeStatusAxisModel;
            double xNewPosition = xRealTimeStatusAxisModel.Position;
            double yNewPosition = yRealTimeStatusAxisModel.Position;

            //mmPerStep for each actuator.
            double xmmPerStep = 0;
            double ymmPerStep = 0;

            //InvertDirection for each actuator.
            bool xInvertDirection = (xAxisModel.IsDirectionInverted == false) ? false : true;
            bool yInvertDirection = (yAxisModel.IsDirectionInverted == false) ? false : true;

            //Distances from the center.
            double xDistanceFromCenter = 0;
            double yDistanceFromCenter = 0;
            string[] gCodePhrases = GCodeStringParsing.GCodeTo2DArr(commandSet)[0];
            foreach (string phrase in gCodePhrases)
            {
                switch (phrase[0])
                {
                    case 'X':
                        xDistanceFromCenter = GCodeStringParsing.ParseDouble(phrase);
                        break;
                    case 'Y':
                        yDistanceFromCenter = GCodeStringParsing.ParseDouble(phrase);
                        break;
                    default:
                        //Do nothing.
                        break;
                }
            }

            //Centering the actuator involves:
            // 1. Finding the median position directly in the center of the max and min position.
            // 2. Finding the distance between median position and the current position.
            // 3. Executing that difference worth of movement.

            if (commandSet.Contains("X"))
            {
                xNewPosition = (xAxisModel.MaxPosition - xAxisModel.MinPosition) / 2 + xAxisModel.MinPosition + xDistanceFromCenter;
                xmmPerStep = xAxisModel.MmPerStep;
            }

            if (commandSet.Contains("Y"))
            {
                yNewPosition = (yAxisModel.MaxPosition - yAxisModel.MinPosition) / 2 + yAxisModel.MinPosition + yDistanceFromCenter;
                ymmPerStep = yAxisModel.MmPerStep;
            }

            returnCommands[0] = GCodeLinesConverter.GCodeLinesListToString(
                WriteG00.WriteAxesMovement(
                xmmPerStep, ymmPerStep, 0, 
                xNewPosition - xRealTimeStatusAxisModel.Position, yNewPosition - yRealTimeStatusAxisModel.Position, 0,
                xInvertDirection, yInvertDirection, false));

            return returnCommands;
        }

        /// <summary>
        /// Set the new center as the origin (position 0) and adjust the max and min range around such.
        /// </summary>
        /// <param name="commandSet"></param>
        /// <returns></returns>
        private string[] InterpretSetOrigin(string commandSet)
        {
            //Remove "*Origin" from the beginning of the string.
            commandSet = commandSet.Substring(6);

            if (commandSet.Contains("E"))
            {
                if (_realTimeStatusDataModel.ActivePrintheadType == PrintheadType.Motorized)
                {
                    RealTimeStatusMotorizedPrintheadModel realTimeStatusMotorizedPrintheadModel = (RealTimeStatusMotorizedPrintheadModel)_realTimeStatusDataModel.ActivePrintheadModel;
                    MotorizedPrintheadTypeModel motorizedPrintheadTypeModel = (MotorizedPrintheadTypeModel)_printerModel.FindPrinthead(realTimeStatusMotorizedPrintheadModel.Name).PrintheadTypeModel;

                    double previousEPosition = realTimeStatusMotorizedPrintheadModel.Position;

                    motorizedPrintheadTypeModel.MaxPosition -= previousEPosition;
                    motorizedPrintheadTypeModel.MinPosition -= previousEPosition;
                    realTimeStatusMotorizedPrintheadModel.Position = 0;
                }
            }

            if (commandSet.Contains("X"))
            {
                AxisModel xAxisModel = _printerModel.AxisModelList[0];

                double previousXPosition = _realTimeStatusDataModel.XRealTimeStatusAxisModel.Position;

                xAxisModel.MaxPosition -= previousXPosition;
                xAxisModel.MinPosition -= previousXPosition;
                _realTimeStatusDataModel.XRealTimeStatusAxisModel.Position = 0;
            }

            if (commandSet.Contains("Y"))
            {
                AxisModel yAxisModel = _printerModel.AxisModelList[1];

                double previousYPosition = _realTimeStatusDataModel.YRealTimeStatusAxisModel.Position;

                yAxisModel.MaxPosition -= previousYPosition;
                yAxisModel.MinPosition -= previousYPosition;
                _realTimeStatusDataModel.YRealTimeStatusAxisModel.Position = 0;
            }

            if (commandSet.Contains("Z"))
            {
                AxisModel zAxisModel = _printerModel.FindAxis(_realTimeStatusDataModel.ZRealTimeStatusAxisModel.Name);

                double previousZPosition = _realTimeStatusDataModel.ZRealTimeStatusAxisModel.Position;

                zAxisModel.MaxPosition -= previousZPosition;
                zAxisModel.MinPosition -= previousZPosition;
                _realTimeStatusDataModel.ZRealTimeStatusAxisModel.Position = 0;
            }

            OnCommandSetPositionChanged();
            OnCommandSetMinMaxPositionChanged();

            //No commands to return.
            return null;
        }

        /// <summary>
        /// Sets the minimum and/or maximum position of actuator-based equipment as its current position.
        /// </summary>
        /// <param name="commandSet"></param>
        /// <returns></returns>
        private string[] InterpretSetMinMaxPosition(string commandSet)
        {
            //Remove "*SetMinMaxPos" from the beginning of the command set.
            commandSet = commandSet.Substring(12);
            
            for (int index = 0; index < commandSet.Length; index++)
            {
                switch (commandSet[index])
                {
                    case 'E':
                        if (_realTimeStatusDataModel.ActivePrintheadType == PrintheadType.Motorized)
                        {
                            //Set the current position as the parameter value.
                            RealTimeStatusMotorizedPrintheadModel realTimeStatusMotorizedPrintheadModel = (RealTimeStatusMotorizedPrintheadModel)_realTimeStatusDataModel.ActivePrintheadModel;
                            MotorizedPrintheadTypeModel motorizedPrintheadTypeModel = (MotorizedPrintheadTypeModel)_printerModel.FindPrinthead(_realTimeStatusDataModel.ActivePrintheadModel.Name).PrintheadTypeModel;
                            double ePreviousPosition = realTimeStatusMotorizedPrintheadModel.Position;
                            realTimeStatusMotorizedPrintheadModel.Position = ParseDouble(commandSet.Substring(index));
                            double ePositionDifference = realTimeStatusMotorizedPrintheadModel.Position - ePreviousPosition;

                            //Set the Min or Max Position property as the parameter value.
                            //Adjust the Min and Max positions such that the distance between Max and Min Position remains the same.
                            switch (commandSet[index + 1])
                            {
                                case 'N':
                                    motorizedPrintheadTypeModel.MinPosition = realTimeStatusMotorizedPrintheadModel.Position;
                                    motorizedPrintheadTypeModel.MaxPosition += ePositionDifference; 
                                    break;
                                case 'M':
                                    motorizedPrintheadTypeModel.MaxPosition = realTimeStatusMotorizedPrintheadModel.Position;
                                    motorizedPrintheadTypeModel.MinPosition += ePositionDifference;
                                    break;
                            }
                            
                        }   
                        break;
                    case 'X':
                        //Set the current position as the parameter value.
                        RealTimeStatusAxisModel xRealTimeStatusAxisModel = _realTimeStatusDataModel.XRealTimeStatusAxisModel;
                        AxisModel xAxisModel = _printerModel.AxisModelList[0];
                        double xPreviousPosition = xRealTimeStatusAxisModel.Position;
                        xRealTimeStatusAxisModel.Position = ParseDouble(commandSet.Substring(index));
                        double xPositionDifference = xRealTimeStatusAxisModel.Position - xPreviousPosition;

                        //Set the Min or Max Position property as the parameter value.
                        //Adjust the Min and Max positions such that the distance between Max and Min Position remains the same.
                        switch (commandSet[index + 1])
                        {
                            case 'N':
                                xAxisModel.MinPosition = xRealTimeStatusAxisModel.Position;
                                xAxisModel.MaxPosition += xPositionDifference;
                                break;
                            case 'M':
                                xAxisModel.MaxPosition = xRealTimeStatusAxisModel.Position;
                                xAxisModel.MinPosition += xPositionDifference;
                                break;
                        }
                        break;
                    case 'Y':
                        //Set the current position as the parameter value.
                        RealTimeStatusAxisModel yRealTimeStatusAxisModel = _realTimeStatusDataModel.YRealTimeStatusAxisModel;
                        AxisModel yAxisModel = _printerModel.AxisModelList[1];
                        double yPreviousPosition = yRealTimeStatusAxisModel.Position;
                        yRealTimeStatusAxisModel.Position = ParseDouble(commandSet.Substring(index));
                        double yPositionDifference = yRealTimeStatusAxisModel.Position - yPreviousPosition;

                        //Set the Min or Max Position property as the parameter value.
                        //Adjust the Min and Max positions such that the distance between Max and Min Position remains the same.
                        switch (commandSet[index + 1])
                        {
                            case 'N':
                                yAxisModel.MinPosition = yRealTimeStatusAxisModel.Position;
                                yAxisModel.MaxPosition += yPositionDifference;
                                break;
                            case 'M':
                                yAxisModel.MaxPosition = yRealTimeStatusAxisModel.Position;
                                yAxisModel.MinPosition += yPositionDifference;
                                break;
                        }
                        break;
                    case 'Z':
                        //Set the current position as the parameter value.
                        RealTimeStatusAxisModel zRealTimeStatusAxisModel = _realTimeStatusDataModel.ZRealTimeStatusAxisModel;
                        AxisModel zAxisModel = _printerModel.FindAxis(zRealTimeStatusAxisModel.Name);
                        double zPreviousPosition = zRealTimeStatusAxisModel.Position;
                        zRealTimeStatusAxisModel.Position = ParseDouble(commandSet.Substring(index));
                        double zPositionDifference = zRealTimeStatusAxisModel.Position - zPreviousPosition;

                        //Set the Min or Max Position property as the parameter value.
                        //Adjust the Min and Max positions such that the distance between Max and Min Position remains the same.
                        switch (commandSet[index + 1])
                        {
                            case 'N':
                                zAxisModel.MinPosition = zRealTimeStatusAxisModel.Position;
                                zAxisModel.MaxPosition += zPositionDifference;
                                break;
                            case 'M':
                                zAxisModel.MaxPosition = zRealTimeStatusAxisModel.Position;
                                zAxisModel.MinPosition += zPositionDifference;
                                break;
                        }

                        break;
                }
            }

            OnCommandSetMinMaxPositionChanged();

            //No commands to return.
            return null;
        }

        /// <summary>
        /// Retracts the Z Axis up to a short distance away from the Limit Switch (does not hit the Limit Switch).
        /// </summary>
        /// <param name="commandSet"></param>
        /// <returns></returns>
        private string[] InterpretRetractZ(string commandSet)
        {
            //Command set to be returned.
            string[] returnCommands = new string[2];

            //Find the retracting Z Axis.
            AxisModel zAxisModel = _printerModel.FindAxis(_realTimeStatusDataModel.ZRealTimeStatusAxisModel.Name);

            //Generate commands for retracting the Z Axis.
            returnCommands = RetractZ(commandSet, zAxisModel);

            //Return null if failed to find Z Axis.
            return returnCommands;
        }

        /// <summary>
        /// Generate commands for retracting a Z Axis.
        /// </summary>
        /// <param name="zAxisModel"></param>
        /// <returns></returns>
        private string[] RetractZ(string commandSet, AxisModel zAxisModel)
        {
            string[] returnCommands = new string[2];

            if (zAxisModel != null)
            {
                //Retract Z Axis.
                bool zDirectionInverted = (zAxisModel != null) ? zAxisModel.IsDirectionInverted : false;
                if (commandSet.Contains("Limit")) //If CommandSet is "RetractZLimit", then hit the Limit Switch before returning to default position.
                {
                    //Hit the Limti Switch.
                    double retractDistance = 5000;
                    string zRetract = GCodeLinesConverter.GCodeLinesListToString(
                        WriteG00.WriteAxesMovement(
                        1, 1, zAxisModel.MmPerStep,
                        0, 0, retractDistance,
                        false, false, zDirectionInverted));
                    returnCommands[0] = zRetract;

                    //Move away from the Limit Switch to default position.
                    string zMoveAwayFromLimit = GCodeLinesConverter.GCodeLinesListToString(
                        WriteG00.WriteAxesMovement(
                        0, 0, zAxisModel.MmPerStep,
                        0, 0, -1 * GlobalValues.LimitBuffer,
                        false, false, zAxisModel.IsDirectionInverted));
                    returnCommands[1] = zMoveAwayFromLimit;
                }
                else //If the CommandSet is "RetractZ", then move to default position without hitting the Limit Switch.
                {
                    //Move to default position.
                    double retractDistance = zAxisModel.MaxPosition - _realTimeStatusDataModel.ZRealTimeStatusAxisModel.Position - GlobalValues.LimitBuffer;
                    string zRetract = GCodeLinesConverter.GCodeLinesListToString(
                        WriteG00.WriteAxesMovement(
                        1, 1, zAxisModel.MmPerStep,
                        0, 0, retractDistance,
                        false, false, zDirectionInverted));
                    returnCommands[0] = zRetract;
                }
                

                return returnCommands;
            }

            return null;
        }

        /// <summary>
        /// Pauses the thread for a specified amount of time.
        /// </summary>
        /// <param name="commandSet"></param>
        /// <returns></returns>
        private string[] InterpretPause(string commandSet)
        {
            //Remove "*Pause" from the beginning of the command set.
            commandSet = commandSet.Substring(6);

            //Length of time to pause this thread.
            //In milliseconds.
            int pauseTime = (int)ParseDouble(commandSet);

            //Pause the thread.
            Thread.Sleep(pauseTime);

            //No command set to be returned.
            return null;
        }

        /// <summary>
        /// Interpret a switch material command set and return an array of commands.
        /// </summary>
        /// <param name="commandSet"></param>
        /// <returns></returns>
        private string[] InterpretSwitchMaterial(string commandSet)
        {
            //Remove "*SwitchMaterial" from the beginning of the command set.
            commandSet = commandSet.Substring(14);

            //Set of commands to be returned at the end of this method.
            List<string> returnCommands = new List<string>();

            //The name of the Material which will be switched to will be between quote characters.
            int firstNameIndex = commandSet.IndexOf('"');
            int nameLength = commandSet.Substring(firstNameIndex + 1).IndexOf('"');
            string materialName = commandSet.Substring(firstNameIndex + 1, nameLength);
            MaterialModel materialModel = _printModel.FindMaterialByName(materialName);

            //References to the current and new Printheads.
            PrintheadModel currentPrintheadModel = _printerModel.FindPrinthead(_realTimeStatusDataModel.ActivePrintheadModel.Name);
            PrintheadModel newPrintheadModel = materialModel.PrintheadModel;

            //References to the XY Axes and new Z Axis.
            AxisModel currentZAxisModel = _printerModel.FindAxis(currentPrintheadModel.AttachedZAxisModel.Name);
            int currentZLimitPinID = (currentZAxisModel.AttachedLimitSwitchGPIOPinModel != null) ? currentZAxisModel.AttachedLimitSwitchGPIOPinModel.PinID : GlobalValues.NullPinID;
            AxisModel xAxisModel = _printerModel.AxisModelList[0];
            int xLimitPinID = (xAxisModel.AttachedLimitSwitchGPIOPinModel != null) ? xAxisModel.AttachedLimitSwitchGPIOPinModel.PinID : GlobalValues.NullPinID;
            AxisModel yAxisModel = _printerModel.AxisModelList[1];
            int yLimitPinID = (yAxisModel.AttachedLimitSwitchGPIOPinModel != null) ? yAxisModel.AttachedLimitSwitchGPIOPinModel.PinID : GlobalValues.NullPinID;
            AxisModel zAxisModel = _printerModel.FindAxis(newPrintheadModel.AttachedZAxisModel.Name);
            int zLimitPinID = (zAxisModel.AttachedLimitSwitchGPIOPinModel != null) ? zAxisModel.AttachedLimitSwitchGPIOPinModel.PinID : GlobalValues.NullPinID;

            //If a new Printhead is required...
            if (newPrintheadModel.Name != currentPrintheadModel.Name)
            {
                //1. Set previous Z Axis at max speeds.
                //2. Retract the previous Printhead / Z Axis.
                returnCommands.Add(_writeSetAxisModel.WriteSetAxis('Z', currentZAxisModel.AttachedMotorStepGPIOPinModel.PinID, currentZAxisModel.AttachedMotorDirectionGPIOPinModel.PinID,
                    currentZAxisModel.StepPulseTime, currentZLimitPinID, currentZAxisModel.MaxSpeed, currentZAxisModel.MaxAcceleration, currentZAxisModel.MmPerStep));
                string[] retractZ = RetractZ("", currentPrintheadModel.AttachedZAxisModel);
                foreach (string command in retractZ)
                {
                    if (!String.IsNullOrWhiteSpace(command))
                    {
                        returnCommands.Add(command);
                    }
                }

                //3. Set new XYZ to max speeds and move to the new Offset.
                //Set associated X Axis at max speeds.                
                returnCommands.Add(_writeSetAxisModel.WriteSetAxis('X', xAxisModel.AttachedMotorStepGPIOPinModel.PinID, xAxisModel.AttachedMotorDirectionGPIOPinModel.PinID,
                    xAxisModel.StepPulseTime, xLimitPinID, xAxisModel.MaxSpeed, xAxisModel.MaxAcceleration, xAxisModel.MmPerStep));
                //Set associated Y Axis at max speeds.
                returnCommands.Add(_writeSetAxisModel.WriteSetAxis('Y', yAxisModel.AttachedMotorStepGPIOPinModel.PinID, yAxisModel.AttachedMotorDirectionGPIOPinModel.PinID,
                    yAxisModel.StepPulseTime, yLimitPinID, yAxisModel.MaxSpeed, yAxisModel.MaxAcceleration, yAxisModel.MmPerStep));
                //Set associated Z Axis at max speeds.
                returnCommands.Add(_writeSetAxisModel.WriteSetAxis('Z', zAxisModel.AttachedMotorStepGPIOPinModel.PinID, zAxisModel.AttachedMotorDirectionGPIOPinModel.PinID,
                    zAxisModel.StepPulseTime, zLimitPinID, zAxisModel.MaxSpeed, zAxisModel.MaxAcceleration, zAxisModel.MmPerStep));
                //4.Move to the new Offset at max speeds.
                double zPosition = _realTimeStatusDataModel.ZRealTimeStatusAxisModel.Position;
                string[] moveToOffset = WriteMoveToOffset(newPrintheadModel, currentPrintheadModel, zPosition);
                foreach (string command in moveToOffset)
                {
                    if (!String.IsNullOrWhiteSpace(command))
                    {
                        returnCommands.Add(command);
                    }
                }
            }

            //5.Set the print speed parameters for the new Material.
            //Set associated X Axis at print speeds.
            returnCommands.Add(_writeSetAxisModel.WriteSetAxis('X', xAxisModel.AttachedMotorStepGPIOPinModel.PinID, xAxisModel.AttachedMotorDirectionGPIOPinModel.PinID,
                xAxisModel.StepPulseTime, xLimitPinID, materialModel.XPrintSpeed, materialModel.XPrintAcceleration, xAxisModel.MmPerStep));
            //Set associated Y Axis at print speeds.
            returnCommands.Add(_writeSetAxisModel.WriteSetAxis('Y', yAxisModel.AttachedMotorStepGPIOPinModel.PinID, yAxisModel.AttachedMotorDirectionGPIOPinModel.PinID,
                yAxisModel.StepPulseTime, yLimitPinID, materialModel.YPrintSpeed, materialModel.YPrintAcceleration, yAxisModel.MmPerStep));
            //Set associated Z Axis at print speeds.
            returnCommands.Add(_writeSetAxisModel.WriteSetAxis('Z', zAxisModel.AttachedMotorStepGPIOPinModel.PinID, zAxisModel.AttachedMotorDirectionGPIOPinModel.PinID,
                zAxisModel.StepPulseTime, zLimitPinID, materialModel.ZPrintSpeed, materialModel.ZPrintAcceleration, zAxisModel.MmPerStep));
            
            //6. Set the new Printhead at print speeds.
            string setNewPrinthead = _setWritePrintheadModel.SetWritePrinthead(newPrintheadModel);
            returnCommands.Add(setNewPrinthead);

            return returnCommands.ToArray<string>();
        }

        /// <summary>
        /// Returns a set of commands for Axis movement that compensates for a new Printhead's offsets.
        /// Print speeds will be maximized before moving to offset.
        /// </summary>
        /// <param name="newPrinthead"></param>
        /// <param name="currentPrinthead"></param>
        /// <param name="zPosition">Relative position of the Z Axis before and after switching.</param>
        /// <returns></returns>
        private string[] WriteMoveToOffset(PrintheadModel newPrinthead, PrintheadModel currentPrinthead, double zPosition)
        {
            string[] returnCommands = new string[5];
            int returnIndex = 0;

            AxisModel xAxisModel = _printerModel.AxisModelList[0];
            AxisModel yAxisModel = _printerModel.AxisModelList[1];
            AxisModel zAxisModel = newPrinthead.AttachedZAxisModel;

            double xMove = newPrinthead.XOffset - currentPrinthead.XOffset;
            double yMove = newPrinthead.YOffset - currentPrinthead.YOffset;
            double zMove = -1 * (newPrinthead.AttachedZAxisModel.MaxPosition - GlobalValues.LimitBuffer) + zPosition;

            //Move to the X and Y offsets first to prevent bumping of print container walls.
            if ((xMove != 0) || (yMove != 0))
            {
                //Maximize movement speeds before moving to offset.
                returnCommands[returnIndex++] = _writeSetAxisModel.WriteSetAxis(_printerModel.AxisModelList[0]) + "\r\n";
                returnCommands[returnIndex++] = _writeSetAxisModel.WriteSetAxis(_printerModel.AxisModelList[1]) + "\r\n";
                returnCommands[returnIndex++] = GCodeLinesConverter.GCodeLinesListToString(WriteG00.WriteAxesMovement(xAxisModel.MmPerStep, yAxisModel.MmPerStep, 0,
                    xMove, yMove, 0, xAxisModel.IsDirectionInverted, yAxisModel.IsDirectionInverted, false)) + "\r\n";
            }

            //Move the Z offset after the X and Y positions are set.
            if (zMove != 0)
            {
                returnCommands[returnIndex++] = _writeSetAxisModel.WriteSetAxis(newPrinthead.AttachedZAxisModel) + "\r\n";
                returnCommands[returnIndex++] = GCodeLinesConverter.GCodeLinesListToString(WriteG00.WriteAxesMovement(0, 0, zAxisModel.MmPerStep,
                    0, 0, zMove, false, false, zAxisModel.IsDirectionInverted)) + "\r\n";
            }

            return returnCommands;
        }

        /// <summary>
        /// Takes a string and returns only the first number within the string.
        /// Supports negative/positive, integer/decimal numbers.
        /// </summary>
        private double ParseDouble(string phrase)
        {
            return double.Parse(Regex.Match(phrase, @"-?\d+(\.\d+)?$").Value);
        }
        #endregion
    }
}
