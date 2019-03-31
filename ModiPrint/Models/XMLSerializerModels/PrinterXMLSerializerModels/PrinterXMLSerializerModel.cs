using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;
using ModiPrint.Models.XMLSerializerModels.PrinterXMLSerializerModels.AxisXMLSerializerModels;
using ModiPrint.Models.XMLSerializerModels.PrinterXMLSerializerModels.PrintheadXMLSerializerModels;
using ModiPrint.Models.XMLSerializerModels.PrinterXMLSerializerModels.PrinterSettingsXMLSerializerModels;
using ModiPrint.ViewModels.PrinterViewModels;
using ModiPrint.ViewModels.PrinterViewModels.AxisViewModels;
using ModiPrint.ViewModels.PrinterViewModels.PrintheadViewModels;

namespace ModiPrint.Models.XMLSerializerModels.PrinterXMLSerializerModels
{
    public class PrinterXMLSerializerModel
    {
        #region Fields and Properties
        //Contains functions to generate XML from Printer equipment classes.
        AxisXMLSerializerModel _axisXMLSerializerModel;
        PrintheadXMLSerializerModel _printheadXMLSerializerModel;
        PrinterSettingsXMLSerializerModel _printerSettingsXMLSerializerModel;
        #endregion

        #region Constructor
        public PrinterXMLSerializerModel()
        {
            _axisXMLSerializerModel = new AxisXMLSerializerModel();
            _printheadXMLSerializerModel = new PrintheadXMLSerializerModel();
            _printerSettingsXMLSerializerModel = new PrinterSettingsXMLSerializerModel();
        }
        #endregion

        #region Methods
        /// <summary>
        /// Writes Printer properties into XML.
        /// </summary>
        /// <param name="xmlWriter"></param>
        /// <param name="printerViewModel"></param>
        public void SerializePrinter(XmlWriter xmlWriter, PrinterViewModel printerViewModel)
        {
            //Outmost element should be "Printer".
            xmlWriter.WriteStartElement("Printer");

            //Output XML for Axes.
            foreach (AxisViewModel axisViewModel in printerViewModel.AxisViewModelList)
            {
                _axisXMLSerializerModel.SerializeAxis(xmlWriter, axisViewModel);
            }

            //Output XML for Printheads.
            foreach (PrintheadViewModel printheadViewModel in printerViewModel.PrintheadViewModelList)
            {
                _printheadXMLSerializerModel.SerializePrinthead(xmlWriter, printheadViewModel);
            }

            //Printer Settings.
            _printerSettingsXMLSerializerModel.SerializeSettings(xmlWriter, printerViewModel.PrinterSettingsViewModel);

            //Number of Z Axes Created.
            xmlWriter.WriteElementString("ZAxesCreatedCount", printerViewModel.ZAxesCreatedCount.ToString());

            //Number of Printheads Created.
            xmlWriter.WriteElementString("PrintheadsCreatedCount", printerViewModel.PrintheadsCreatedCount.ToString());            
            
            //Close outmost element "Printer".
            xmlWriter.WriteEndElement();
        }
        #endregion
    }
}
