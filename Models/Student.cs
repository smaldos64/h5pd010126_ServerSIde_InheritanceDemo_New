namespace InheritanceDemo.Models
{
    // Student med 1:N og N:M relationer
    public class Student : Person
    {
        public int HoldId { get; set; }
        public Hold? Hold { get; set; }
        public List<Fag> Fag { get; set; } = new();
    }
}
