using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace ModiPrint.Models.GCodeConverterModels.ProcessModels.ProcessG00Models
{
    /// <summary>
    /// Contains methods that perform calculations for G00 command conversion.
    /// </summary>
    public static class G00Calculator
    {
        #region Methods
        /// <summary>
        /// Outputs a GCode segment containing information about an Axis and its relative movement (e.g. X#).
        /// </summary>
        /// <param name="axisName"></param>
        /// <param name="startCoord"></param>
        /// <param name="finishCoord"></param>
        /// <param name="mmPerStep"></param>
        /// <returns></returns>
        public static string WriteSteps(char axisName, double distance, double mmPerStep, bool invertDirection)
        {
            int steps = (int)Math.Round(DistanceToSteps(distance, mmPerStep, invertDirection));
            steps = (invertDirection == false) ? steps : (-1 * steps);
            if (steps != 0)
            {
                return " " + axisName.ToString() + steps.ToString();
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// Calculates distance (in mm) based on steps and Axis parameters.
        /// </summary>
        /// <param name="steps"></param>
        /// <param name="mmPerStep"></param>
        /// <param name="invertDirection"></param>
        /// <returns></returns>
        public static double StepsToDistance(int steps, double mmPerStep, bool invertDirection)
        {
            return (invertDirection == false) ? (steps * mmPerStep) : (-1 * steps * mmPerStep);
        }

        /// <summary>
        /// Calculates steps based on distance and Axis parameters.
        /// Returns a double value to leave the rounding open ended.
        /// </summary>
        /// <param name="distance"></param>
        /// <param name="mmPerStep"></param>
        /// <param name="invertDirection"></param>
        /// <returns></returns>
        public static double DistanceToSteps(double distance, double mmPerStep, bool invertDirection)
        {
            return (mmPerStep == 0) ? 0 : Math.Round(distance / mmPerStep);
        }

        /// <summary>
        /// Calculates the distance between two coordinates in 3 dimension.
        /// </summary>
        /// <param name="xStart"></param>
        /// <param name="xFinish"></param>
        /// <param name="yStart"></param>
        /// <param name="yFinish"></param>
        /// <param name="zStart"></param>
        /// <param name="zFinish"></param>
        /// <returns></returns>
        public static double CalculateDistance(double xDistance, double yDistance, double zDistance)
        {
            return Math.Sqrt(Math.Pow((xDistance), 2) + Math.Pow((yDistance), 2) + Math.Pow((zDistance), 2));
        }
        #endregion
    }
}
