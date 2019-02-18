using System;
using System.IO;
using System.Linq;

namespace StankinsHelperCommands
{
    public class RecipeText
    {
        public RecipeText()
        {

        }
        public RecipeText(string fileName )
        {
            RecipeContent= File.ReadAllText(fileName);
            Description =string.Join(Environment.NewLine, File.ReadLines(fileName).Where(it=>it.StartsWith("#")).ToArray());
        }

        public string RecipeContent { get; set; }
        public string Description { get; set; }
    }
}