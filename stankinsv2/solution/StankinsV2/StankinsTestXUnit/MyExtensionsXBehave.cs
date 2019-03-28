using System;
using Xbehave;
using Xbehave.Sdk;

namespace StankinsTestXUnit
{
    static class MyExtensionsXBehave
    {
        public static IStepBuilder w(this string text, Action body)
        {
            //Console.WriteLine("!" + text);
            return text.x(body);
        }
    }
}
