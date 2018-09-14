using System;
using System.IO;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace StankinsCommon
{
    /// <summary>
    /// reads from local or reads from web
    /// TODO : read from FTP
    /// </summary>
    public class ReadFileToString
    {
        public string FileToRead { get; set; }
        public Encoding FileEnconding { get;  set; }

        bool IsLocalFile()
        {
            try
            {
                var uri = new Uri(FileToRead, UriKind.RelativeOrAbsolute);
                return uri.IsFile;
            }
            catch (Exception)
            {
                //if it is local file, 
                //then uri.IsFile will send error
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