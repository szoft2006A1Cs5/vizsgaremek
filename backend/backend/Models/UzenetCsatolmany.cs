namespace backend.Models
{
    public class UzenetCsatolmany
    {
       public int UzenetId { get; set; }
       public Uzenet Uzenet { get; set; }
       public required string EleresiUt { get; set; }
    }
}
