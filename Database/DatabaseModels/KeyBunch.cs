using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Database.DatabaseModels
{
    public class KeyBunch: DatabaseModel
    {
        public string? Name { get; set; }
        public string? BunchNumber { get; set; }
        public int? NumberOfKeys { get; set; }
        public List<Person> AuthorizedPersonnel { get; set; } = new List<Person>();

        public override bool IsValid => !(Name is null) && !(BunchNumber is null) && !(NumberOfKeys is null);

        /// <summary>
        /// Looks up this keybunch's Authorized Personnel list for the NRIC or name given.
        /// Purely numeric queries, or queries matching the last 3 digits + letter of an NRIC, 
        /// will be treated as an NRIC.
        /// </summary>
        /// <param name="query">A name or NRIC</param>
        /// <returns>Any matching personnel.</returns>
        public List<Person> FindPersonnel(string query)
        {
            bool queryIsNRIC = Regex.IsMatch(query, @"\d{1,3}[A-Za-z]?$");
            if (queryIsNRIC)
            {
                return AuthorizedPersonnel.Where(personnel => Regex.IsMatch(personnel.NRIC, query, RegexOptions.IgnoreCase)).ToList();
            }
            else
            {
                return AuthorizedPersonnel.Where(personnel => Regex.IsMatch(personnel.Name, query, RegexOptions.IgnoreCase)).ToList();
            }
        }
    }
}
