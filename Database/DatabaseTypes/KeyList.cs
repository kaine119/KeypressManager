using System;
using System.Collections.Generic;
using System.Text;

namespace Database.DatabaseTypes
{
    public class KeyList: IDatabaseType
    {
        public string Name { get; set; }
        public List<KeyBunch> Keys { get; set; } = new List<KeyBunch>();

        public bool IsValid => !(Name is null);
    }
}
