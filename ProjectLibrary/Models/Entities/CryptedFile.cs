using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectLibrary.Models
{
    public class CryptedFile
    {
        private string name;

        public string Name { get => name; set => name = value; }

        public override bool Equals(object obj)
        {
            var other = obj as CryptedFile;
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
