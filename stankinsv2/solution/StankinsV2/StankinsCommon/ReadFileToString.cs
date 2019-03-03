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
                wc.Headers["User-Agent"] = "Mozilla/5.0 (Windows; U; Windows NT 6.0; en-US; rv:1.9.2.6) Gecko/20100625 Firefox/3.6.6 (.NET CLR 3.5.30729)";
                wc.Headers["Accept"] = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
                wc.Headers["Accept-Language"] = "en-us,en;q=0.5";
                //wc.Headers["Accept-Encoding"] = "gzip,deflate";
                wc.Headers["Accept-Charset"] = "ISO-8859-1,utf-8;q=0.7,*;q=0.7";
                var data=await wc.DownloadStringTaskAsync(FileToRead);                
                return data;
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