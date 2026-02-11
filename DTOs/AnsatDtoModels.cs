using InheritanceDemo.DTOInterfaces;

namespace InheritanceDemo.DTOs
{
    // POST: Oprettelse
    public class AnsatCreateDto : PersonCreateDto
    {
        public decimal MaanedsLoen { get; set; }
        public int AfdelingId { get; set; }
    }

    // Bruges ved HTTP PUT
    public class AnsatUpdateDto : AnsatCreateDto, IHasIdField
    {
        public int Id { get; set; }
    }

    //public class AnsatUpdateDto : PersonDto
    //{
    //    // Vi kan arve fra PersonDto, da den har alt hvad vi skal bruge til en update
    //}

    // GET: Læsning
    public class AnsatDto : PersonDto
    {
        public decimal MaanedsLoen { get; set; }
        public string AfdelingNavn { get; set; } = String.Empty;
        // Mapster flader denne ud fra Afdeling.Navn
    }
}
