
namespace Database.DatabaseTypes
{
    /// <summary>
    /// A person, either as a key drawer/returner (customer) or a key issuer/receiver (staff).
    /// </summary>
    public class Person: IDatabaseType
    {
        public string? NRIC { get; set; }
        public string? Name { get; set; }
        public Rank? Rank { get; set; }
        public string? ContactNumber { get; set; }

        public bool IsValid => !(NRIC is null) && !(Name is null) && !(Rank is null);
    }
}
