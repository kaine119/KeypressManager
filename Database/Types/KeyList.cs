using System;
using System.Collections.Generic;
using System.Text;

namespace Database.Types
{
    public class KeyList
    {
        public string Name { get; set; }
        public List<KeyBunch> Keys { get; }
    }
}
