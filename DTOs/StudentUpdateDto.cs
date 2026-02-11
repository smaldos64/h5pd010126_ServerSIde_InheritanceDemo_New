namespace InheritanceDemo.DTOs
{
    public class StudentUpdateDto : PersonDto
    {
        public int? HoldId { get; set; }
        public List<int> FagIds { get; set; } = new(); // Liste over de valgte fags primærnøgler
    }
}
