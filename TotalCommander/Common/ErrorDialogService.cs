using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TotalComander.Common
{
    public class ErrorDialogService : IDialogService
    {
        public void ShowMessageBox(string message)
        {
            MessageBox.Show(message); 
        }
    }
}
