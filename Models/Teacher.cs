namespace InheritanceDemo.Models
{
    public class Teacher : Person
    {
        public List<Fag> Fag { get; set; } = new();

        public List<Afdeling> Afdelinger { get; set;} = new();
    }
}
