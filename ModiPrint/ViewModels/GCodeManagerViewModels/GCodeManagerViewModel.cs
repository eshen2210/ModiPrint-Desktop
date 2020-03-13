using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ModiPrint.Models.GCodeModels;
using ModiPrint.Models.GCodeConverterModels;
using ModiPrint.Models.PrintModels;

namespace ModiPrint.ViewModels.GCodeManagerViewModels
{
    //Handles event that is fired when g-code file is uploaded.
    public delegate void GCodeFileUploadedEventHandler(object sender);
    
    /// <summary>
    /// To Do: This should not handle application logic such as converting GCode
    /// </summary>
    public class GCodeManagerViewModel : ViewModel
    {
        #region Fields and Properties
        //GCode and utilities related to the conversion of GCode.
        private GCodeFileManagerModel _gCodeFileManagerModel;
        public GCodeModel UploadedGCodeModel
        {
            get { return _gCodeFileManagerModel.UploadedGCodeModel; }
        }
        public GCodeType UploadedGCodeType
        {
            get { return _gCodeFileManagerModel.UploadedGCodeType; }
            set
            {
                _gCodeFileManagerModel.UploadedGCodeType = value;
                OnPropertyChanged("UploadedGCodeType");
            }
        }
        private GCodeConverterModel _gCodeConverterModel;


        //GCode file name of the most recently uploaded or uploaded GCode file.
        private string _gCodeFileName = "";
        public string GCodeFileName
        {
            get { return _gCodeFileName; }
            set
            {
                _gCodeFileName = value;
                OnGCodeFileUploaded();
                OnPropertyChanged("GCodeFileName");
            }
        }

        //Status of the g-code converter such as % progress to conversion completion.
        private string _gCodeConverterStatus = "";
        public string GCodeConverterStatus
        {
            get { return _gCodeConverterStatus; }
            set
            {
                _gCodeConverterStatus = value;
                OnPropertyChanged("GCodeConverterStatus");
            }
        }

        //Event that is fired when a g-code file is uploaded.
        public event GCodeFileUploadedEventHandler GCodeFileUploaded;
        public void OnGCodeFileUploaded()
        {
            if (GCodeFileUploaded != null)
            { GCodeFileUploaded(this); }
        }

        //List of all T commands in the uploaded g-code file.
        //Used to populate possible option for the RepRapID parameter in Materials.
        private List<string> _repRapIDList;
        public List<string> RepRapIDList
        {
            get { return _repRapIDList; }
        }
        #endregion

        #region Contructor
        public GCodeManagerViewModel(GCodeFileManagerModel GCodeFileManagerModel, GCodeConverterModel GCodeConverterModel)
        {
            _gCodeFileManagerModel = GCodeFileManagerModel;
            _gCodeConverterModel = GCodeConverterModel;

            _gCodeConverterModel.ParametersModel.LineConverted += new GCodeConverterLineConvertedEventHandler(UpdateGCodeConverterStatus);
        }
        #endregion

        #region Methods
        /// <summary>
        /// TUpdates the g-code converter status.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="progress"></param>
        private void UpdateGCodeConverterStatus(object sender, string progress)
        {
            GCodeConverterStatus = progress;
        }
        
