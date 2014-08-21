using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TotalCommander.Common;
using TotalCommander.Model;
using TotalCommander.Model.Utils;
using TotalCommander.Services;
using TotalCommander.Services.Sorting;

namespace TotalCommander.ViewModel
{
    public class FilesViewModel : ViewModelBase, IDisposable
    {
        #region Fields

        private IErrorDialogService _dialogService;
        private IFileBrowserService _fileBrowser;
        private ITranslationService _translationService;
        private IFileSorter _fileSorter;

        private ObservableCollection<FileModel> _files;
        private ObservableCollection<Drive> _drives;
        private DirectoryInfo _currentDir;
        private Drive _currentDrive;
        private FileModel _currentItem;

        private FileSystemWatcher _watcher;

        private bool _sortedNameAsc = true;

        #endregion // Fields

        #region Properties

        public FileModel CurrentItem 
        {
            get { return _currentItem; }
            set { SetField(ref _currentItem, value); }
        }

        public int DirectoryCount
        {
            get { return CurrentDir.GetDirectories().Length; }
        }

        public int FilesCount
        {
            get { return CurrentDir.GetFiles().Length; }
        }

        public ObservableCollection<FileModel> Files
        {
            get { return _files; }
            set { SetField(ref _files, value); }
        }

        public ObservableCollection<Drive> Drives
        {
            get { return _drives; }
            set { SetField(ref _drives, value); }
        }

        public Drive CurrentDrive
        {
            get { return _currentDrive; }
            set { SetField(ref _currentDrive, value); }
        }

        public DirectoryInfo CurrentDir
        {
            get { return _currentDir; }
            set 
            {               
                SetField(ref _currentDir, value);
                base.RaisePropertyChanged("DirectoryCount");
                base.RaisePropertyChanged("FilesCount");
            }
        }

        public string DirectoryTranslation
        {
            get { return _translationService.GetString("Dirs"); }
        }

        public string FilesTranslation
        {
            get { return _translationService.GetString("Files"); }
        }

        #endregion // Properties

        #region Ctor

        public FilesViewModel(IErrorDialogService dialogService, IFileBrowserService fileBrowser,
                                ITranslationService translationService, IFileSorter fileSorter)
        {
            _dialogService = dialogService;
            _fileBrowser = fileBrowser;
            _translationService = translationService;
            _fileSorter = fileSorter;

            Init();
        }

        ~FilesViewModel()
        {
            Dispose();
        }

        private void Init()
        {
            _files = new ObservableCollection<FileModel>();
            _drives = new ObservableCollection<Drive>();

            _fileBrowser.AddDrives(_drives);

            SetWatcher();

            if (_drives.Count() > 0)
            {
                _currentDrive = _drives[0];
                CurrentDir = new DirectoryInfo(_drives[0].Name);
                UpdateFilesView(_drives[0].Name);
            }          
        }

        private void SetWatcher()
        {
            _watcher = new FileSystemWatcher();
            _watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite
                        | NotifyFilters.FileName | NotifyFilters.DirectoryName;
            _watcher.Changed += new FileSystemEventHandler((s, e) => this.OnWatcher());
            _watcher.Created += new FileSystemEventHandler((s, e) => this.OnWatcher());
            _watcher.Renamed += new RenamedEventHandler((s, e) => this.OnWatcher());
            _watcher.Deleted += new FileSystemEventHandler((s, e) => this.OnWatcher());
            _watcher.EnableRaisingEvents = false;
        }

        #endregion // Ctor

        #region Commands

        private ICommand _itemClicked;
        public ICommand ItemClicked
        {
            get
            {
                if (_itemClicked == null)
                {
                    _itemClicked = new RelayCommand(param => this.OnItemClicked());
                }

                return _itemClicked;
            }
        }

        private ICommand _itemRightClicked;
        public ICommand ItemRightClicked
        {
            get
            {
                if (_itemRightClicked == null)
                {
                    _itemRightClicked = new RelayCommand(param => this.OnItemRightClicked());
                }

                return _itemRightClicked;
            }
        }

        private ICommand _columneClicked;
        public ICommand ColumnClicked
        {
            get
            {
                if (_columneClicked == null)
                {
                    _columneClicked = new RelayCommand(param => this.OnColumnClicked((string)param));
                }

                return _columneClicked;
            }
        }

