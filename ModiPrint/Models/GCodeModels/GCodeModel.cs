using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Win32;

namespace ModiPrint.Models.GCodeModels
{
    public class GCodeModel
    {
        #region Fields and Properties
        //GCode delimited by newline.
        //GCodeArr is meant to be used for processing functions.
        private string[] _gCodeArr;
        public string[] GCodeArr
        {
            get { return _gCodeArr; }
        }

        //GCode as a full string.
        //GCodeStr is meant to be displayed in the GUI.
        private string _gCodeStr;
        public string GCodeStr
        {
            get { return _gCodeStr; }
            set { SetGCode(value); }
        }
        #endregion

        #region Constructors
        public GCodeModel()
        {

        }
        #endregion

        #region Methods
        /// <summary>
        /// Parses the string parameter into a string[] with newline as the delimiter and sets the string[] GCode property as this string[].
        /// Sets the string GCode property as the string parameter.
        /// </summary>
        /// <param name="gCode"></param>
        public void SetGCode(string newGCode)
        {
            Clear();
            _gCodeArr = newGCode.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
            _gCodeStr = newGCode;
        }

        /// <summary>
        /// Pastes the string[] parameter into a string with newline as the delimiter.
        /// Sets the string[] parameter as the string[] GCode property.
        /// </summary>
        /// <param name="newGCode"></param>
        public void SetGCode(string[] newGCode)
        {
            Clear();
            _gCodeArr = newGCode;
            for (int line = 0; line < newGCode.Length; line++)
            { _gCodeStr += newGCode[line] + "\r\n"; }
        }

        /// <summary>
        /// Sets both GCode properties to default values.
        /// </summary>
        public void Clear()
        {
            if (_gCodeArr != null)
            { _gCodeArr = new string[0]; }
            _gCodeStr = "";
        }
        #endregion
    }
}
