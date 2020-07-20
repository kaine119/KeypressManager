using System;
using System.Collections.Generic;
using System.Text;

namespace Database.Types
{
    public class KeyBunch
    {
        public string Name { get; set; }
        public string BunchNumber { get; set; }
        public List<Person> AuthorizedPersonnel { get; set; }
    }
}
