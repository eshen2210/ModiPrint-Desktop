using System;
using System.Windows;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModiPrint.DataTypes.GlobalValues;
using ModiPrint.Models.PrinterModels;
using ModiPrint.Models.PrintModels;
using ModiPrint.Models.PrintModels.MaterialModels;
using ModiPrint.Models.PrinterModels.PrintheadModels;
using ModiPrint.Models.PrinterModels.AxisModels;
using ModiPrint.Models.RealTimeStatusModels;
using ModiPrint.Models.GCodeConverterModels.ProcessModels.ProcessG00Models;
using ModiPrint.Models.GCodeConverterModels.ProcessModels.ProcessTCommandModels;

namespace ModiPrint.Models.GCodeConverterModels.ProcessModels
{
    public class ProcessGCodeModel
    {
        #region Fields and Properties
        //The GCode Converter will take Print parameters from these objects.
        private PrinterModel _printerModel;
        private PrintModel _printModel;
        private RealTimeStatusDataModel _realTimeStatusDataModel;

        //Classes associated with GCodeConverter-specific functions.
        private ProcessG00CommandModel _processG00CommandModel;
        private ProcessTCommandModel _processTCommandModel;

        //Class containing pseudo-global parameters for the GCodeConverter that are passed around various GCodeConverter classes.
        private ParametersModel _parametersModel;

        //The GCode Converter is currently processing for which Material?
        //The GCode Converter will read parameters from this Material.
        private MaterialModel _currentMaterial;
        #endregion

        #region Constructor
        public ProcessGCodeModel(PrinterModel PrinterModel, PrintModel PrintModel, RealTimeStatusDataModel RealTimeStatusDataModel, ParametersModel ParametersModel)
        {
            //Create a dummy Material with default parameters to be passed around until a user-set Material is in place.
            _currentMaterial = new MaterialModel("Unset", _printerModel);
            _currentMaterial.PrintheadModel = new PrintheadModel("Unset");
            _currentMaterial.PrintheadModel.AttachedZAxisModel = new AxisModel("Unset", 'I');

            _printerModel = PrinterModel;
            _printModel = PrintModel;
            _realTimeStatusDataModel = RealTimeStatusDataModel;
            _parametersModel = ParametersModel;

            _processG00CommandModel = new ProcessG00CommandModel(_printerModel, _parametersModel);
            _processTCommandModel = new ProcessTCommandModel(_printModel, _printerModel, RealTimeStatusDataModel, _parametersModel);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Sets processGCodeCommand to a ProcessGCodeCommand method.
        /// Executes processGCodeCommand and returns the resulting string.
        /// </summary>
        public List<ConvertedGCodeLine> SetProcessGCodeCommand(string[] slic3rLine)
        {
            //Return null if the input parameter is invalid.
            if ((slic3rLine.Length == 0) || (String.IsNullOrWhiteSpace(slic3rLine[0])))
            { return null; }
            
            //The return GCode.
            List<ConvertedGCodeLine> convertedGCodeLinesList = null;

            if ((slic3rLine[0] == "G00") //Axes movement
             || (slic3rLine[0] == "G0"))
            {
                convertedGCodeLinesList = _processG00CommandModel.ProcessG00Command(slic3rLine, _currentMaterial, false);
            }
            else if (slic3rLine[0] == "G01" //Printing with movement.
             || slic3rLine[0] == "G1")
            {
                convertedGCodeLinesList = _processG00CommandModel.ProcessG00Command(slic3rLine, _currentMaterial, true);
            }
            else if (slic3rLine[0][0] == 'T') //Switch printheads.
            {
                convertedGCodeLinesList = _processTCommandModel.ProcessTCommand(slic3rLine, ref _currentMaterial);
            }
            else if (slic3rLine[0] == "G90") //Absolute coordinates for Axes.
            {
                _parametersModel.AbsCoordAxis = true;
            }
            else if (slic3rLine[0] == "G91") //Relative coordinates for Axes.
            {
                _parametersModel.AbsCoordAxis = false;
            }
            else if (slic3rLine[0] == "M82") //Absolute coordinates for Motor Printheads.
            {
                _parametersModel.AbsCoordExtruder = true;
            }
            else if (slic3rLine[0] == "M83") //Relative coordinates for Motor Printheads.
            {
                _parametersModel.AbsCoordExtruder = false;
            }
            //ModiPrint will ignore these commands.
            else if (String.IsNullOrWhiteSpace(slic3rLine[0])
                || (slic3rLine[0][0] == SerialMessageCharacters.SerialTerminalCharacter) //Comments from Slic3r.
                || (slic3rLine[0] == "M104") //The command to set printer temperature. This is unneeded.
                || (slic3rLine[0] == "M106") //The command to turn the fan on. ModiPrint printers do not have fans by default.
                || (slic3rLine[0] == "M107") //The command to turn the fan off. ModiPrint printers do not have fans by default.
                || (slic3rLine[0] == "M109") //The command to set extruder temperature. ModiPrint printers do not have fans by default.
                || (slic3rLine[0] == "G04") //The command to pause. ModiPrint has its own built in pausing features.
                || (slic3rLine[0] == "G21") //The command to set units to milimeters. ModiPrint only operates in milimeters so this is irrelevant.
                || (slic3rLine[0] == "G28") //The command to home to the origin. ModiPrint keeps track of positioning, not the microcontroller.
                || (slic3rLine[0] == "G92") //The command to program the zero point. This is unneeded as ModiPrint will control this process.
                )
            {
                return null;
            }
            //The command to set units to inches. ModiPrint operates in milimeters. The user should set Slic3r to output milimeters only.
            else if (slic3rLine[0] == "G20") 
            {
                string slic3rLineStr = GCodeStringParsing.GCodeLineArrToStr(slic3rLine);
                _parametersModel.ErrorReporterViewModel.ReportError("GCode Converter: Inches Not Supported, set Slic3r param to milimeters", slic3rLineStr);
                return null;
            }
            //ModiPrint will throw errors for unrecognized commands.
            else
            {
                string slic3rLineStr = GCodeStringParsing.GCodeLineArrToStr(slic3rLine);
                _parametersModel.ErrorReporterViewModel.ReportError("GCode Converter: Unrecognized Command", slic3rLineStr);
                return null;
            }

            return convertedGCodeLinesList;
        }
        #endregion
    }
}
