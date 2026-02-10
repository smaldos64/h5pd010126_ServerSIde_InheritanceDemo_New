namespace InheritanceDemo.Models
{
    // Base klasse
    public abstract class Person
    {
        public int PersonId { get; set; }
        public string PersonNavn { get; set; } = string.Empty;
        public int PersonAlder { get; set; }
        public string PersonEmail { get; set; } = string.Empty;
    }
}
