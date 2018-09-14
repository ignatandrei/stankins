using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace StankinsTests
{
    class UtilsIO
    {
        public static string DeleteCreateFolder(string dir)
        {
            if (Directory.Exists(dir))
                Directory.Delete(dir, true);
            Directory.CreateDirectory(dir);
            return dir;
        }
    }
}
