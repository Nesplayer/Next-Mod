using System;

namespace TORWL.Translations
{
    public struct TranslationPool
    {
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