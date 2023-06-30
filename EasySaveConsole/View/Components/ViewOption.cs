using System;

namespace EasySaveConsole
{
    /*
     *  Class : ViewOption
     *  Usage : Class representing a part of the View of the MVC architecture.
     */
    public class ViewOption
    {
        public string Name { get; set; }

        public string Key { get; }

        public Action SelectedAction { get; set; }

        private bool selected = false;
        public bool Selected { get => selected; set => selected = value; }

        /*
         *  Constructor : ViewOption Constructor
         *  Usage       : Constructor of the View class, initiate the name and selected attribut
         */

        public ViewOption(string name, Action selectedAction)
        {
            Name = name;
            SelectedAction = selectedAction;
        }

        /*
         *  Constructor : ViewOption Constructor (overload)
         *  Usage       : Constructor of the View class, initiate the name, selected and key attribut
         */

        public ViewOption(string name,string key, Action selectedAction)
        {
            Name = name;
            Key = key;
            SelectedAction = selectedAction;
        }


    }
}
