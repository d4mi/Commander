using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using TotalCommander.Common;

namespace TotalCommander.ViewModel
{
    public class CopyDialogViewModel : ViewModelBase
    {
        #region Fields

        private string _filePath;
        private string _destinationPath;
        private int _progress;
        private int _allProgress;
        private CancellationTokenSource _cancellationTokenSource;


        #endregion // Fields

        #region Properties

        public string FilePath
        {
            get { return _filePath; }
            set { SetField(ref _filePath, value); }
        }

        public string DestinationPath
        {
            get { return _destinationPath; }
            set { SetField(ref _destinationPath, value); }
        }

        public int Progress
        {
            get { return _progress; }
            set { SetField(ref _progress, value); }
        }

        public int AllProgress
        {
            get { return _allProgress; }
            set { SetField(ref _allProgress, value); }
        }

        #endregion // Properties

        #region Ctor

        public CopyDialogViewModel(string file, string destination)
        {
            FilePath = file;
            DestinationPath = destination;
            Progress = 0;
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public CopyDialogViewModel()
        {

        }

        #endregion // Ctor

        #region Command

        private ICommand _okClicked;
        public ICommand OkClicked
        {
            get
            {
                if (_okClicked == null)
                {
                    _okClicked = new RelayCommand(param => this.OnOkClicked());
                }

                return _okClicked;
            }
        }

        private ICommand _cancelClicked;
        public ICommand CancelClicked
        {
            get
            {
                if (_cancelClicked == null)
                {
                    _cancelClicked = new RelayCommand(param => this.OnCancelClicked());
                }

                return _cancelClicked;
            }
        }

        #endregion // Commands

        #region Methods

        private void OnCancelClicked()
        {
            _cancellationTokenSource.Cancel();            
        }

        private async void OnOkClicked()
        {
            var cprogress = new Progress<StreamCopyProgress>();
            cprogress.ProgressChanged += new EventHandler<StreamCopyProgress>((sender, e) =>
            {
                Application.Current.Dispatcher.Invoke((Action)(delegate
                {
                    this.Progress = (int)e.CurrentBytes;
                }));
            });

            string StartDirectory = _filePath;
            string EndDirectory = _destinationPath;
            var cancellationToken = _cancellationTokenSource.Token;
            

            FileAttributes attr = File.GetAttributes(_filePath);
            if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
            {
                DirectoryInfo directory = new DirectoryInfo(_filePath);
                DirectoryInfo targetDirectory = new DirectoryInfo(_destinationPath + "\\" + directory.Name);

                int fCount = Directory.GetFiles(directory.FullName, "*.*", SearchOption.AllDirectories).Length;
                int current = 0;
                
                foreach (DirectoryInfo dirInfo in directory.GetDirectories("*", SearchOption.AllDirectories))
                {
                    string dirPath = dirInfo.FullName;
                    string outputPath = dirPath.Replace(directory.FullName, targetDirectory.FullName);
                    Directory.CreateDirectory(outputPath);

                    foreach (FileInfo file in dirInfo.EnumerateFiles())
                    {
                        using (FileStream SourceStream = file.OpenRead())
                        {
                            using (FileStream DestinationStream = File.Create(outputPath +"\\"+ file.Name))
                            {
                                try
                                {
                                    await SourceStream.CopyToAsync(DestinationStream, cancellationToken, cprogress);
                                }
                                catch (OperationCanceledException)
                                {
                                    Progress = 0;
                                    AllProgressChanged(0);
                                }
                                current++;
                                AllProgressChanged((int)((double)current / (double)fCount * 100));
                            }
                        }
                    }
                }
            }
            else
            {
                string filename = _filePath;
                using (FileStream SourceStream = File.Open(filename, FileMode.Open))
                {
                    using (FileStream DestinationStream = File.Create(EndDirectory + filename.Substring(filename.LastIndexOf('\\'))))
                    {
                        try
                        {
                            await SourceStream.CopyToAsync(DestinationStream, cancellationToken, cprogress);
                        }
                        catch (OperationCanceledException)
                        {
                            MessageBox.Show("Canceled");
                            Progress = 0;
                        }
                    }
                }
            }
        }

        private void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke((Action)(delegate
            {
                this.Progress = e.ProgressPercentage;
            }));
        }

        private void AllProgressChanged(int value)
        {
            Application.Current.Dispatcher.Invoke((Action)(delegate
            {
                this.AllProgress = value;
            }));
        }

        #endregion // Methods
    }
}
