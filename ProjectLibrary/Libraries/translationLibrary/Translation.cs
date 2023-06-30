using System;
using System.Collections.Generic;
using System.Text.Json;

namespace ProjectLibrary.TranslationLibrary
{

    /* 
     *   Class : Translation 
     *   Usage : Class representing a translation
     */
    public class Translation
    {
        private readonly JsonElement jsonContent;

        private string locale;


        /*
         * Constructor : Translation Constructor
         * Usage       : Constructor of the Translation class, takes as a parameter the parsed content of a JSON file as well as the corresponding locale.
         * 
         */
        public Translation(JsonElement jsonContent, string locale)
        {
            this.jsonContent = jsonContent;
            this.locale = locale;
        }

        public string Locale { get => locale; set => locale = value; }

        /*
         * Function    : Translate(string key, params string[] args)
         * Usage       : Function that accesses the parsed stored JSON Content and tries to access the given key, if it is found at the end of the loop we check if it is a string
         *               in the opposite case: if it isnt a string or if the key wasn't found at the end of the loop, we throw a MissingTranslationException.
         *               And in the case we found the said key, therefore the corresponding key, we format the string and add the optional arguments (to fill the {0},{1}...Etc)
         * 
         */
        public string Translate(string key, params string[] args)
        {
            List<string> keyList = new List<string>(key.Split('.'));
            int count = keyList.Count;
            int index = 0;
            JsonElement currentElement = this.jsonContent;
            foreach(string keyElement in keyList)
            {
                index++;
                if (currentElement.TryGetProperty(keyElement,out currentElement))
                {
                    if (index.Equals(count) && !currentElement.ValueKind.Equals(JsonValueKind.String))
                    {
                        throw new MissingTranslationException();
                    }
                }
                else
                {
                    throw new MissingTranslationException();
                }
            }

            return String.Format(currentElement.ToString(),args);
        }
    }
}