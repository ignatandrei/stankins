using StankinsInterfaces;
using StanskinsImplementation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Transformers;

namespace ReceiverFileSystem
{
    public class FilterForFoldersHierarchical : FilterComparableEqual
    {
        public FilterForFoldersHierarchical() : base(typeof(string), "folder", "RowType")
        {
        }
    }
    public class FilterForFilesHierarchical : FilterComparableEqual
    {
        public FilterForFilesHierarchical() : base(typeof(string), "file", "RowType")
        {
        }
    }
    /// <summary>
    /// TODO: add depth of folders to read - infinite , by default...
    /// </summary>
    public class ReceiverFolderHierarchical : IReceive
    {
        public string Name { get; set; }
        public ReceiverFolderHierarchical(string folderName, string searchPattern="*")
        {
            FolderName = folderName;
            SearchPattern = searchPattern;
            Name = $"Receiver from folder {folderName} with {searchPattern}";
        }
        public string[] ExcludeFolderNames { get; set; }
        public string FolderName { get; set; }
        public string SearchPattern { get; set; }
        
        public IRowReceive[] valuesRead { get; set; }

        public async Task LoadData()
        {
           
            
            valuesRead= RecursiveFromDirectory(new DirectoryInfo(FolderName),null);
            await Task.CompletedTask;
        }

        IRowReceiveHierarchicalParent[] RecursiveFromDirectory(DirectoryInfo di,IRowReceive parent)
        {
            if(ExcludeFolderNames?.Length>0)
            {
                //TODO: case insensitive
                if (ExcludeFolderNames.Contains(di.Name)) 
                    return null;
            }
            
            var ret = new List<IRowReceiveHierarchicalParent>();
            //var di = new DirectoryInfo(FolderName);
            var rh=DirectoryRow(di);
            if(parent != null)
                rh.Parent = parent as IRowReceiveHierarchicalParent; 

            ret.Add(rh);

            var files = Files(di, rh);
            
            if(files?.Length>0)
                ret.AddRange(files);
            rh.Values.Add("nrfiles", files?.Length);

            foreach(var dir in di.EnumerateDirectories())
            {
                var filesFolders = RecursiveFromDirectory(dir,rh);
                if(filesFolders?.Length>0)
                    ret.AddRange(filesFolders.Where(it=>it!=null).ToArray());
            }
            return ret.ToArray();
        }
        RowReadHierarchical[] Files(DirectoryInfo di, IRowReceive parent)
        {
            var ret = new List<RowReadHierarchical>();
            
            foreach(var file in di.EnumerateFiles(SearchPattern))
            {
                var item = new RowReadHierarchical();
                item.Values.Add("Name", file.Name);
                item.Values.Add("FullName", file.FullName);
                item.Values.Add("RowType", "file");
                item.Parent = parent as IRowReceiveHierarchicalParent;
                ret.Add(item);
            }
            return ret.ToArray();
        }
        RowReadHierarchical DirectoryRow(DirectoryInfo di)
        {
            var item = new RowReadHierarchical();
            //DirectoryInfo di = new DirectoryInfo(FolderName);
            item.Values.Add("Name", di.Name);
            item.Values.Add("FullName", di.FullName);
            item.Values.Add("RowType", "folder");
            return item;
        }
        

    }
}
