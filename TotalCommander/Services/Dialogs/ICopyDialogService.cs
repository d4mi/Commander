using System;
namespace TotalCommander.Services.Dialogs
{
    public interface ICopyDialogService
    {
        void Show(string file, string destination);
    }
}
