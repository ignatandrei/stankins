using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Stankins.Interfaces;
using StankinsCommon;
using StankinsObjects;

namespace Stankins.Interpreter
{
    public class RecipeFromString : BaseObjectInSerial, IReceive
    {
        static RecipeText[] recipes = null;
        public static RecipeFromString FindRecipe(string name)
        {
            var recipe = RecipeFromString.Recipes().FirstOrDefault(it => string.Equals(it.Name, name, StringComparison.InvariantCultureIgnoreCase));
            if (recipe == null)
            {
                System.Console.Error.WriteLine($"can not found {name}");
                return null;//maybe throw the error?
            }
            return new RecipeFromString(recipe.RecipeContent);
            

        }
        public static RecipeText[] Recipes()
        {
            if (recipes != null)
            {
                return recipes;
            }
            string folderRecipes = "Recipes";
            if (!Directory.Exists(folderRecipes))
            {
                var pathDll = Assembly.GetEntryAssembly().Location;
                var path = Path.GetDirectoryName(pathDll);
                folderRecipes = Path.Combine(path, folderRecipes);
            }
            folderRecipes = Path.Combine(folderRecipes, "v1");
            var files = Directory.GetFiles(folderRecipes, "*.txt");
            recipes = files.Select(it => new RecipeText(it)).ToArray();

            return recipes;

        }
        private readonly string content;

        public RecipeFromString(CtorDictionary dataNeeded) : base(dataNeeded)
        {
            this.content = GetMyDataOrThrow<string>(nameof(content));
        }

        public RecipeFromString(string content) : this(new CtorDictionary()
        {
            {nameof(content), content}
        })
        {

        }

        public override Task<IDataToSent> TransformData(IDataToSent receiveData)
        {
            var lines = content.Split(Environment.NewLine);
            foreach (var line in lines)
            {
                if (line.Trim().StartsWith("#"))
                    continue;
                IInterpreter i = new InterpretFromType();
                if (!i.CanInterpretString(line.Trim()))
                {
                    var v = i.Validate(null).First();
                    throw new ArgumentException(v.ErrorMessage);
                }
                //validate from prev shows errors and warnings
                var ValidWarnings = i.Validate(null).ToArray();
                if (ValidWarnings?.Length > 0)
                {
                    foreach (var item in ValidWarnings)
                    {
                        Console.WriteLine(item.ErrorMessage);
                    }
                }
                base.AddType(i.ObjectType.Type, i.ObjectType.ConstructorParam);
            }
            return base.TransformData(receiveData);
        }
    }
}