using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace TotalCommander.Model
{
    public class FileModel
    {
        public FileModel()
        {
            Selected = Brushes.Black;
        }

        public string FileName { get; set; }
        public long Size { get; set; }
        public DateTime? Date { get; set; }
        public ImageSource Icon { get; set; }
        public string FullPath { get; set; }
        public bool IsDirectory { get; set; }
        public string Extension { get; set; }
        public string Attributes { get; set; }
        public Brush Selected { get; set; }

        public string FormatedDate
        {
            get
            {
                if (IsDirectory)
                    return "";

                DateTime time = Date ?? DateTime.Now;
                return time.ToString(System.Threading.Thread.CurrentThread.CurrentUICulture);
            }
        }

        public string DisplayedSize
        {
            get
            {
                return this.IsDirectory ? "<DIR>" : Size.ToString();               
            }
        }        
    }
}
