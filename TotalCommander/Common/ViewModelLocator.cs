using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TotalCommander.Model;
using TotalCommander.Model.Utils;
using TotalCommander.Services;
using TotalCommander.Services.Dialogs;
using TotalCommander.Services.Sorting;
using TotalCommander.ViewModel;

namespace TotalCommander.Common
{
    public class ViewModelLocator
    {
        private readonly UnityContainer _container;

        public ViewModelLocator()
        {
            _container = new UnityContainer();
            _container.RegisterType<IErrorDialogService, ErrorDialogService>();
            _container.RegisterType<IFileBrowserService, FileBrowserService>();
            _container.RegisterType<ITranslationService, TranslationService>();
            _container.RegisterType<IFileSorter, FileSorter>();
            _container.RegisterType<ICopyDialogService, CopyDialogService>();
            _container.RegisterType<IQuestionDialogService, QuestionDialogService>();
        }

        public MainWindowViewModel MainWindow
        {
            get { return _container.Resolve<MainWindowViewModel>(); }
        }
    }
}
