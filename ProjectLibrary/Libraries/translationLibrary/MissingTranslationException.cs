using System;

namespace ProjectLibrary.TranslationLibrary
{
    class MissingTranslationException : Exception
    {
        //todo @Virgile

        private string missingtranslation;

        public string Missingtranslationexception()
        {
            this.missingtranslation = DateTime.Now.ToString("dddd, dd MMMM yyyy") + ": translation missing";

            return missingtranslation;
        }
    }
}
