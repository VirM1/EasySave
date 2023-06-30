using System;
using System.Collections.Generic;
using System.Text;

namespace EasySaveWPF.ViewModels
{
    public interface IViewModel
    {
        /// <summary>
        /// Method invoked once we show the said ViewModel.
        /// </summary>
        public void OnShow();
    }
}
