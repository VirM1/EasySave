using System;

namespace ProjectLibrary.EnvManager
{
    public class MissingEnvVariableException : Exception
    {

        private string missingtranslationLocation { get; set; }

        private string missingbackupLocation { get; set; }

        private string missingworksLocation { get; set; }

        private string missingbaseLocale { get; set; }

        private string missingenvPath { get; set; }

        private string logexception;

        public string Logexception(string missingEnvVariableException)
        {
            this.logexception = DateTime.Now.ToString("dddd, dd MMMM yyyy") + " : " + missingEnvVariableException + " missing";

            return logexception;
        }

        public void Checkmissingvariable(string translationLocation, string backupLocation, string worksLocation, string logLocation, string baseLocale, string envPath)
        {
            if(translationLocation == "")
            {
                Logexception("translationLocation");
            }

            if (backupLocation == "")
            {
                Logexception("backupLocation");
            }

            if (worksLocation == "")
            {
                Logexception("worksLocation");
            }

            if (logLocation == "")
            {
                Logexception("logLocation");
            }

            if (baseLocale == "")
            {
                Logexception("baseLocale");
            }

            if (envPath == "")
            {
                Logexception("envPath");
            }
        }
    }
}