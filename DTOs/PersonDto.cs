using InheritanceDemo.Models;
using System.Text.Json.Serialization;

namespace InheritanceDemo.DTOs
{
    [JsonDerivedType(typeof(StudentDto), typeDiscriminator: "student")]
    [JsonDerivedType(typeof(AnsatDto), typeDiscriminator: "ansat")]
    public class PersonDto : Person
    {
        //public int PersonId { get; set; }
        //public string PersonNavn { get; set; } = string.Empty;
        //public int PersonAlder { get; set; }
        //public string PersonEmail { get; set; } = string.Empty;
    }
}
