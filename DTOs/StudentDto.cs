namespace InheritanceDemo.DTOs
{
    public class StudentDto : PersonDto
    {
        public string? HoldNavn { get; set; }
        public List<string> FagTitler { get; set; } = new();
    }
}
