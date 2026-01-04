using Microsoft.EntityFrameworkCore;

namespace backend.Models
{
    [PrimaryKey(nameof(UzenetId), nameof(EleresiUt))]
    public class UzenetCsatolmany
    {
       public int UzenetId { get; set; }
       public Uzenet Uzenet { get; set; }
       public required string EleresiUt { get; set; }
    }
}
