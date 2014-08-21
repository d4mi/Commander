using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TotalCommander.Services.Files
{
    public class FileOperationService
    {
        public async void Copy(string targetPath, string destinationPath)
        {
            foreach (string filename in Directory.EnumerateFiles(targetPath))
            {
                using (FileStream sourceStream = File.Open(filename, FileMode.Open))
                {
                    using (FileStream destinationStream = File.Create(destinationPath + filename.Substring(filename.LastIndexOf('\\'))))
                    {
                        await sourceStream.CopyToAsync(destinationStream);
                    }
                }
            }
        }

        public void Move(string targetPath, string destinationPath)
        {

        }


    }
}
