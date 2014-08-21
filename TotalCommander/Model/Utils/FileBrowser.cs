using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace TotalComander.Model
{
    public class FileBrowser
    {
        static public void AddFiles(DirectoryInfo dir, ObservableCollection<FileModel> filesCollection)
        {
            FileInfo[] Files = dir.GetFiles();
            foreach (FileInfo file in Files)
            {
                filesCollection.Add(new FileModel()
                {
                    FileName = file.Name,
                    Date = file.LastAccessTime,
                    Size = file.Length,
                    FullPath = file.FullName,
                    Extension = file.Extension,
                    Attributes = file.Attributes.ToString(),
                    Icon = CreateImageFromIcon(file.FullName)
                });
            }
        }

        static public void AddDirectories(DirectoryInfo dir, ObservableCollection<FileModel> filesCollection)
        {
            DirectoryInfo[] directories = dir.GetDirectories();
            foreach (DirectoryInfo directory in directories)
            {
                filesCollection.Add(new FileModel()
                {
                    FileName = directory.Name,
                    FullPath = directory.FullName,
                    Attributes = directory.Attributes.ToString(),
                    Icon = new BitmapImage(new Uri("/Images/Folder.png", UriKind.Relative)),
                    IsDirectory = true
                });
            }
        }

        static public ImageSource CreateImageFromIcon(string fileName)
        {
            ImageSource imageSource = Imaging.CreateBitmapSourceFromHIcon(
                                System.Drawing.Icon.ExtractAssociatedIcon(fileName).Handle,
                                Int32Rect.Empty,
                                BitmapSizeOptions.FromEmptyOptions()
                             );

            return imageSource;
        }

        static public void AddBackFolder(ObservableCollection<FileModel> filesCollection)
        {
            filesCollection.Add(new FileModel()
            {
                FileName = "[..]",
                IsDirectory = true,
                Icon = new BitmapImage(new Uri("/Images/Up.png", UriKind.Relative)),
            });
        }

        static public void AddDrives(ObservableCollection<Drive> drivesCollection)
        {
            DriveInfo[] allDrives = DriveInfo.GetDrives();

            foreach (DriveInfo d in allDrives)
            {
                if (d.IsReady == true)
                {
                    drivesCollection.Add(new Drive() { 
                        Name = d.Name,
                        AvailableFreeSpace = d.AvailableFreeSpace / 1024,
                        TotalFreeSpace = d.TotalFreeSpace/1024,
                        TotalSize = d.TotalSize / 1024,
                        Label = d.VolumeLabel
                    });
                }
            }
        }
    }
}