        private ICommand _driveChanged;
        public ICommand DriveChanged
        {
            get
            {
                if (_driveChanged == null)
                {
                    _driveChanged = new RelayCommand(param => this.OnDriveChanged());
                }

                return _driveChanged;
            }
        }

        #endregion

        #region Methods

        public void UpdateFilesView(string directory)
        {
            DirectoryInfo dir = null;
            ObservableCollection<FileModel> enteringDirecotry = new ObservableCollection<FileModel>();

            try
            {
                dir = new DirectoryInfo(directory);

                if (dir.Parent != null)
                {
                    _fileBrowser.AddBackFolder(enteringDirecotry);
                }

                _fileBrowser.AddDirectories(dir, enteringDirecotry);
                _fileBrowser.AddFiles(dir, enteringDirecotry);    
            }
            catch (Exception ex)
            {
                ShowError(ex);
                return;
            }
            
            Files = enteringDirecotry;
            CurrentDir = dir;
            _watcher.Path = CurrentDir.FullName;

            if (!_watcher.EnableRaisingEvents)
            {
               _watcher.EnableRaisingEvents = true;
            }
 
            RaisePropertyChanged("FilesTranslation");
            RaisePropertyChanged("DirectoryTranslation");
        }

        private void ShowError(Exception ex)
        {
            if (ex is SecurityException || ex is UnauthorizedAccessException)
            {
                _dialogService.ShowMessageBox(_translationService.GetString("AccessDeniedMessage"));
            }
            else if (ex is ArgumentException)
            {
                _dialogService.ShowMessageBox(_translationService.GetString("ArgumentExceptionMessage"));
            }
            else if (ex is PathTooLongException)
            {
                _dialogService.ShowMessageBox(_translationService.GetString("PathTooLongMessage"));
            }
            else
            {
                _dialogService.ShowMessageBox(_translationService.GetString("WrongPathMessage"));
            }
        }

        public void OnItemClicked()
        {
            if (CurrentItem == null)
                return;

            if (CurrentItem.FileName == "[..]")
            {
                string parentPath;
                try
                {
                    parentPath = _currentDir.Parent.FullName;
                }
                catch(NullReferenceException)
                {
                    return;
                }

                UpdateFilesView(parentPath);
            }
            else
            {
                if (CurrentItem.IsDirectory)
                {
                    UpdateFilesView(CurrentItem.FullPath);
                }
                else
                {
                    RunProcess(CurrentItem.FullPath);
                }
            }
        }

        public void OnItemRightClicked()
        {
            if (CurrentItem != null)
            {
                if (CurrentItem.Selected == Brushes.Black)
                {
                    CurrentItem.Selected = Brushes.Red;
                }
                else
                {
                    CurrentItem.Selected = Brushes.Black;
                }
                 ObservableCollection<FileModel> newColl = new ObservableCollection<FileModel>(_files);
                 _files = newColl;

                RaisePropertyChanged("Files");
            }

        }

        private void RunProcess(string path)
        {
            try
            {
                System.Diagnostics.Process.Start(path);
            }
            catch (Exception)
            {
                _dialogService.ShowMessageBox("No application is associated with the specified file.");
            }
        }

        private void OnDriveChanged()
        {
            UpdateFilesView(_currentDrive.Name);
        }

        private void OnWatcher()
        {
            Application.Current.Dispatcher.Invoke((Action)(delegate
            {
                this.UpdateFilesView(CurrentDir.FullName);
            }));
        }

        private void OnColumnClicked(string columnName)
        {
            Func<FileModel, string> condition = x => "";
            switch( columnName )
            {
                case "Name":
                    condition = x => x.FileName;
                    break;
                case "Date":
                    condition = x => x.Date.ToString();
                    break;
                case "Extension":
                    condition = x => x.Extension;
                    break;
                case "Size":
                    condition = x => x.Size.ToString();
                    break;
            }

            _files = _fileSorter.SortBy(_files, condition, !_sortedNameAsc);
            _sortedNameAsc = !_sortedNameAsc;
            base.RaisePropertyChanged("Files");
        }

        public void Dispose()
        {
            _watcher.EnableRaisingEvents = false;
        }

        #endregion
    }
}
