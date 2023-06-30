using System;
using System.Collections.Generic;
using System.Text;
using ProjectLibrary.Models;

namespace ProjectLibrary.LogLibrary.Serializers
{
    interface ISerializer
    {
        LogExtension LogExtension { get; set; }

        /*
         * Function    : SerializeLogList(LogList logList)
         * Usage       : Function that returns a Serialized LogList()
         * 
         */
        string SerializeLogList(LogList logList);

        /*
         * Function    : SerializeWorkList(SerializableDictionary<string, List<Work>> workList)
         * Usage       : Function that returns a Serialized Dictionnary containing the works.
         * 
         */
        string SerializeWorkList(SerializableDictionary<string, List<Work>> workList);


        /*
         * Function    : GetLogList(string path)
         * Usage       : Function that returns a deserialized logFile in the form of a LogList object.
         * 
         */
        LogList GetLogList(string path);


        /*
         * Function    : GetWorkList()
         * Usage       : Function that returns a deserialized dictionnary of works from the workFile.
         * 
         */
        SerializableDictionary<string, List<Work>> GetWorkList(string path);
    }
}