        /// <summary>
        /// Return RepRap flavor g-code from the uploaded g-code file.
        /// Will return an empty string if the uploaded g-code file is not RepRap flavor.
        /// </summary>
        /// <returns></returns>
        public string GetRepRapGCode()
        {
            if (UploadedGCodeType == GCodeType.RepRap)
            {
                return _gCodeFileManagerModel.UploadedGCodeModel.GCodeStr;
            }
            else if (UploadedGCodeType == GCodeType.ModiPrint)
            {
                return "";
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// Return ModiPrint g-code from the uploaded g-code file.
        /// A lengthy conversion process may occur.
        /// Will return an empty string with no uploaded g-code file.
        /// </summary>
        /// <returns></returns>
        public string GetModiPrintGCode()
        {
            if (UploadedGCodeType == GCodeType.RepRap)
            {
                //Retrieve the converted g-code from the converter BGW.
                return _gCodeConverterModel.ConvertGCode(_gCodeFileManagerModel.UploadedGCodeModel.GCodeStr);
            }
            else if (UploadedGCodeType == GCodeType.ModiPrint)
            {
                return _gCodeFileManagerModel.UploadedGCodeModel.GCodeStr;
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// Retrieve a list of T commands from RepRap g-code.
        /// </summary>
        /// <returns></returns>
        public void UpdateRepRapIDList()
        {
            //Return parameter.
            _repRapIDList = new List<string>();

            //Delimit the g-code string by lines then spaces.
            string[][] repRapGCodeArr = GCodeStringParsing.GCodeTo2DArr(_gCodeFileManagerModel.UploadedGCodeModel.GCodeStr);

            //Move through each g-code line.
            for (int line = 0; (line < repRapGCodeArr.Length) && (repRapGCodeArr != null); line++)
            {
                if (repRapGCodeArr[line] != null
                && !String.IsNullOrWhiteSpace(repRapGCodeArr[line][0]))
                {
                    //Remove comments from the g-code line.
                    string[] uncommentedRepRapLine = GCodeStringParsing.RemoveGCodeComments(repRapGCodeArr[line]);

                    if ((uncommentedRepRapLine != null)
                     && (uncommentedRepRapLine.Length != 0)
                     && (!String.IsNullOrWhiteSpace(uncommentedRepRapLine[0])) 
                     && (uncommentedRepRapLine[0][0] == 'T'))
                    {
                        _repRapIDList.Add(uncommentedRepRapLine[0]);
                    }
                }
            }

            OnPropertyChanged("RepRapIDList");
        }
        #endregion

        #region Commands
        /// <summary>
        /// Reads and saves a .gcode file into RepRapGCode.
        /// </summary>
        private RelayCommand<object> _uploadGCodeFileCommand;
        public ICommand UploadGCodeFileCommand
        {
            get
            {
                if (_uploadGCodeFileCommand == null)
                { _uploadGCodeFileCommand = new RelayCommand<object>(ExecuteUploadGCodeFileCommand, CanExecuteUploadGCodeFileCommand); }
                return _uploadGCodeFileCommand;
            }
        }

        public bool CanExecuteUploadGCodeFileCommand(object notUsed)
        {
            return true;
        }

        public void ExecuteUploadGCodeFileCommand(object notUsed)
        {
            _gCodeFileName = _gCodeFileManagerModel.UploadGCodeFile();
            UpdateRepRapIDList();
            OnPropertyChanged("GCodeFileName");
            OnPropertyChanged("UploadedGCodeType");
        }

        /// <summary>
        /// Reads and saves a .mdpt file from ModiPrintGCode.
        /// </summary>
        private RelayCommand<object> _saveModiPrintGCodeFileCommand;
        public ICommand SaveModiPrintGCodeFileCommand
        {
            get
            {
                if (_saveModiPrintGCodeFileCommand == null)
                { _saveModiPrintGCodeFileCommand = new RelayCommand<object>(ExecuteSaveModiPrintGCodeFileCommand, CanExecuteSaveModiPrintGCodeFileCommand); }
                return _saveModiPrintGCodeFileCommand;
            }
        }

        public bool CanExecuteSaveModiPrintGCodeFileCommand(object notUsed)
        {
            return true;
        }

        public void ExecuteSaveModiPrintGCodeFileCommand(object notUsed)
        {
            _gCodeFileManagerModel.SaveGCodeFile(GetModiPrintGCode(), ".mdpt", "ModiPrint GCode Files (.mdpt)|*.mdpt", ref _gCodeFileName);
        }
        #endregion
    }
}

