using backend.Models;

namespace backend.Services.RentalService;

public class RentalResult
{
    public Rental? Data { get; set; }
    public int StatusCode { get; set; }
    public string? ErrorMessage { get; set; }

    public RentalResult(int statusCode)
    {
        StatusCode = statusCode;
    }
        
    public RentalResult(int statusCode, string errorMessage)
    {
        StatusCode = statusCode;
        ErrorMessage = errorMessage;
    }

    public RentalResult(Rental data)
    {
        Data = data;
        StatusCode = 200;
    }

    public static RentalResult Ok(Rental data) => new(data);
    public static RentalResult BadRequest(string message) => new(400, message);
    public static RentalResult NoContent() => new(204);

    public override bool Equals(object? obj)
    {
        if (obj is RentalResult result) return result.StatusCode == StatusCode;
            
        return false;
    }
}