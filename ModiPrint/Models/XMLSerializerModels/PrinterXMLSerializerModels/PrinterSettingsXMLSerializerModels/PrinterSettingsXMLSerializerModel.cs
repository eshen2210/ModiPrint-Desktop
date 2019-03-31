using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using ModiPrint.ViewModels.PrinterViewModels;

namespace ModiPrint.Models.XMLSerializerModels.PrinterXMLSerializerModels.PrinterSettingsXMLSerializerModels
{
    public class PrinterSettingsXMLSerializerModel
    {
        #region Construtor
        public PrinterSettingsXMLSerializerModel()
        {

        }
        #endregion

        #region Methods
        /// <summary>
        /// Writes PrinterSettingsViewModel properties into a XmlWriter.
        /// </summary>
        /// <param name="xmlWriter"></param>
        /// <param name="axisViewModel"></param>
        public void SerializeSettings(XmlWriter xmlWriter, PrinterSettingsViewModel printerSettingsViewModel)
        {
            /*
            //Outmost element should be "PrinterSettings".
            xmlWriter.WriteStartElement("PrinterSettings");

            //End "PrinterSettings" outmost element.
            xmlWriter.WriteEndElement();
            */
        }
        #endregion
    }
}
