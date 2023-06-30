using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using ProjectLibrary.Models;
using ProjectLibrary.LogLibrary;

namespace ProjectLibrary.FileLibrary
{

    /* 
     *   Class : FileManager 
     *   Usage : Class representing the static FileManager, used for to manage the files and Serialize Objects 
     */
    public class FileManager
    {

        /*
         * Function    : GetAvailableFileNamesRegex(string path, string regex)
         * Usage       : Returns all the files matching the given Regex in the given path.
         */
        public static List<string> GetAvailableFileNamesRegex(string path, string regex)
        {
            DirectoryInfo d = new DirectoryInfo(path);

            FileInfo[] Files = d.GetFiles(regex);
            List<string> fileNames = new List<string>();
            foreach (FileInfo file in Files)
            {
                fileNames.Add(file.Name);
            }
            return fileNames;
        }

        /*
         * Function    : GetBackup(string backupFilePath,LogExtension logExtension = LogExtension.Json)
         * Usage       : Opens a JSON file at the given path and deserializes its content into a BackUp object.
         */
        public static Backup GetBackup(string backupFilePath, LogExtension logExtension = LogExtension.json)
        {
            return JsonSerializer.Deserialize<Backup>(FileManager.GetStream(backupFilePath));
        }

        /*
         * Function    : GetStream(string filePath)
         * Usage       : Returns a string containing the content of a file.
         */
        public static string GetStream(string filePath)
        {
            string stream;
            using StreamReader streamReader = new StreamReader(filePath, Encoding.UTF8);
            stream = streamReader.ReadToEnd();
            streamReader.Close();
            return stream;
        }

        /*
         * Function    : DoFileExist(string path)
         * Usage       : Returns a boolean whether a file exists or not.
         */
        public static bool DoFileExist(string path)
        {
            return File.Exists(path);
        }

        /*
         * Method      : WriteAndSaveFile(string path, string content)
         * Usage       : Writes a file with the given content
         * 
         */
        public static void WriteAndSaveFile(string path, string content)
        {
            File.WriteAllText(path, content);
        }

        /*
         * Method      : DeleteFile(string path)
         * Usage       : Deletes a file at the given path.
         * 
         */
        public static void DeleteFile(string path)
        {
            File.Delete(path);
        }

        /*
         * Function    : CheckIfValidFileName(string filename)
         * Usage       : Returns a boolean whether a filename is valid or not.
         */
        public static bool CheckIfValidFileName(string filename)
        {
            if(filename == null)
            {
                return true;
            }
            return filename.IndexOfAny(Path.GetInvalidFileNameChars()) > 0 || filename.Contains(" ") || filename == String.Empty || filename.Trim().Length == 0;
        }

        /*
         * Function    : GetCompleteFiles(string source, string destination)
         * Usage       : Returns a List filled with all the relativePaths of the files contained inside the source folder.
         */
        public static (List<string>, List<string>) GetCompleteFiles(string source, string destination, List<string> priorityExtensions)
        {
            List<string> fileNamesNonPriority = new List<string>();
            List<string> fileNamesPriority = new List<string>();
            FileInfo[] allfiles = FileManager.GetAllFilesFromDir(source);
            foreach (FileInfo file in allfiles)
            {
                (fileNamesPriority, fileNamesNonPriority) = AddFileToAccordingList(fileNamesPriority, fileNamesNonPriority, file, Path.GetRelativePath(source, file.FullName),priorityExtensions);
            }

            return (fileNamesPriority, fileNamesNonPriority);
        }

        /*
         * Function    : GetDifferentialFiles(string source, string destination)
         * Usage       : Returns a List filled with all the relativePaths of the files contained inside the source folder that are different or absent from the Destination folder.
         */
        public static (List<string>,List<string>) GetDifferentialFiles(string source, string destination, List<string> priorityExtensions)
        {
            List<string> fileNamesNonPriority = new List<string>();
            List<string> fileNamesPriority = new List<string>();
            string relativePath;
            string distantPath;
            FileInfo[] allfiles = FileManager.GetAllFilesFromDir(source);
            foreach (FileInfo file in allfiles)
            {
                relativePath = Path.GetRelativePath(source, file.FullName);
                distantPath = Path.Combine(destination, relativePath);
                if (!File.Exists(distantPath))
                {
                    (fileNamesPriority, fileNamesNonPriority) = AddFileToAccordingList(fileNamesPriority, fileNamesNonPriority, file, relativePath,priorityExtensions);
                }
                else
                {//for comparison use Date or go with something else ? (idk Fileattribute Archive)
                    if (DateTime.Compare(File.GetLastWriteTime(file.FullName), File.GetLastWriteTime(distantPath)) > 0)
                    {
                        (fileNamesPriority, fileNamesNonPriority) = AddFileToAccordingList(fileNamesPriority, fileNamesNonPriority, file, relativePath,priorityExtensions);
                    }
                }
            }

            return (fileNamesPriority, fileNamesNonPriority);
        }

        private static (List<string>,List<string>) AddFileToAccordingList(List<string> fileNamesPriority, List<string> fileNamesNonPriority, FileInfo file, string path,List<string> priorityExtensions)
        {
            if (IsPriorityFile(file,priorityExtensions))
            {
                fileNamesPriority.Add(path);
            }
            else
            {
                fileNamesNonPriority.Add(path);
            }
            return (fileNamesPriority, fileNamesNonPriority);
        }

        private static bool IsPriorityFile(FileInfo file,List<string> priorityExtensions)
        {
            return priorityExtensions.Contains(file.Extension);
        }

        private static FileInfo[] GetAllFilesFromDir(string source)
        {

            DirectoryInfo dir = new DirectoryInfo(source);
            return dir.GetFiles("*.*", SearchOption.AllDirectories);
        }

        /*
         * Method      : CopyFile(string source,string destination)
         * Usage       : Copies a file to the destination folder.
         * 
         */
        public static void CopyFile(string source,string destination)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(destination));
            File.Copy(source, destination,true);
            File.SetLastWriteTime(destination, DateTime.Now);
        }
    }
}
