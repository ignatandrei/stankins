using System;
using System.IO;
using System.Linq;

namespace Stankins.Interpreter
{
    public class RecipeText
    {
        public readonly string fileName;

        public RecipeText()
        {

        }
        public RecipeText(string fileName )
        {
            Name=Path.GetFileNameWithoutExtension(fileName);
            RecipeContent= File.ReadAllText(fileName);
            Description =string.Join(Environment.NewLine, File.ReadLines(fileName).Where(it=>it.StartsWith("#")).ToArray());
            this.fileName = fileName;
        }
        public string Name { get; set; }
        public string RecipeContent { get; set; }
        public string Description { get; set; }
    }
}