using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using TotalCommander.Common;
using TotalCommander.Model;
using TotalCommander.Services;
using TotalCommander.Services.Dialogs;

namespace TotalCommander.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        #region Fields

        private readonly FilesViewModel _leftFilesViewModel;
        private readonly FilesViewModel _rightFilesViewModel;

        private ICopyDialogService _copyDialogService;
        private ITranslationService _translationService;
        private IQuestionDialogService _questionDialogService;

        #endregion // Fields

        #region Properties

        public bool IsFocusedLeft { get; set; }
        public bool IsFocusedRight { get; set; }

        public FilesViewModel LeftFilesViewModel
        {
            get { return _leftFilesViewModel; }
        }

        public FilesViewModel RightFilesViewModel
        {
            get { return _rightFilesViewModel; }
        }

        #endregion // Properties

        #region Ctor

        public MainWindowViewModel(FilesViewModel leftFilesViewModel, FilesViewModel rightFilesViewModel, 
            ICopyDialogService copyDialogService, 
            ITranslationService translationService,
            IQuestionDialogService questionDialogService)
        {
            _rightFilesViewModel = leftFilesViewModel;
            _leftFilesViewModel = rightFilesViewModel;

            _copyDialogService = copyDialogService;
            _translationService = translationService;
            _questionDialogService = questionDialogService;

        }

        #endregion // Ctor

        #region Commands

        private ICommand _changeLanguage;
        public ICommand ChangeLanguage
        {
            get
            {
                if (_changeLanguage == null)
                {
                    _changeLanguage = new RelayCommand(param => this.OnLanguageChanged((string)param));
                }

                return _changeLanguage;
            }
        }



        private ICommand _runApplication;
        public ICommand RunApplication
        {
            get
            {
                if (_runApplication == null)
                {
                    _runApplication = new RelayCommand( param => this.OnRunApplication((string)param) ); 
                }

                return _runApplication;
            }
        }

        private ICommand _copyFile;
        public ICommand CopyFile
        {
            get
            {
                if (_copyFile == null)
                {
                    _copyFile = new RelayCommand(param => this.OnCopy(),
                        param => _leftFilesViewModel.CurrentItem != null);
                }

                return _copyFile;
            }
        }

        private ICommand _deleteFile;
        public ICommand DeleteFile
        {
            get
            {
                if (_deleteFile == null)
                {
                    _deleteFile = new RelayCommand(param => this.OnDelete(),
                        param => _leftFilesViewModel.CurrentItem != null);
                }

                return _deleteFile;
            }
        }

        private ICommand _moveFile;
        public ICommand MoveFile
        {
            get
            {
                if (_moveFile == null)
                {
                    _moveFile = new RelayCommand(param => this.OnMove(),
                        param => _leftFilesViewModel.CurrentItem != null);
                }

                return _moveFile;
            }
        }  

        #endregion // Commands

        #region Private Helpers

        private void OnRunApplication(string param)
        {
            switch (param)
            {
                case "Notepad":
                    System.Diagnostics.Process.Start(@"notepad.exe");
                    break;
                default:
                    break;
            }
        }

        private void OnCopy()
        {
            string file = _leftFilesViewModel.CurrentItem.FullPath;
            string destination = _rightFilesViewModel.CurrentDir.FullName;

            _copyDialogService.Show(file, destination);
        }

        private void OnMove()
        {
            string file = _leftFilesViewModel.CurrentItem.FullPath;
            string destination = _rightFilesViewModel.CurrentDir.FullName;

            MoveDialogViewModel moveDialogViewModel = new MoveDialogViewModel(file, destination);

            View.MoveDialogView moveDialog = new View.MoveDialogView();
            moveDialog.DataContext = moveDialogViewModel;

            System.Windows.Window window = new System.Windows.Window()
            {
                Title = file,
                Content = moveDialog,
                Width = 600,
                Height = 250,
                WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen
            };
            window.ShowDialog();
        }

        private void OnDelete()
        {
            bool isDirecotry = _leftFilesViewModel.CurrentItem.IsDirectory;
            string file = _leftFilesViewModel.CurrentItem.FullPath;
            string question = _translationService.GetString("deleteFile");

            if( _questionDialogService.Show(question) )
            {
                try
                {
                    if (isDirecotry)
                    {
                        DirectoryInfo directory = new DirectoryInfo(file);

                        foreach (FileInfo f in directory.GetFiles())
                            f.Delete();
                        foreach (DirectoryInfo subDirectory in directory.GetDirectories())
                            subDirectory.Delete(true);

                        directory.Delete();
                    }
                    else
                    {
                        System.IO.File.Delete(file);
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                    return;
                }
            }
        }

        private void OnLanguageChanged(string param)
        {
            App.Change(param);
            _leftFilesViewModel.UpdateFilesView(_leftFilesViewModel.CurrentDir.FullName);
            _rightFilesViewModel.UpdateFilesView(_rightFilesViewModel.CurrentDir.FullName);


        }

        #endregion // Private Helpers


    }
}
