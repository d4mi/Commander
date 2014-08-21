using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TotalCommander.Services;

namespace TotalCommander.Common
{
    public class ErrorDialogService : IErrorDialogService
    {
        private ITranslationService _translationService;

        public ErrorDialogService(ITranslationService translationService)
        {
            _translationService = translationService;
        }

        public void ShowMessageBox(string message)
        {
            MessageBox.Show(message, _translationService.GetString("Error"),
                MessageBoxButton.OK,
                MessageBoxImage.Error); 
        }
    }
}
