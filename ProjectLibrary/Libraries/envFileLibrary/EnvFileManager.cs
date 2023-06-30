using System;
using System.IO;
using ProjectLibrary.FileLibrary;
using System.Text.Json;
using System.Text.Json.Serialization;
using ProjectLibrary.LogLibrary;

namespace ProjectLibrary.EnvManager
{

    /* 
     *   Class : EnvManager 
     *   Usage : Class used to retrieve the environment variables from the environment files
     *           possibly env.local.json (if present) or by default env.json
     */
    public class EnvFileManager
    {

        private string translationLocation;

        private string backupLocation;

        private string worksLocation;

        private string priorityLocation;

        private string logLocation;

        private string forbiddenFilesLocation;

        private string baseLocale;

        private string envPath;

        private LogExtension logextension;

        private int bigFileSize;

        private string cryptoSoftLocation;

        private string cryptedExtensionsLocation;

        private JsonSerializerOptions _serializerOptions;


        /*
         * Constructor : EnvManager Constructor
         * Usage       : Constructor of the EnvManager class, takes as a parameter the environment path,
         *               after that, we determine which envFile is available then open the said file and retrieve all the needed variables.
         * 
         * 
         */
        public EnvFileManager(string envPath)
        {
            this.envPath = envPath;
            OpenEnvFileAndGiveVariables(DeterminateWhichEnvFile(envPath));
            this._serializerOptions = new JsonSerializerOptions()
            {
                WriteIndented = true,
                Converters =
                {
                            new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
                }
            };
        }

        public string TranslationLocation { 
            get => translationLocation;
            set
            { translationLocation = value;
            }
        }
        public string BackupLocation { 
            get => backupLocation;
            set
            { backupLocation = value;
            }
        }
        public string WorksLocation { 
            get => worksLocation;
            set
            { worksLocation = value;
            }
        }
        public string LogLocation { 
            get => logLocation;
            set
            { logLocation = value;
            }
        }
        public string ForbiddenFilesLocation { 
            get => forbiddenFilesLocation;
            set
            { forbiddenFilesLocation = value;
            }
        }

        public string BaseLocale { 
            get => baseLocale;
            set
            { baseLocale = value;
            }
        }

        public LogExtension LogExtension { 
            get => logextension;
            set
            { logextension = value;
            }
        }

        public string PriorityLocation { get => priorityLocation; set => priorityLocation = value; }
        public int BigFileSize { get => bigFileSize; set => bigFileSize = value; }
        public string CryptoSoftLocation { get => cryptoSoftLocation; set => cryptoSoftLocation = value; }
        public string CryptedExtensionsLocation { get => cryptedExtensionsLocation; set => cryptedExtensionsLocation = value; }

        public void updateOrCreateEnvFile()
        {
            string parsedJson = JsonSerializer.Serialize(this, this._serializerOptions);
            FileManager.WriteAndSaveFile("env.local.json", parsedJson);
        }


        /*
         * Function : DeterminateWhichEnvFile(string path)
         * Usage    : Determines and returns the path of the most available path (env.local.json or env.json if it is absent) using the environmentPath given
         *            if none are present then we throw an NoAvailableEnvFilesException since we need the environment variables to continue the program.
         * 
         */
        private string DeterminateWhichEnvFile(string path)
        {
            string returnedPath = Path.Combine(path, "env.local.json");
            if (File.Exists(returnedPath))
            {
                return returnedPath;
            }
            returnedPath = Path.Combine(path, "env.json");
            if (File.Exists(Path.Combine(path, "env.json")))
            {
                return returnedPath;
            }
            throw new NoAvailableEnvFilesException();
        }


        /*
         * Method : OpenEnvFileAndGiveVariables(string propertiesPath)
         * Usage  : Tries to set all the different environment variables while opening the environment file (using the provided path)
         *          if one is absent we inform the user that a variable is absent and we throw a MissingEnvVariableException
         * 
         * 
         */
        private void OpenEnvFileAndGiveVariables(string propertiesPath)
        {
            string stream = FileManager.GetStream(propertiesPath);
            JsonElement data = (JsonDocument.Parse(stream)).RootElement;

            try
            {
                this.TranslationLocation = this.GetPathEnvVariable(data, "TranslationLocation"); 
                this.BackupLocation = this.GetPathEnvVariable(data, "BackupLocation");
                this.WorksLocation = this.GetPathEnvVariable(data, "WorksLocation");
                this.LogLocation = this.GetPathEnvVariable(data, "LogLocation");
                this.ForbiddenFilesLocation = this.GetPathEnvVariable(data, "ForbiddenFilesLocation");
                this.PriorityLocation = this.GetPathEnvVariable(data, "PriorityLocation");
                this.BaseLocale = GetVariable(data,"BaseLocale", JsonValueKind.String).ToString();
                this.BigFileSize = GetVariable(data, "BigFileSize", JsonValueKind.Number).GetInt32();
                this.LogExtension = (LogExtension) Enum.Parse(typeof(LogExtension), GetVariable(data, "LogExtension", JsonValueKind.String).ToString(),true);
                this.CryptoSoftLocation = this.GetPathEnvVariable(data, "CryptoSoftLocation");
                this.CryptedExtensionsLocation = this.GetPathEnvVariable(data, "CryptedExtensionsLocation");
            }
            catch(MissingEnvVariableException e)
            {
                Console.WriteLine("Missing variable");
                throw e;
            }
        }

        /*
         * 
         * Function : GetPathEnvVariable(JsonElement data, string label)
         * Usage    : Returns a full path of the environment variable (mainly used for the locations) 
         * 
         */
        private string GetPathEnvVariable(JsonElement data,string label)
        {
            return Path.Combine(this.envPath, this.GetVariable(data, label, JsonValueKind.String).GetString());
        }


        /*
         * Function : GetVariable(JsonElement data,string varName)
         * Usage    : Returns an environment variable if present and if it is a string
         * 
         * 
         */
        private JsonElement GetVariable(JsonElement data,string varName,JsonValueKind valueKind)
        {
            JsonElement returned;
            if (data.TryGetProperty(varName, out returned) && returned.ValueKind.Equals(valueKind))
            {
                return returned;
            }
            else
            {
                throw new MissingEnvVariableException();
            }
        }
    }
}


