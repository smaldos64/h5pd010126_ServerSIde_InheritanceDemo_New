namespace InheritanceDemo.Models
{
    public class Afdeling
    {
        public int AfdelingId { get; set; }
        public string AfdelingNavn { get; set; } = string.Empty;
        public List<Ansat> Ansatte { get; set; } = new();
        public List<Teacher> Teachers { get; set; } = new();
    }
}
