
namespace Database.Types
{
    /// <summary>
    /// A person, either as a key drawer/returner (customer) or a key issuer/receiver (staff).
    /// </summary>
    public class Person
    {
        public string NRIC { get; set; }
        public string Name { get; set; }
        public Rank Rank { get; set; }
        public string ContactNumber { get; set; }

        public Person(string nric, string name, Rank rank, string contactNumber)
        {
            NRIC = nric;
            Name = name;
            Rank = rank;
            ContactNumber = contactNumber;
        }
    }
}
