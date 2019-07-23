using System;
using System.Windows;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using ModiPrint.DataTypes.GlobalValues;
using ModiPrint.ViewModels;
using ModiPrint.Models.PrinterModels;
using ModiPrint.Models.PrinterModels.AxisModels;
using ModiPrint.Models.PrintModels;
using ModiPrint.Models.RealTimeStatusModels;
using ModiPrint.Models.GCodeConverterModels.ProcessModels;
using ModiPrint.Models.GCodeConverterModels.ProcessModels.WriteSetEquipmentModels;
using ModiPrint.Models.GCodeConverterModels.CorneringModels;

namespace ModiPrint.Models.GCodeConverterModels
{
    /// <summary>
    /// The master class for the GCodeConverter application logic.
    /// </summary>
    public class GCodeConverterMainModel
    {
        //To Do: Double check if this converter has any rounding issues when tranlating coordinates to steps.
        
        #region Fields and Properties
        //The GCode Converter will take Print parameters from these objects.
        private PrinterModel _printerModel;
        private PrintModel _printModel;
        private RealTimeStatusDataModel _realTimeStatusDataModel;

        //Classes that output converted GCode.
        private ProcessGCodeModel _processGCodeModel;
        private WriteSetAxisModel _writeSetAxisModel;
        private CorneringModel _corneringModel;

        //Class associated with reporting GCodeConverter information to the GUI.
        private ErrorListViewModel _errorListViewModel;

        //Class containing pseudo-global parameters for the GCodeConverter that are passed around various GCodeConverter classes.
        private ParametersModel _parametersModel;
        public ParametersModel ParametersModel
        {
            get { return _parametersModel; }
        }
        #endregion

        #region Constructors
        public GCodeConverterMainModel(PrinterModel PrinterModel, PrintModel PrintModel, RealTimeStatusDataModel RealTimeStatusDataModel, ErrorListViewModel ErrorListViewModel)
        {
            _printerModel = PrinterModel;
            _printModel = PrintModel;
            _realTimeStatusDataModel = RealTimeStatusDataModel;
            _errorListViewModel = ErrorListViewModel;
            _parametersModel = new ParametersModel(_printerModel, _errorListViewModel);

            _processGCodeModel = new ProcessGCodeModel(_printerModel, _printModel, _realTimeStatusDataModel, _parametersModel);
            _writeSetAxisModel = new WriteSetAxisModel(_parametersModel);
            _corneringModel = new CorneringModel(_printerModel, _printModel, _parametersModel);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Converts RepRapGCode into ModiPrintGCode.
        /// </summary>
        public string ConvertGCode(string repRapGCodeInput)
        {
            //The input string is split into a 2D array, delimited first by linebreaks and then by whitespaces.
            string[][] repRapGCode = GCodeStringParsing.GCodeTo2DArr(repRapGCodeInput);

            //The return string which is the converted GCode.
            //Each line of the GCode is an index in the string.
            //Each line will be delimited by \r\n when converting to a string.
            List<ConvertedGCodeLine> convertedGCode = new List<ConvertedGCodeLine>();

            //Converted GCode commands for starting up the printer.
            convertedGCode.Add(new ConvertedGCodeLine("", "Setup"));

            //Set the current X and Y positions as the origin.
            convertedGCode.Add(new ConvertedGCodeLine(SerialMessageCharacters.SerialCommandSetCharacter + "OriginXY"));

            //Iterates through each line of RepRap's GCode and converts it to ModiPrint's flavor of GCode.
            convertedGCode.Add(new ConvertedGCodeLine("", "Print Start"));
            for (_parametersModel.RepRapLine = 0; (_parametersModel.RepRapLine < repRapGCode.Length) && (repRapGCode != null); _parametersModel.RepRapLine++)
            {
                if (repRapGCode[_parametersModel.RepRapLine] != null
                && !String.IsNullOrWhiteSpace(repRapGCode[_parametersModel.RepRapLine][0]))
                {
                    //Processes the single line of GCode and returns a converted string.
                    List<ConvertedGCodeLine> appendModiPrintGCode;
                    appendModiPrintGCode = _processGCodeModel.SetProcessGCodeCommand(RemoveGCodeComments(repRapGCode[_parametersModel.RepRapLine]));

                    //Adds a converted GCode line to the return string with line breaks and comments.
                    if ((appendModiPrintGCode != null) && (appendModiPrintGCode.Count != 0))
                    {
                        //Comment the repRap line at the end of the converted GCode line.
                        appendModiPrintGCode[appendModiPrintGCode.Count - 1].Comment += (_parametersModel.RepRapLine + 1); //(repRapLine + 1) because repRapLine is index 0 where line count is index 1.
                        convertedGCode.AddRange(appendModiPrintGCode);
                    }
                }
                int percentCompleted = ((_parametersModel.RepRapLine + 1) * 100) / repRapGCode.Length;
                _parametersModel.ReportProgress("Converting GCode", percentCompleted);
            }

            //Calculates the deceleration steps parameter in G00 commands.
            _corneringModel.AddDecelerationStepsParameter(ref convertedGCode);

            //Retract this Z Axis.
            string retractZ = SerialMessageCharacters.SerialCommandSetCharacter + "RetractZ";
            convertedGCode.Add(new ConvertedGCodeLine(retractZ));

            _parametersModel.ReportProgress("GCode Converted!", 100);

            return GCodeLinesConverter.GCodeLinesListToString(convertedGCode);
        }

        /// <summary>
        /// Remove ';' and all characters following.
        /// </summary>
        /// <param name="gCodeLine"></param>
        private string[] RemoveGCodeComments(string[] gCodeLine)
        {
            List<string> phrasesList = new List<string>();
            
            //Count the number of phrases that are not comments.
            for (int i = 0; i < gCodeLine.Length; i++)
            {
                if (gCodeLine[i].Contains(';'))
                {
                    gCodeLine[i] = gCodeLine[i].Substring(0, gCodeLine[i].IndexOf(';'));
                    if (!String.IsNullOrWhiteSpace(gCodeLine[i]))
                    {
                        phrasesList.Add(gCodeLine[i]);
                    }
                    break;
                }

                if (!String.IsNullOrWhiteSpace(gCodeLine[i]))
                {
                    phrasesList.Add(gCodeLine[i]);
                }
            }

            //Instantiate, populate, and return a GCode line with no comments.
            string[] uncommentedGCodeLine = new string[phrasesList.Count];
            for (int i = 0; i < uncommentedGCodeLine.Length; i++)
            {
                uncommentedGCodeLine[i] = gCodeLine[i];
            }

            return uncommentedGCodeLine;
        }
        #endregion
    }
}
