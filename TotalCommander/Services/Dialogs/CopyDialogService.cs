using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TotalCommander.ViewModel;

namespace TotalCommander.Services.Dialogs
{
    public class CopyDialogService : TotalCommander.Services.Dialogs.ICopyDialogService
    {
        public void Show(string file, string destination)
        {
            CopyDialogViewModel copyDialogViewModel = new CopyDialogViewModel(file, destination);

            View.CopyDialogView copyDialog = new View.CopyDialogView();
            copyDialog.DataContext = copyDialogViewModel;

            System.Windows.Window window = new System.Windows.Window()
            {
                Title = file,
                Content = copyDialog,
                Width = 600,
                Height = 250,
                WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen
            };
            window.ShowDialog();
        }
    }
}
