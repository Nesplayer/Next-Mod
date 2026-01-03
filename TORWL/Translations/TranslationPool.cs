using System;

namespace TORWL.Translations
{
    public struct TranslationPool
    {
        // Fields marked with ? allow null values
        public string English;
        public string? Spanish;
        public string? French;

        public TranslationPool(string english, string? spanish = null, string? french = null)
        {
            English = english;
            Spanish = spanish;
            French = french;
        }
    }
}