
namespace Database.Types
{
    /// <summary>
    /// A person, either as a key drawer/returner (customer) or a key issuer/receiver (staff).
    /// </summary>
    public class Person
    {
        public string NRIC { get; }
        public string Name { get; }
        public Rank Rank { get; }
        public string ContactNumber { get; }
    }
}
