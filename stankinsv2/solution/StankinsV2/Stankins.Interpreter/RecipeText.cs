using StankinsHelperCommands;
using StankinsObjects;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace Stankins.Interpreter
{
    public class Recipe
    {
        public string Name{get;set;}
        public string Content{get;set;}
        public string Description{get;set;}
    }

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

    public class RecipeFromType: Recipe
    {

        public RecipeFromType(ResultTypeStankins r)
        {
            Name=r.Name;
            Content = r.Name;
            foreach(var item in r.ConstructorParam)
            {
                Content+=$"{item.Key}={item.Value} ";
            }
            Content=r.Type.FullName;
            var attr=r.Type.GetCustomAttributes(typeof(DescriptionAttribute),false);
            if(attr?.Length == 1)
            {
                Content=(attr[0] as DescriptionAttribute).Description;
            }

        }

    }
}