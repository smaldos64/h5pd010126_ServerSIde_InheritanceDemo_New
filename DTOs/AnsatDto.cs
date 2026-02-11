namespace InheritanceDemo.DTOs
{
    // Bruges ved HTTP POST
    public class AnsatDtoCreate : PersonCreateDto
    {
        public int AfdelingId { get; set; }
        public decimal MaanedsLoen { get; set; }
    }

    // Bruges ved HTTP PUT

    public class AnsatDto : PersonDto
    {
        public decimal MaanedsLoen { get; set; }
        public int AfdelingId { get; set; }
        public string AfdelingNavn { get; set; }
    }
}
