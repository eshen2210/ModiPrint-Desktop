﻿using System;
using System.Windows;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModiPrint.Models.PrinterModels;
using ModiPrint.Models.PrinterModels.PrintheadModels;
using ModiPrint.Models.PrinterModels.PrintheadModels.PrintheadTypeModels;
using ModiPrint.Models.PrintModels.MaterialModels;
using ModiPrint.Models.PrintModels.PrintStyleModels;

namespace ModiPrint.Models.GCodeConverterModels.ProcessModels.ProcessG00Models
{
    /// <summary>
    /// Processes the G00 command for movement and printing.
    /// </summary>
    public class ProcessG00CommandModel
    {
        #region Fields and Properties
        //Contains parameters for the printer.
        private PrinterModel _printerModel;

        //Contains pseudo-global parameters for the GCodeConverter that are passed around various GCodeConverter classes.
        private ParametersModel _parametersModel;
        #endregion

        #region Constructor
        public ProcessG00CommandModel(PrinterModel PrinterModel, ParametersModel ParametersModel)
        {
            _printerModel = PrinterModel;
            _parametersModel = ParametersModel;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Processes the command for Axis movement and Printing.
        /// </summary>
        public List<ConvertedGCodeLine> ProcessG00Command(string[] repRapGCodeLine, MaterialModel currentMaterial, bool isPrinting)
        {
            //The return GCode.
            List<ConvertedGCodeLine> convertedGCodeLinesList = null;

            //Reads the GCode line and sets the fields in the coord classes.
            //If the printer is moving or printing, then proceed to convert the GCode line.
            if ((ReadCoord(repRapGCodeLine) == true)
             && ((_parametersModel.XCoord.Changed == true)
              || (_parametersModel.YCoord.Changed == true)
              || (_parametersModel.ZCoord.Changed == true)))
            {
                //Does this RepRap line indicate printing?
                _parametersModel.IsPrinting = ((isPrinting) || (_parametersModel.ERepRapCoord.PositiveChanged == true)) ? true : false; //To Do: Test is E retraction causes IsPrinting to be false.

                try
                {
                    int eModiPrintCoordIndex = _parametersModel.FindEModiPrintCoordIndex(currentMaterial.PrintheadModel);
                    convertedGCodeLinesList = SetWriteG00Command(
                        currentMaterial, _printerModel, _parametersModel.XCoord, _parametersModel.YCoord, _parametersModel.ZCoord, _parametersModel.EModiPrintCoordList[eModiPrintCoordIndex]);
                }
                catch when (currentMaterial == null) //Material unset.
                {
                    //Catching and error handling should have happened earlier.
                    convertedGCodeLinesList = null;
                }
                catch when (currentMaterial.PrintheadModel == null)
                {
                    _parametersModel.ErrorReporterViewModel.ReportError("GCodeConverter: Printer Unset", currentMaterial.Name + " Printhead Unset");
                    convertedGCodeLinesList = null;
                }
                catch when (currentMaterial.PrintheadModel.AttachedZAxisModel == null)
                {
                    _parametersModel.ErrorReporterViewModel.ReportError("GCodeConverter: Printer Unset", currentMaterial.PrintheadModel.Name + "Z Axis Unset");
                    convertedGCodeLinesList = null;
                }
                catch
                {
                    //Should never reach this point.
                    _parametersModel.ErrorReporterViewModel.ReportError("GCodeConverter", "Unspecified Error, Please Check GCode");
                }
            }

            //Prepares coordinate values for the next line of GCode processing.
            _parametersModel.XCoord.ResolveCoord();
            _parametersModel.YCoord.ResolveCoord();
            _parametersModel.ZCoord.ResolveCoord();
            _parametersModel.ERepRapCoord.ResolveCoord();
            foreach(CoordinateModel eModiPrintCoord in _parametersModel.EModiPrintCoordList)
            { eModiPrintCoord.ResolveCoord(); }

            return convertedGCodeLinesList;
        }

        /// <summary>
        /// Reads the coordinate values in a movement or print command.
        /// Calls SetCoord to set the coordinates that are read.
        /// </summary>
        private bool ReadCoord(string[] gCodeLine)
        {
            for (int phrase = 1; phrase < gCodeLine.Length; phrase++)
            {
                switch (gCodeLine[phrase][0])
                {
                    case 'X':
                        _parametersModel.XCoord.SetCoord(GCodeStringParsing.ParseDouble(gCodeLine[phrase]), _parametersModel.AbsCoordAxis);
                        break;
                    case 'Y':
                        _parametersModel.YCoord.SetCoord(GCodeStringParsing.ParseDouble(gCodeLine[phrase]), _parametersModel.AbsCoordAxis);
                        break;
                    case 'Z':
                        _parametersModel.ZCoord.SetCoord(GCodeStringParsing.ParseDouble(gCodeLine[phrase]), _parametersModel.AbsCoordAxis);
                        break;
                    case 'E':
                        _parametersModel.ERepRapCoord.SetCoord(GCodeStringParsing.ParseDouble(gCodeLine[phrase]), _parametersModel.AbsCoordExtruder);
                        break;
                    case 'F':
                        //Irrelevant RepRap command. Do nothing.
                        break;
                    default:
                        string gCodeLineStr = GCodeStringParsing.GCodeLineArrToStr(gCodeLine);
                        string movementCommand = (_parametersModel.IsPrinting ? "G01" : "G00"); //Not using global values for these because they are RepRap commands.
                        _parametersModel.ErrorReporterViewModel.ReportError("GCodeConverter: Unrecognized " + movementCommand + " Command", "SL:" + _parametersModel.RepRapLine + " " + gCodeLineStr);
                        return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Delegates the appropriate Write method for converting G00 GCode.
        /// Returns the resulting string.
        /// </summary>
        /// <param name="currentMaterial"></param>
        /// <returns></returns>
        public List<ConvertedGCodeLine> SetWriteG00Command(
            MaterialModel currentMaterial, PrinterModel printerModel,
            CoordinateModel xCoord, CoordinateModel yCoord, CoordinateModel zCoord, CoordinateModel currentEModiPrintCoord)
        {
            //The return GCode.
            List<ConvertedGCodeLine> convertedGCodeLinesList = null;

            try
            {
                if (_parametersModel.IsPrinting == false) //If G00 (no printing, movement only)...
                {
                    //If droplet printing, reset droplet print parameters since printing is no longer continuous.
                    double[] remainingDropletMovementArr = { 0, 0, 0 };
                    if (currentMaterial.PrintStyle == PrintStyle.Droplet)
                    {
                        remainingDropletMovementArr = _parametersModel.ResetDropletPrintParameters(currentMaterial, null);
                    }
                    
                    //Append the remaining droplet print movement to the new G00 movement.
                    convertedGCodeLinesList = WriteG00.WriteAxesMovement(
                        printerModel.AxisModelList[0].MmPerStep, printerModel.AxisModelList[1].MmPerStep, currentMaterial.PrintheadModel.AttachedZAxisModel.MmPerStep,
                        xCoord.CurrentCoord - xCoord.PreviousCoord + remainingDropletMovementArr[0], 
                        yCoord.CurrentCoord - yCoord.PreviousCoord + remainingDropletMovementArr[1],
                        zCoord.CurrentCoord - zCoord.PreviousCoord + remainingDropletMovementArr[2],
                        printerModel.AxisModelList[0].IsDirectionInverted, printerModel.AxisModelList[1].IsDirectionInverted, currentMaterial.PrintheadModel.AttachedZAxisModel.IsDirectionInverted);
                }
                else if (_parametersModel.IsPrinting == true) //If G01 (printing is occurring), then select the appropriate printhead and print style...
                {
                    switch (currentMaterial.PrintheadModel.PrintheadType)
                    {
                        case PrintheadType.Motorized:
                            MotorizedPrintheadTypeModel motorizedPrintheadTypeModel = (MotorizedPrintheadTypeModel)currentMaterial.PrintheadModel.PrintheadTypeModel;

                            switch (currentMaterial.PrintStyle)
                            {
                                case PrintStyle.Continuous:
                                    ContinuousPrintStyleModel continousPrintStyleModel = (ContinuousPrintStyleModel)currentMaterial.PrintStyleModel;
                                    convertedGCodeLinesList = WriteG00.WriteMotorizedContinuousPrint(
                                        motorizedPrintheadTypeModel.MmPerStep, printerModel.AxisModelList[0].MmPerStep, printerModel.AxisModelList[1].MmPerStep, currentMaterial.PrintheadModel.AttachedZAxisModel.MmPerStep,
                                        continousPrintStyleModel.MotorizedDispenseDistancePermm,
                                        xCoord.CurrentCoord - xCoord.PreviousCoord, yCoord.CurrentCoord - yCoord.PreviousCoord, zCoord.CurrentCoord - zCoord.PreviousCoord,
                                        printerModel.AxisModelList[0].IsDirectionInverted, printerModel.AxisModelList[1].IsDirectionInverted, currentMaterial.PrintheadModel.AttachedZAxisModel.IsDirectionInverted, motorizedPrintheadTypeModel.IsDirectionInverted,
                                        currentEModiPrintCoord);
                                    break;

                                case PrintStyle.Droplet:
                                    DropletPrintStyleModel dropletPrintStyleModel = (DropletPrintStyleModel)currentMaterial.PrintStyleModel;
                                    convertedGCodeLinesList = WriteG00.WriteMotorizedDropletPrint(
                                        motorizedPrintheadTypeModel.MmPerStep, printerModel.AxisModelList[0].MmPerStep, printerModel.AxisModelList[1].MmPerStep, currentMaterial.PrintheadModel.AttachedZAxisModel.MmPerStep,
                                        dropletPrintStyleModel.MotorizedDispenseDistance,
                                        xCoord.PreviousCoord, xCoord.CurrentCoord, yCoord.PreviousCoord, yCoord.CurrentCoord, zCoord.PreviousCoord, zCoord.CurrentCoord,
                                        printerModel.AxisModelList[0].IsDirectionInverted, printerModel.AxisModelList[1].IsDirectionInverted, currentMaterial.PrintheadModel.AttachedZAxisModel.IsDirectionInverted, motorizedPrintheadTypeModel.IsDirectionInverted,
                                        currentEModiPrintCoord, _parametersModel.DropletModel);
                                    break;

                                case PrintStyle.Unset:
                                    //Try-catch should catch this.
                                    break;
                            }
                            break;
                        case PrintheadType.Valve:
                            switch (currentMaterial.PrintStyle)
                            {
                                case PrintStyle.Continuous:
                                    ContinuousPrintStyleModel continousPrintStyleModel = (ContinuousPrintStyleModel)currentMaterial.PrintStyleModel;
                                    convertedGCodeLinesList = WriteG00.WriteValveContinuousPrint(
                                        printerModel.AxisModelList[0].MmPerStep, printerModel.AxisModelList[1].MmPerStep, currentMaterial.PrintheadModel.AttachedZAxisModel.MmPerStep,
                                        xCoord.PreviousCoord, xCoord.CurrentCoord, yCoord.PreviousCoord, yCoord.CurrentCoord, zCoord.PreviousCoord, zCoord.CurrentCoord,
                                        printerModel.AxisModelList[0].IsDirectionInverted, printerModel.AxisModelList[1].IsDirectionInverted, currentMaterial.PrintheadModel.AttachedZAxisModel.IsDirectionInverted);
                                    break;

                                case PrintStyle.Droplet:
                                    DropletPrintStyleModel dropletPrintStyleModel = (DropletPrintStyleModel)currentMaterial.PrintStyleModel;
                                    convertedGCodeLinesList = WriteG00.WriteValveDropletPrint(
                                        printerModel.AxisModelList[0].MmPerStep, printerModel.AxisModelList[1].MmPerStep, currentMaterial.PrintheadModel.AttachedZAxisModel.MmPerStep,
                                        dropletPrintStyleModel.ValveOpenTime,
                                        xCoord.PreviousCoord, xCoord.CurrentCoord, yCoord.PreviousCoord, yCoord.CurrentCoord, zCoord.PreviousCoord, zCoord.CurrentCoord,
                                        printerModel.AxisModelList[0].IsDirectionInverted, printerModel.AxisModelList[1].IsDirectionInverted, currentMaterial.PrintheadModel.AttachedZAxisModel.IsDirectionInverted,
                                        _parametersModel.DropletModel);
                                    break;

                                case PrintStyle.Unset:
                                    //Try-catch should catch this.
                                    break;
                            }
                            break;
                        case PrintheadType.Custom:
                            //To Do
                            break;
                        case PrintheadType.Unset:
                            //To Do: Throw errors
                            break;
                    }
                }
            }
            catch when (currentMaterial == null) //Material unset.
            {
                //Catching and error handling should have happened before this.
                convertedGCodeLinesList = null;
            }
            catch when ((printerModel == null) //Various things unset.
                     || (xCoord == null)
                     || (yCoord == null)
                     || (zCoord == null)
                     || (currentEModiPrintCoord == null))
            {
                //This should never happen.
                convertedGCodeLinesList = null;
            }
            catch when (currentMaterial.PrintStyle == PrintStyle.Unset) //Print Style unset.
            {
                _parametersModel.ErrorReporterViewModel.ReportError("GCode Converter: Material Unset", currentMaterial.Name + " Print Style Unset");
                convertedGCodeLinesList = null;
            }
            catch when (currentMaterial.PrintheadModel.PrintheadType == PrintheadType.Unset) //Printhead Type unset.
            {
                _parametersModel.ErrorReporterViewModel.ReportError("GCode Converter: Printer Unset", currentMaterial.PrintheadModel.Name + " Printhead Type Unset");
                convertedGCodeLinesList = null;
            }
            catch
            {
                //Should never reach this point.
                _parametersModel.ErrorReporterViewModel.ReportError("GCodeConverter", "Unspecified Error, Please Check Code");
                convertedGCodeLinesList = null;
            }

            return convertedGCodeLinesList;
        }
        #endregion
    }
}
