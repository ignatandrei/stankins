using Stankins.Interfaces;
using StankinsCommon;
using StankinsHelperCommands;
using StankinsObjects;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Stankins.Interpreter
{
    public class RecipeFromString : BaseObjectInSerial, IReceive
    {
        private static readonly object lockMe = new object();
        private static Recipe[] recipesFromFolder = null;
        private static Recipe[] recipesFromBaseObjects = null;
        public static RecipeFromString FindRecipe(string name)
        {
            Func<Recipe,string,bool> find=(it,found) => string.Equals(it.Name, found, StringComparison.InvariantCultureIgnoreCase);
            Recipe recipe = RecipesFromFolder().FirstOrDefault(it=>find(it,name));
            if (recipe == null)
            {
                recipe= recipesFromBaseObjects.FirstOrDefault(it=>find(it,name));
                if (recipe == null)
                {
            
                    System.Console.Error.WriteLine($"can not found {name}");
                    return null;//maybe throw the error?
                }
            }

            return new RecipeFromString(recipe.Content);
        }
        public static Recipe[] RecipesFromBaseObjects()
        {

            if (recipesFromBaseObjects != null)
            {
                return recipesFromBaseObjects;
            }
            lock (lockMe)
            {
                if (recipesFromBaseObjects != null)
                {
                    return recipesFromBaseObjects;
                }
                recipesFromBaseObjects = FindAssembliesToExecute.AddReferences().Select(it => new RecipeFromType(it)).ToArray();
                return recipesFromBaseObjects;
            }
        }
        public static Recipe[] RecipesFromFolder()
        {
            if (recipesFromFolder != null)
            {
                return recipesFromFolder;
            }
            lock (lockMe)
            {
                if (recipesFromFolder != null)
                {
                    return recipesFromFolder;
                }
                string folderRecipes = "Recipes";
                if (!Directory.Exists(folderRecipes))
                {
                    string pathDll = Assembly.GetEntryAssembly().Location;
                    string path = Path.GetDirectoryName(pathDll);
                    folderRecipes = Path.Combine(path, folderRecipes);
                }
                folderRecipes = Path.Combine(folderRecipes, "v1");
                string[] files = Directory.GetFiles(folderRecipes, "*.txt");
                recipesFromFolder = files.Select(it => new RecipeFromFile(it)).ToArray();



                return recipesFromFolder;
            }
        }
        private readonly string content;

        public RecipeFromString(CtorDictionary dataNeeded) : base(dataNeeded)
        {
            content = GetMyDataOrThrow<string>(nameof(content));
        }

        public RecipeFromString(string content) : this(new CtorDictionary()
        {
            {nameof(content), content}
        })
        {

        }

        public override Task<IDataToSent> TransformData(IDataToSent receiveData)
        {
            string[] lines = content.Split(Environment.NewLine);
            foreach (string line in lines)
            {
                string str = line.Trim();
                if (string.IsNullOrWhiteSpace(str))
                {
                    continue;
                }

                if (line.Trim().StartsWith("#"))
                {
                    continue;
                }

                IInterpreter i = new InterpretFromType();
                if (!i.CanInterpretString(line.Trim()))
                {
                    System.ComponentModel.DataAnnotations.ValidationResult v = i.Validate(null).First();
                    throw new ArgumentException(v.ErrorMessage);
                }
                //validate from prev shows errors and warnings
                System.ComponentModel.DataAnnotations.ValidationResult[] ValidWarnings = i.Validate(null).ToArray();
                if (ValidWarnings?.Length > 0)
                {
                    foreach (System.ComponentModel.DataAnnotations.ValidationResult item in ValidWarnings)
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