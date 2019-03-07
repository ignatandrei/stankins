using StankinsHelperCommands;
using StankinsObjects;

namespace Stankins.Interpreter
{
    public class Recipe
    {
        public string Name{get;set;}
        public string Content{get;set;}
        public string Description{get;set;}
        public string[] Arguments {get;set;}
        public WhatToList? WhatToList{get;set;}
    }
}