using System;
using System.Linq;
using Stankins.Interfaces;
using StankinsCommon;
using StankinsObjects;

namespace StankinsHelperCommands
{
    public class ResultTypeStankins
    {
        public ResultTypeStankins(Type type,CtorDictionary constructorParam)
        {
            Type = type;
            ConstructorParam = constructorParam;
            CacheWhatToList = FromType();
        }

        public string Name
        {
            get { return Type.Name; }
        }
        public WhatToList CacheWhatToList { get; private set; } 
        public Type Type { get;  }
        public CtorDictionary ConstructorParam { get; }

        public WhatToList FromType()
        {
            if (CacheWhatToList != WhatToList.None)
                return CacheWhatToList;

            if (typeof(IReceive).IsAssignableFrom(Type))
            {
                CacheWhatToList |= WhatToList.Receivers;
            }
            if (typeof(ISender).IsAssignableFrom(Type))
            {
                CacheWhatToList |= WhatToList.Senders;
            }
            if (typeof(IFilter).IsAssignableFrom(Type))
            {
                CacheWhatToList |= WhatToList.Filters;
            }
            if (typeof(ITransformer).IsAssignableFrom(Type))
            {
                CacheWhatToList |= WhatToList.Transformers;
            }
            
            return CacheWhatToList;
        }
        
        public BaseObject Create(in object[] ctorStrings)
        {
            var nrArgs = (ctorStrings?.Length ?? 0);
            if (nrArgs != ConstructorParam.Count())
            {
                throw new ArgumentException($"number of args {ConstructorParam.Count} != {nrArgs}");
            }

            BaseObject act;
            if (ctorStrings?.Length > 0)
            {
                act = Activator.CreateInstance(Type, ctorStrings) as BaseObject;

            }
            else
            {
                act = Activator.CreateInstance(Type) as BaseObject;

            }

            return act;
        }
    }
}