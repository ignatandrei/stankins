using System;

namespace StankinsRecipes
{
    [Flags]
    public enum WhatToList
    {
        None=0,
        Receivers=0x1,
        Senders=0x2,
        Transformers=0x4,
        Filters=0x8,
        
        
        RecipeSimple= Receivers | Senders
    }
}