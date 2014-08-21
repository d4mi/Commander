using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TotalCommander.Common
{
    public interface IErrorDialogService
    {
        void ShowMessageBox(string message);
    }
}
