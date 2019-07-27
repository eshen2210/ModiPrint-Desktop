using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Diagnostics;


namespace ModiPrint.ViewModels
{
    public class UnsetMainViewModel : ViewModel
    {
        #region Fields and Properties

        #endregion

        #region Constructor
        public UnsetMainViewModel()
        {

        }
        #endregion

        #region Methods

        #endregion

        #region Commands
        /// <summary>
        /// Hyperlinks to the McCloskey Lab homepage.
        /// </summary>
        private RelayCommand<object> _mcCloskeyLabHyperlinkCommand;
        public ICommand McCloskeyLabHyperlinkCommand
        {
            get
            {
                if (_mcCloskeyLabHyperlinkCommand == null)
                { _mcCloskeyLabHyperlinkCommand = new RelayCommand<object>(ExecuteMcCloskeyLabHyperLinkCommand, CanExecuteMcCloskeyLabHyperLinkCommand); }
                return _mcCloskeyLabHyperlinkCommand;
            }
        }

        public bool CanExecuteMcCloskeyLabHyperLinkCommand(object notUsed)
        {
            return true;
        }

        public void ExecuteMcCloskeyLabHyperLinkCommand(object notUsed)
        {
            Process.Start("http://www.kara-mccloskey.squarespace.com/");
        }
        #endregion
    }
}
