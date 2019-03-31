﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModiPrint.Models.GCodeConverterModels
{
    /// <summary>
    /// The GCodeConverter will use this class to store information on each line of GCode.
    /// </summary>
    public class ConvertedGCodeLine
    {
        #region Fields and Methods
        //The actual line of GCode that will be sent to the microcontroller.
        private string _gCode;
        public string GCode
        {
            get { return _gCode; }
            set { _gCode = value; }
        }

        //Comments that will not be sent to the microcontroller.
        private string _comment;
        public string Comment
        {
            get { return _comment; }
            set { _comment = value; }
        }
        #endregion

        #region Constructor
        public ConvertedGCodeLine(string GCode, string Comment)
        {
            _gCode = GCode;
            _comment = Comment;
        }

        public ConvertedGCodeLine(string GCode)
        {
            _gCode = GCode;
        }
        #endregion
    }

    public static class GCodeLinesConverter
    {
        /// <summary>
        /// Converts a list of ConvertedGCodeLines to the string format.
        /// </summary>
        /// <param name="convertedGCodeLinesList"></param>
        /// <returns></returns>
        public static string GCodeLinesListToString(List<ConvertedGCodeLine> convertedGCodeLinesList)
        {
            //Return value.
            string returnString = "";
            if (convertedGCodeLinesList != null)
            {
                for (int i = 0; i < convertedGCodeLinesList.Count; i++)
                {
                    returnString += convertedGCodeLinesList[i].GCode;

                    if (!String.IsNullOrWhiteSpace(convertedGCodeLinesList[i].Comment))
                    {
                        returnString += " ; " + convertedGCodeLinesList[i].Comment;
                    }

                    if (i != (convertedGCodeLinesList.Count - 1))
                    {
                        returnString += "\r\n";
                    }
                }
            }
            return returnString;
        }
    }
}
