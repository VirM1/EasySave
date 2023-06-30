using System;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;
using ProjectLibrary.FileLibrary;

namespace ProjectLibrary.TranslationLibrary
{


    /* 
     *   Class : TranslationManager 
     *   Usage : Class used to handle all the translation part of the program
     *           More precisely the translation part using key references and translation switch
     */
    public class TranslationManager
    {
        //https://stackoverflow.com/questions/14899422/how-to-navigate-a-few-folders-up
        private Translation translation;


        private readonly string translationLocation;


        /*
         * Constructor : TranslationManager Constructor
         * Usage       : Constructor of the TranslationManager class, takes as a parameter the translationsLocation as well as the default locale
         *               provided by the environment files.
         *               So we set the translationLocation property and initiate a translationChange in order to properly set up a Translation.
         * 
         */
        public TranslationManager(string translationLocation, string locale)
        {
            this.translationLocation = translationLocation;
            this.ChangeTranslation(locale);
        }


        /*
         * Function : Translate(string key, params string args)
         * Usage    : Access the object Translation and proceeeds to Translate using the key provided and also the
         *            optional arguments if given.
         *            Also Handles the MissingTranslationException.
         * 
         */
        public string Translate(string key, params string[] args)
        {
            try
            {
                if (this.translation != null)
                {
                    return this.translation.Translate(key, args);
                }
                else
                {//todo @virgile � voir
                    throw new Exception();
                }
            }
            catch (MissingTranslationException)
            {
                if (!key.Equals("exception.missingTranslation"))
                {
                    return this.Translate("exception.missingTranslation", key);
                }
                else
                {
                    throw;
                }
            }
        }



        /*
         * Function : GetAvailableTranslations()
         * Usage    : Gets all the available translations fileNames and returns the said translations in a List (without the file extension)
         * 
         */
        public List<string> GetAvailableTranslations()
        {
            List<string> fileNames = FileManager.GetAvailableFileNamesRegex(this.translationLocation, "*.json");
            List<string> translations = new List<string>();
            int index;
            foreach(string fileName in fileNames)
            {
                index = fileName.IndexOf(".");
                translations.Add(fileName.Substring(0, index));
            }
            return translations; 
        }

        /*
         * Function : GetTranslation(string newLocale)
         * Usage    : Obtains a Translation object by opening and Parsing a translation file (json).
         * 
         */
        private Translation? GetTranslation(string newLocale)
        {
            Translation? translation = null;
            try
            {
                JsonElement jsonContent = this.FindAndReturnTranslation(newLocale);
                translation = new Translation(jsonContent, newLocale);
            }
            catch (Exception e)
            {
                Console.WriteLine(this.VoidHandleTranslationChangeException(e, newLocale));
            }

            return translation;
        }

        /*
         * Method   : ChangeTranslation(string newLocale)
         * Usage    : Tries to change translation with the new provided Locale if it could be found and is a valid JSON file.
         * 
         */
        public void ChangeTranslation(string newLocale)
        {

            Translation? translation = this.GetTranslation(newLocale);
            if (translation != null)
            {
                this.translation = translation;
            }
        }

        /*
         * Function : VoidHandleTranslationChangeException(Exception exception, string newLocale)
         * Usage    : Handles the different translation change exceptions returning a message if the exception is handled.
         * 
         */
        private string VoidHandleTranslationChangeException(Exception exception, string newLocale)
        {
            return exception switch
            {
                FileNotFoundException _ => this.Translate("exception.filenotfound", newLocale),
                DirectoryNotFoundException _ => this.Translate("exception.directorynotfound", newLocale),
                JsonException _ => this.Translate("exception.json", newLocale),
                _ => throw exception,
            };
        }

        /*
         * Function : FindAndReturnTranslation(string locale)
         * Usage    : Tries to find and return a Parsed Json file from the given locale.
         * 
         */
        private JsonElement FindAndReturnTranslation(string locale)
        {
            string filePath = String.Format("{0}.json", locale);
            filePath = Path.Combine(this.translationLocation, filePath);
            string stream = FileManager.GetStream(filePath);
            JsonDocument json = JsonDocument.Parse(stream);
            return json.RootElement;
        }

        public string GetLanguage()
        {
            return this.translation.Locale;
        }
    }
}