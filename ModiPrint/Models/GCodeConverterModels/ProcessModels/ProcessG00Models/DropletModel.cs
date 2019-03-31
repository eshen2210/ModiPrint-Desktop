using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using ModiPrint.Models.PrintModels.PrintStyleModels.GradientModels;
using ModiPrint.Models.PrintModels.PrintStyleModels;

namespace ModiPrint.Models.GCodeConverterModels.ProcessModels.ProcessG00Models
{
    public class DropletModel
    {
        #region Fields and Properties
        //Distance already travelled since the previous printed droplet.
        //When the distanceTravelled == interpolateDistance, the system should print.
        //In mm.
        private double _distanceTravelled = 0;

        //Distance in between each droplet.
        private double _interpolateDistance;

        //Is this the first droplet?
        //If so, then a droplet needs to be deposited at the beginning of the first movement.
        private bool _firstDroplet = true;

        //Contains functions for managing the Gradient. 
        private GradientModel _gradientModel;
        public GradientModel GradientModel
        {
            get { return _gradientModel; }
        }

        private GradientShape _gradientShape;
        public GradientShape GradientShape
        {
            get { return _gradientShape; }
        }
        private GradientScaling _gradientScaling;
        public GradientScaling GradientScaling
        {
            get { return _gradientScaling; }
        }

        #endregion

        #region Constructor
        /// <summary>
        /// Supports concentration gradients.
        /// </summary>
        /// <param name="DropletPrintStyleModel"></param>
        public DropletModel(DropletPrintStyleModel DropletPrintStyleModel)
        {
            _interpolateDistance = DropletPrintStyleModel.InterpolateDistance;
            _gradientShape = DropletPrintStyleModel.GradientShape;
            _gradientScaling = DropletPrintStyleModel.GradientScaling;

            Point3D point1 = new Point3D(DropletPrintStyleModel.X1, DropletPrintStyleModel.Y1, DropletPrintStyleModel.Z1);
            Point3D point2 = new Point3D(DropletPrintStyleModel.X2, DropletPrintStyleModel.Y2, DropletPrintStyleModel.Z2);
            Point3D point3 = new Point3D(DropletPrintStyleModel.X3, DropletPrintStyleModel.Y3, DropletPrintStyleModel.Z3);

            switch (DropletPrintStyleModel.GradientShape)
            {
                case GradientShape.None:
                    _gradientModel = null;
                    break;
                case GradientShape.Point:
                    _gradientModel = new PointModel(_gradientScaling, DropletPrintStyleModel.PercentPerMm, point1);
                    break;
                case GradientShape.Line:
                    _gradientModel = new LineModel(_gradientScaling, DropletPrintStyleModel.PercentPerMm, point1, point2);
                    break;
                case GradientShape.Plane:
                    _gradientModel = new PlaneModel(_gradientScaling, DropletPrintStyleModel.PercentPerMm, point1, point2, point3);
                    break;
            }
        }

        /// <summary>
        /// Does not support concentration gradients.
        /// </summary>
        /// <param name="InterpolateDistance"></param>
        public DropletModel(double InterpolateDistance)
        {
            _interpolateDistance = InterpolateDistance;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Returns a list of relative positions that should be droplet printed.
        /// Positions are relative and in units of mm.
        /// </summary>
        /// <param name="xDistance"></param>
        /// <param name="yDistance"></param>
        /// <param name="zDistance"></param>
        /// <returns></returns>
        public List<double[]> PrintPosition(double xDistance, double yDistance, double zDistance)
        {
            //All position values in this function are relative to the starting point of movements.
            //Rounding is used here to prevent errors incurred from using floating point values.

            xDistance = Math.Round(xDistance, 8);
            yDistance = Math.Round(yDistance, 8);
            zDistance = Math.Round(zDistance, 8);

            double distanceToPrint = Math.Round(_interpolateDistance - _distanceTravelled, 8);
            double maxDistance = Math.Round(G00Calculator.CalculateDistance(xDistance, yDistance, zDistance), 8);
            List<double[]> printDistancesList = new List<double[]>(); //Relative coordinates in units of mm.

            //Is this the first droplet?
            //If so, then a droplet needs to be deposited at the beginning of the first movement.
            if (_firstDroplet == true)
            {
                double[] printCoord = new double[3];
                printCoord[0] = printCoord[1] = printCoord[2] = 0;

                printDistancesList.Add(printCoord);

                _firstDroplet = false;
            }

            //Movement is too short for the interpolation.
            //No print happens.
            if (distanceToPrint > maxDistance)
            {
                _distanceTravelled += maxDistance;
                return null;
            }
            //Movement is exactly the length needed for the next droplet.
            //The print happens at the end of the movement.
            else if (distanceToPrint == maxDistance)
            {
                _distanceTravelled = 0;

                double[] printCoord = new double[3];
                printCoord[0] = xDistance;
                printCoord[1] = yDistance;
                printCoord[2] = zDistance;

                printDistancesList.Add(printCoord);
                return printDistancesList;
            }
            //Movement is longer than the length needed for the next droplet.
            //One or mutliple prints happen in the middle of the movement.
            else if (distanceToPrint < maxDistance)
            {
                double percentMovementTravelled = 0; //Percentage of the movement that needs to be traversed until the next droplet.
                double xPrintPosition = 0; //Last position printed at.
                double yPrintPosition = 0;
                double zPrintPosition = 0;
                for (percentMovementTravelled = Math.Abs(distanceToPrint / maxDistance); Math.Round(percentMovementTravelled, 10) <= 1; percentMovementTravelled += (_interpolateDistance / maxDistance))
                {
                    double xMovementTravelled = percentMovementTravelled * (xDistance);
                    double yMovementTravelled = percentMovementTravelled * (yDistance);
                    double zMovementTravelled = percentMovementTravelled * (zDistance);

                    xPrintPosition = xMovementTravelled;
                    yPrintPosition = yMovementTravelled;
                    zPrintPosition = zMovementTravelled;

                    double[] printCoord = new double[3];
                    printCoord[0] = xPrintPosition;
                    printCoord[1] = yPrintPosition;
                    printCoord[2] = zPrintPosition;
                    printDistancesList.Add(printCoord);

                    _distanceTravelled = 0;
                }

                _distanceTravelled += G00Calculator.CalculateDistance(xDistance - xPrintPosition, yDistance - yPrintPosition, zDistance - zPrintPosition);
                return printDistancesList;
            }

            //Should not reach this point.
            return null;
        }
        #endregion
    }
}
