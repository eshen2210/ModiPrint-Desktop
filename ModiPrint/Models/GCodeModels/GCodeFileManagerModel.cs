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
        //Contains functions to display errors to the GUI.
        ErrorListViewModel _errorListViewModel;
        #endregion

        #region Constructor
        public GCodeFileManagerModel(ErrorListViewModel ErrorListViewModel)
        {
            _errorListViewModel = ErrorListViewModel;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Loads a .gcode file into the GCode properties with OpenFileDialog.
        /// </summary>
        /// <param name="gCode"></param>
        /// <param name="fileName"></param>
        public void UploadGCodeFile(GCodeModel gCodeModel, string fileFilter, ref string fileName)
        {
            string gCode = gCodeModel.GCodeStr;
            try
            {
                //Open file.
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = fileFilter;

                //Reads file.
                if (openFileDialog.ShowDialog() == true)
                {
                    gCode = File.ReadAllText(openFileDialog.FileName);
                    fileName = Path.GetFileName(openFileDialog.FileName);
                    gCodeModel.SetGCode(gCode);
                }
            }
            catch
            {
                _errorListViewModel.AddError("GCode File Manager", "Unable to Upload GCode File");
            }
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
