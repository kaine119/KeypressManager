using System;
using System.Collections.Generic;
using System.Text;

namespace Database.DatabaseModels
{
    public class KeyList: DatabaseModel
    {
        public string Name { get; set; }
        public List<KeyBunch> Keys { get; set; } = new List<KeyBunch>();

        public override bool IsValid => !(Name is null);
    }
}
