using StankinsInterfaces;
using StanskinsImplementation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReceiverFileSystem
{
    public class ReceiverFolderRelational : IReceive
    {
        public string Name { get; set; }
        public ReceiverFolderRelational(string folderName, string searchPattern = "*")
        {
            FolderName = folderName;
            SearchPattern = searchPattern;
        }
        public string[] ExcludeFolderNames { get; set; }
        public string FolderName { get; set; }
        public string SearchPattern { get; set; }
        public IRowReceive[] valuesRead { get; set; }
        public async Task LoadData()
        {
            valuesRead =new IRowReceive[] { RecursiveFromDirectory(new DirectoryInfo(FolderName)) };
            await Task.CompletedTask;
        }
        IRowReceiveRelation RecursiveFromDirectory(DirectoryInfo di)
        {
            if (ExcludeFolderNames?.Length > 0)
            {
                //TODO: case insensitive
                if (ExcludeFolderNames.Contains(di.Name))
                    return null;
            }
            var rh = DirectoryRow(di);
            rh.Relations.Add("files",Files(di));
            var folders = new List<IRowReceiveRelation>();
            rh.Relations.Add("folders", folders);
            foreach (var folder in di.EnumerateDirectories())
            {
                var dir = RecursiveFromDirectory(folder);
                if (dir != null)
                    folders.Add(dir);
            }
            return rh;
        }
        List<IRowReceiveRelation> Files(DirectoryInfo di)
        {
            var ret = new List<IRowReceiveRelation>();

            foreach (var file in di.EnumerateFiles(SearchPattern))
            {
                var item = new RowReadRelation();
                item.Values.Add("Name" , file.Name);
                item.Values.Add("FullName", file.FullName);
                item.Values.Add("RowType", "file");
                item.Values.Add("CreationTimeUtc", file.CreationTimeUtc);
                item.Values.Add("LastAccessTimeUtc", file.LastAccessTimeUtc);
                item.Values.Add("LastWriteTimeUtc", file.LastWriteTimeUtc);
                ret.Add(item);
            }
            return ret;
        }
        RowReadRelation DirectoryRow(DirectoryInfo di)
        {
            var item = new RowReadRelation();
            //DirectoryInfo di = new DirectoryInfo(FolderName);
            item.Values.Add("Name", di.Name);
            item.Values.Add("FullName", di.FullName);
            item.Values.Add("RowType", "folder");
            item.Values.Add("CreationTimeUtc", di.CreationTimeUtc);
            item.Values.Add("LastAccessTimeUtc", di.LastAccessTimeUtc);
            item.Values.Add("LastWriteTimeUtc", di.LastWriteTimeUtc);

            return item;
        }
    }

    
}
