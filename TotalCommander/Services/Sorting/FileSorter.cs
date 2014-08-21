using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TotalCommander.Model;

namespace TotalCommander.Services.Sorting
{
    public class FileSorter : TotalCommander.Services.Sorting.IFileSorter
    {
        public ObservableCollection<FileModel> SortBy(ObservableCollection<FileModel> collection, 
            Func<FileModel, string> condition,
            bool asc = true)
        {
            List<FileModel> sortedCollection = new List<FileModel>();
            RemoveBackFile(ref collection, ref sortedCollection);
            if (!asc)
            {
                sortedCollection.AddRange(collection.Where(x => x.IsDirectory).OrderByDescending(condition).ToList());
                sortedCollection.AddRange(collection.Where(x => !x.IsDirectory).OrderByDescending(condition).ToList());
            }
            else
            {
                sortedCollection.AddRange(collection.Where(x => x.IsDirectory).OrderBy(condition).ToList());
                sortedCollection.AddRange(collection.Where(x => !x.IsDirectory).OrderBy(condition).ToList());
            }
            
            return new ObservableCollection<FileModel>(sortedCollection);
        }

        private void RemoveBackFile(ref ObservableCollection<FileModel> collection,
            ref List<FileModel> sortedCollection)
        {
            for (int i = 0; i < collection.Count(); ++i )
            {
                if (collection.ElementAt(i).FileName == "[..]")
                {
                    FileModel back = collection.ElementAt(i);
                    sortedCollection.Add(back);
                    collection.Remove(back);
                }
            }
        }
    }
}
