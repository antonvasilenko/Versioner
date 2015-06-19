using System;
using JetBrains.Annotations;

namespace Versioner
{
    public static class Lo
    {
        public static Verbosity Verbosity { get; set; }

        [StringFormatMethod("template")]
        public static void Data(string template, params object[] args)
        {
            if(CanWrite(Verbosity, Verbosity.Data))
                Console.WriteLine(template, args);
        }

        [StringFormatMethod("template")]
        public static void Log(string template, params object[] args)
        {
            if (CanWrite(Verbosity, Verbosity.Normal))
                Console.Write(template, args);
        }

        [StringFormatMethod("template")]
        public static void Details(string template, params object[] args)
        {
            if (CanWrite(Verbosity, Verbosity.Detailed))
                Console.Write(template, args);
        }

        private static bool CanWrite(Verbosity current, Verbosity given)
        {
            return ((int) given) <= ((int) current);
        }
    }
}