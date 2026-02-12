using FluentValidation;

namespace InheritanceDemo.Models
{
    // Ansat med 1:N relation
    public class Ansat : Person
    {
        public decimal MaanedsLoen { get; set; }
        public int AfdelingId { get; set; }
        public Afdeling? Afdeling { get; set; }
    }
}
