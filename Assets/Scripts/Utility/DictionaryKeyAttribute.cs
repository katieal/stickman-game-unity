using UnityEngine;
using System.Collections.Generic;

namespace Utility
{
    public class DictionaryKeyAttribute : System.Attribute
    {
        public string KeyArrayName;

        public DictionaryKeyAttribute(string keyArrayName)
        {
            this.KeyArrayName = keyArrayName;
        }
    }
}