using StankinsInterfaces;
using StanskinsImplementation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Transformers;
using Microsoft.Extensions.Logging;
namespace ReceiverFileSystem
{
    public class FilterForFoldersHierarchical : FilterForHierarchical
    {
        public FilterForFoldersHierarchical() : base(new FilterComparableEqual(typeof(string), "folder", "RowType"))
        {
        }
    }
    public class FilterForFilesHierarchical : FilterForHierarchical
    {
        public FilterForFilesHierarchical() : base(new FilterComparableEqual(typeof(string), "file", "RowType"))
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
            try
            {
                var rh = DirectoryRow(di);
                if (parent != null)
                    rh.Parent = parent as IRowReceiveHierarchicalParent;

                ret.Add(rh);

                var files = Files(di, rh);

                if (files?.Length > 0)
                    ret.AddRange(files);
                rh.Values.Add("nrfiles", files?.Length);

                foreach (var dir in di.EnumerateDirectories())
                {
                    var filesFolders = RecursiveFromDirectory(dir, rh);
                    if (filesFolders?.Length > 0)
                        ret.AddRange(filesFolders.Where(it => it != null).ToArray());
                }
            }
            catch(UnauthorizedAccessException ex)
            {
                string message = ex.Message;
                //@class.Log(LogLevel.Warning,0,$"unauthorized {message}",ex,null);
                message += "text";

            }
            return ret.ToArray();
        }
        RowReadHierarchical[] Files(DirectoryInfo di, IRowReceive parent)
        {
            var ret = new List<RowReadHierarchical>();
            foreach (var searchSplitPattern in SearchPattern.Split(new char[] { ';' },StringSplitOptions.RemoveEmptyEntries))
            {
                foreach (var file in di.EnumerateFiles(searchSplitPattern))
                {
                    var item = new RowReadHierarchical();
                    item.Values.Add("Name", file.Name);
                    item.Values.Add("FullName", file.FullName);
                    item.Values.Add("RowType", "file");
                    item.Values.Add("CreationTimeUtc", file.CreationTimeUtc);
                    item.Values.Add("LastAccessTimeUtc", file.LastAccessTimeUtc);
                    item.Values.Add("LastWriteTimeUtc", file.LastWriteTimeUtc);
                    item.Parent = parent as IRowReceiveHierarchicalParent;
                    ret.Add(item);
                }
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
            item.Values.Add("CreationTimeUtc", di.CreationTimeUtc);
            item.Values.Add("LastAccessTimeUtc", di.LastAccessTimeUtc);
            item.Values.Add("LastWriteTimeUtc", di.LastWriteTimeUtc);
            return item;
        }
        

    }
}
