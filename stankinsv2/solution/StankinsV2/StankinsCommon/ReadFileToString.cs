using System;
using System.IO;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace StankinsCommon
{
    public enum FileType
    {
        None=0,
        Local,
        Web
    }
    /// <summary>
    /// reads from local or reads from web
    /// TODO : read from FTP
    /// </summary>
    public class ReadFileToString
    {
        public FileType FileType { get; private set; }
        public string FileToRead { get; set; }
        public Encoding FileEnconding { get;  set; }

        bool IsLocalFile()
        {
            if(FileType != FileType.None)
            {
                return FileType == FileType.Local;
            }
            try
            {
                var uri = new Uri(FileToRead, UriKind.RelativeOrAbsolute);
                if (uri.IsFile)
                {
                    FileType = FileType.Local;
                    return true;
                }
                FileType = FileType.Web;
                return false;
            }
            catch (Exception)
            {
                //if it is local file, 
                //then uri.IsFile will send error
                FileType = FileType.Local;
                return true;
            }
        }
        public async Task<string> LoadData()
        {
            var b = IsLocalFile();
            if (b)
            {
                return await LoadLocalFile();

            }
            //web
            return await LoadFromWeb();
            //todo : ftp ?

        }

        private async Task<string> LoadFromWeb()
        {
            using (var wc = new WebClient())
            {                
                return  await wc.DownloadStringTaskAsync(FileToRead);                
            }
        }

        private async Task<string> LoadLocalFile()
        {
            using (var stream = File.Open(FileToRead, FileMode.OpenOrCreate, FileAccess.Read))
            {
                using (var reader = new StreamReader(stream, FileEnconding))
                {


                    return await reader.ReadToEndAsync();

                }
            }

        }
    }
}