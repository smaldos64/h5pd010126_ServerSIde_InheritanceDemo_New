using InheritanceDemo.DTOInterfaces;

namespace InheritanceDemo.DTOs
{
    // Bruges ved HTTP POST
    public class StudentCreateDto : PersonCreateDto
    {
        public int HoldId { get; set; }
        // En simpel liste af ID'er fra klienten
        public List<int> FagIds { get; set; } = new();
    }

    // Bruges ved HTTP PUT
    public class StudentUpdateDto : StudentCreateDto, IHasIdField
    {
        public int Id { get; set; }
    }

    public class StudentDto : PersonDto
    {
        public int HoldId { get; set; }
        public string HoldNavn { get; set; } = string.Empty;
        //public List<FagUpdateDto> Fag { get; set; } = new();
        public List<string> FagTitler { get; set; } = new();
    }
}
