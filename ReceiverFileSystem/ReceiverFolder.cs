using StankinsInterfaces;
using StanskinsImplementation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ReceiverFileSystem
{
    /// <summary>
    /// TODO: add depth of folders to read - infinite , by default...
    /// </summary>
    public class ReceiverFolder : IReceive
    {
        public string Name { get; set; }
        public ReceiverFolder(string folderName, string searchPattern="*")
        {
            FolderName = folderName;
            SearchPattern = searchPattern;
        }

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
            var ret = new List<IRowReceiveHierarchicalParent>();
            //var di = new DirectoryInfo(FolderName);
            var rh=DirectoryRow(di);
            if(parent != null)
                rh.Parent = parent as IRowReceiveHierarchicalParent; 

            ret.Add(rh);

            var files = Files(di, rh);
            
            if(files.Length>0)
                ret.AddRange(files);

            foreach(var dir in di.EnumerateDirectories())
            {
                var filesFolders = RecursiveFromDirectory(dir,rh);
                
                ret.AddRange(filesFolders);
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
