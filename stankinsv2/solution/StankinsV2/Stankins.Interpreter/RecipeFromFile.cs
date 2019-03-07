using System;
using System.IO;
using System.Linq;

namespace Stankins.Interpreter
{
    public class RecipeFromFile:Recipe
    {
        public string fileName{get;internal set; }

        public RecipeFromFile(string fileName )
        {
            Name=Path.GetFileNameWithoutExtension(fileName);
            Content= File.ReadAllText(fileName);
            Description =string.Join(Environment.NewLine, File.ReadLines(fileName).Where(it=>it.StartsWith("#")).ToArray());
            this.fileName = fileName;
            var lines=Content.Split(Environment.NewLine,StringSplitOptions.RemoveEmptyEntries);
        }
    }
}