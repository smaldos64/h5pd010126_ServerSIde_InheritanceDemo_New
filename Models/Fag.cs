namespace InheritanceDemo.Models
{
    public class Fag
    {
        public int FagId { get; set; }
        public string FagTitel { get; set; } = string.Empty;
        public List<Student> Studerende { get; set; } = new();
    }
}
