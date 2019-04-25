using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Win32;
using ModiPrint.ViewModels;

namespace ModiPrint.Models.GCodeModels
{
    public class GCodeFileManagerModel
    {
        #region Fields and Properties
        //References to the two GCode classes that this will interact with.
        GCodeModel _repRapGCodeModel;
        GCodeModel _modiPrintGCodeModel;

        //Contains functions to display errors to the GUI.
        ErrorListViewModel _errorListViewModel;
        #endregion

        #region Constructor
        public GCodeFileManagerModel(GCodeModel RepRapGCodeModel, GCodeModel ModiPrintGCodeModel, ErrorListViewModel ErrorListViewModel)
        {
            _repRapGCodeModel = RepRapGCodeModel;
            _modiPrintGCodeModel = ModiPrintGCodeModel;
            _errorListViewModel = ErrorListViewModel;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Loads a .gcode file into the program with OpenFileDialog.
        /// Determines which GCode file to load into depending on the file extension.
        /// Returns the name of the file.
        /// </summary>
        public string UploadGCodeFile()
        {
            try
            {
                //Open file.
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "GCode Files (.gcode)|*.gcode;|  ModiPrint GCode Files (.mdpt)|*.mdpt";

                //Reads file.
                if (openFileDialog.ShowDialog() == true)
                {
                    string extenstion = Path.GetExtension(openFileDialog.FileName);
                    if (extenstion == ".gcode")
                    {
                        _repRapGCodeModel.GCodeStr = File.ReadAllText(openFileDialog.FileName);
                    }
                    else if (extenstion == ".mdpt")
                    {
                        _modiPrintGCodeModel.GCodeStr = File.ReadAllText(openFileDialog.FileName);
                    }
                    else
                    {
                        _errorListViewModel.AddError("GCode File Manager", "Unrecognized Extension");
                    }

                    return Path.GetFileName(openFileDialog.FileName);
                }
            }
            catch
            {
                _errorListViewModel.AddError("GCode File Manager", "Unable to Upload GCode File");
            }

            return "";
        }

        /// <summary>
        /// Saves the GCode into a file with SaveFileDialog.
        /// </summary>
        /// <param name="gCodeModel"></param>
        /// <param name="fileName"></param>
        /// <param name="newFileName"></param>
        public void SaveGCodeFile(GCodeModel gCodeModel, string fileType, string fileFilter, ref string fileName)
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.AddExtension = true;
                saveFileDialog.DefaultExt = fileType;
                saveFileDialog.Filter = fileFilter;

                if (saveFileDialog.ShowDialog() == true)
                {
                    fileName = saveFileDialog.FileName;
                    File.WriteAllText(fileName, gCodeModel.GCodeStr);
                }
            }
            catch
            {
                _errorListViewModel.AddError("GCode File Manager", "Unable to Save GCode File");
            }
        }
        #endregion
    }
}
