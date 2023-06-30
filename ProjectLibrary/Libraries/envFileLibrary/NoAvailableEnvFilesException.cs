using System;

namespace ProjectLibrary.EnvManager
{
    class NoAvailableEnvFilesException : Exception
    {
        private string NoAvailabletranslationLocation { get; set; }

        private string NoAvailablebackupLocation { get; set; }

        private string NoAvailableworksLocation { get; set; }

        private string NoAvailablebaseLocale { get; set; }

        private string NoAvailableenvPath { get; set; }

        private string logexception;

        public string Logexception(string NoAvailableEnvVariableException)
        {
            this.logexception = DateTime.Now.ToString("dddd, dd MMMM yyyy") + " : " + NoAvailableEnvVariableException + " missing";

            return logexception;
        }
    }
}
