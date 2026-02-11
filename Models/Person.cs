namespace InheritanceDemo.Models
{
    // Base klasse
    public abstract class Person
    {
        public int Id { get; set; }
        public string Navn { get; set; } = string.Empty;
        public int Alder { get; set; }
        public string Email { get; set; } = string.Empty;
    }
}
