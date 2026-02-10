namespace InheritanceDemo.Models
{
    public class Hold
    {
        public int HoldId { get; set; }
        public string HoldNavn { get; set; } = string.Empty;
        public List<Student> Studerende { get; set; } = new();
    }
}
