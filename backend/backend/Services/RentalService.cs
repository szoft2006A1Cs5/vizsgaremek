using backend.Contexts;
using backend.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Services
{
    public class Result<T>
    {
        public T? Data { get; set; }
        public int StatusCode { get; set; }
        public string? ErrorMessage { get; set; }

        public Result(int statusCode)
        {
            StatusCode = statusCode;
        }
        
        public Result(int statusCode, string errorMessage)
        {
            StatusCode = statusCode;
            ErrorMessage = errorMessage;
        }

        public Result(T data)
        {
            Data = data;
            StatusCode = 200;
        }

        public static Result<T> Ok(T data) => new(data);
        public static Result<T> NotFound() => new(404);
        public static Result<T> Unauthorized() => new(401);
        public static Result<T> Forbidden() => new(403);
        public static Result<T> BadRequest() => new(400);
        public static Result<T> BadRequest(string message) => new(400, message);
    }
    
    public class RentalService
    {
        private readonly Context _context;

        public RentalService(Context context) 
        {
            _context = context;
        }
        
        public Result<Rental> Update(Rental rental, User authUser)
        {
            return Result<Rental>.Ok(rental);
        }
    }
}
