using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbeXP.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class FirestoreFieldAttribute : Attribute
    {
        public string Path { get; }
        public FirestoreFieldAttribute(string path) => Path = path;
    }
}
