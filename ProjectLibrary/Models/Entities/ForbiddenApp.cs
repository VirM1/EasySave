using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectLibrary.Models
{
    public class ForbiddenApp
    {
        private string name;

        public string Name { get => name; set => name = value; }

        public override bool Equals(object obj)
        {
            var other = obj as ForbiddenApp;
            if (other == null)
            {
                return false;
            }
            return Name == other.Name;
        }
        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }
}
