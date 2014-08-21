using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TotalCommander.Services
{
    public class TranslationService : TotalCommander.Services.ITranslationService
    {
        public string GetString(string value)
        {
            return (string)Application.Current.Resources[value];
        }
    }
}
