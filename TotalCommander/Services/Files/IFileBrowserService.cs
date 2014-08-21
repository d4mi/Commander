using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Media;
namespace TotalCommander.Model.Utils
{
    public interface IFileBrowserService
    {
        void AddBackFolder(ObservableCollection<FileModel> filesCollection);
        void AddDirectories(DirectoryInfo dir, ObservableCollection<FileModel> filesCollection);
        void AddDrives(ObservableCollection<Drive> drivesCollection);
        void AddFiles(DirectoryInfo dir, ObservableCollection<FileModel> filesCollection);
        ImageSource CreateImageFromIcon(string fileName);
    }
}
