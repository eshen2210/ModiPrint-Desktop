using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using ModiPrint.ViewModels;
using ModiPrint.ViewModels.PrinterViewModels;

namespace ModiPrint.Models.XMLSerializerModels.PrinterXMLSerializerModels.PrinterSettingsXMLSerializerModels
{
    public class PrinterSettingsXMLDeserializerModel : XMLConverterModel
    {
        #region Contructor
        public PrinterSettingsXMLDeserializerModel(ErrorListViewModel ErrorListViewModel) : base(ErrorListViewModel)
        {

        }
        #endregion

        #region Methods
        /// <summary>
        /// Reads PrinterSettingsViewModel properties from XML and sets a PrinterSettingsViewModel with said properties.
        /// </summary>
        /// <param name="xmlReader"></param>
        /// <param name="printerSettingsViewModel"></param>
        public void DeserializePrinterSettings(XmlReader xmlReader, PrinterSettingsViewModel printerSettingsViewModel)
        {
            //Read through each element of the XML string and populate each property of the Axis.
            while (xmlReader.Read())
            {
                //Skip through newlines (this program's XML Writer uses newlines).
                if ((xmlReader.Name != "\n") && (!String.IsNullOrWhiteSpace(xmlReader.Name)))
                {
                    //End method if the end of "Axis" element is reached.
                    if ((xmlReader.Name == "PrinterSettings") && (xmlReader.NodeType == XmlNodeType.EndElement))
                    {
                        return;
                    }

                    switch (xmlReader.Name)
                    {
                        default:
                            base.ReportErrorUnrecognizedElement(xmlReader);
                            break;
                    }
                }
            }
        }
        #endregion
    }
}
