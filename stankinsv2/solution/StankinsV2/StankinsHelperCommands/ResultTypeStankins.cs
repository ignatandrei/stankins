using Stankins.Interfaces;
using StankinsCommon;
using StankinsObjects;
using System;
using System.Diagnostics;
using System.Linq;

namespace StankinsHelperCommands
{
    [DebuggerDisplay("{Name}")]
    public class ResultTypeStankins
    {
        public ResultTypeStankins(Type type, CtorDictionary constructorParam)
        {
            Type = type;
            ConstructorParam = constructorParam;
            CacheWhatToList = FromType();
        }

        public string Name => Type.Name;
        public WhatToList CacheWhatToList { get; private set; }
        public Type Type { get; }
        public CtorDictionary ConstructorParam { get; }

        public WhatToList FromType()
        {
            if (CacheWhatToList != WhatToList.None)
            {
                return CacheWhatToList;
            }

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
            int nrArgs = (ctorStrings?.Length ?? 0);
            if (nrArgs != ConstructorParam.Count())
            {
                Console.WriteLine($" for {Type.Name} found number of args {ConstructorParam.Count} != {nrArgs}");
            }
            System.Collections.Generic.KeyValuePair<string, object>[] arr = ConstructorParam.ToArray();

            BaseObject act;
            if (ctorStrings?.Length > 0)
            {
                try
                {
                    act = Activator.CreateInstance(Type, ctorStrings) as BaseObject;
                }
                catch (Exception)
                {
                    //TODO: log
                    object[] ctorTypes = new object[nrArgs];
            
                    for (int i = 0; i < ctorStrings.Length; i++)
                    {
                        Type type = arr[i].Value.GetType();
                        ctorTypes[i] = Convert.ChangeType(ctorStrings[i], type);
                    }
                    act = Activator.CreateInstance(Type, ctorTypes) as BaseObject;
                

                }
            }
            else
            {
                try{
                act = Activator.CreateInstance(Type) as BaseObject;
                }
                catch (Exception)
                {
                    act = Activator.CreateInstance(Type, new CtorDictionary()) as BaseObject;
                }
            }

            return act;
        }
    }
}