using System;
using System.Collections.ObjectModel;
using TotalCommander.Model;
namespace TotalCommander.Services.Sorting
{
    public interface IFileSorter
    {
        ObservableCollection<FileModel> SortBy(ObservableCollection<FileModel> collection,
            Func<FileModel, string> condition,
            bool asc = true);
    }
}
