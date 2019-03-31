using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModiPrint.Models.GCodeConverterModels
{
    /// <summary>
    /// Contains various methods for parsing GCode.
    /// </summary>
    public static class GCodeStringParsing
    {
        /// <summary>
        /// Takes a string and returns only the first number within the string.
        /// Supports negative/positive, integer/decimal numbers.
        /// </summary>
        public static double ParseDouble(string phrase)
        {
            return double.Parse(Regex.Match(phrase, @"-?\d+(\.\d+)?$").Value);
        }

        /// <summary>
        /// Converts a string (of GCode) into a 2D array.
        /// </summary>
        /// <remarks>
        /// The 2D array facilitates the sorting and parsing of GCode.
        /// The first dimension is the string delimited by line breaks.
        /// The second dimention is a first dimension element delimited by whitespaces.
        /// </remarks>
        public static string[][] GCodeTo2DArr(string gCodeStr)
        {
            //First dimenstion.
            //Delimits string by line breaks.
            string[] gCode1DArr = gCodeStr.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);

            //Second dimenstion.
            //Delimits each array of the first dimension by whitespaces.
            string[][] gCode2DArr = new string[gCode1DArr.Length][];
            for (int line = 0; line < gCode1DArr.Length; line++)
            { gCode2DArr[line] = gCode1DArr[line].Split(new string[] { }, StringSplitOptions.None); }

            return gCode2DArr;
        }

        /// <summary>
        /// Stitches back together the second dimension of a GCode 2D array that was originally delimited by whitespace.
        /// </summary>
        public static string GCodeLineArrToStr(string[] gCodeLineFragment)
        {
            string returnString = "";

            for (int i = 0; i < gCodeLineFragment.Length - 1; i++)
            { returnString += gCodeLineFragment[i] + " "; }
            returnString += gCodeLineFragment[gCodeLineFragment.Length - 1]; //So that the last character is not a white space.

            return returnString;
        }
    }
}
