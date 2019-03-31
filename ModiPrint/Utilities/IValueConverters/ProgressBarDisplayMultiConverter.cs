using System;
using System.Globalization;
using System.Windows.Data;

namespace ModiPrint.Utilities.IValueConverters
{
    /// <summary>
    /// Inteded to be used as in a TextBlock/Label over a ProgressBar.
    /// Displays the number of tasks completed/to-be-completed as well as the percentage completed.
    /// </summary>
    public class ProgressBarDisplayMultiConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            string returnString = "";
            int processedItems = (int)values[0];
            int totalItems = (int)values[1];

            if ((processedItems != 0) && (totalItems != 0))
            {
                decimal percentageDone = (decimal)(processedItems * 100 / totalItems);
                returnString = processedItems + " / " + totalItems + "   (" + percentageDone + "%)";
            }
            return returnString;
        }

        public object[] ConvertBack(object value, Type[]targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
