using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TotalCommander.Services.Dialogs
{
    public class QuestionDialogService : TotalCommander.Services.Dialogs.IQuestionDialogService
    {
        public bool Show(string question)
        {
            if (System.Windows.MessageBox.Show(
                  question,
                   "Question",
                  System.Windows.MessageBoxButton.YesNo,
                  System.Windows.MessageBoxImage.Warning) == System.Windows.MessageBoxResult.Yes)
            {
                return true;
            }

            return false;
        }
    }
}
