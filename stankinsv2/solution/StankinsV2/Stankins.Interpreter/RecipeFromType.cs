using StankinsHelperCommands;
using System.Collections.Generic;
using System.ComponentModel;

namespace Stankins.Interpreter
{
    public class RecipeFromType: Recipe
    {

        public RecipeFromType(ResultTypeStankins r)
        {
            var args=new List<string>();
            Name="Adv_"+r.Name;
            Content = r.Type.FullName;
            foreach(var item in r.ConstructorParam)
            {
                Content+=$" {item.Key}={item.Value}";
                args.Add(item.Key);
            }
            this.Arguments=args.ToArray();
            
            var attr=r.Type.GetCustomAttributes(typeof(DescriptionAttribute),false);
            if(attr?.Length == 1)
            {
                Content=(attr[0] as DescriptionAttribute).Description;
            }
            WhatToList = r.CacheWhatToList;
        }

    }
}