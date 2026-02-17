using InheritanceDemo.DTOInterfaces;

namespace InheritanceDemo.DTOs
{
    public class AfdelingCreateDto
    {
        public string AfdelingNavn { get; set; } = string.Empty;
    }

    public class AfdelingUpdateDto : AfdelingCreateDto
    {
        public int AfdelingId { get; set; }
    }

    public class AfdelingDto : AfdelingUpdateDto
    {
        public List<AnsatUpdateDto>? Ansatte { get; set; }
    }
}
