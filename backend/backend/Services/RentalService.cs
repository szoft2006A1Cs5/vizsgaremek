using backend.Contexts;
using backend.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Services
{
    public class RentalService
    {
        private readonly Context _context;

        public RentalService(Context context) 
        {
            _context = context;
        }
        
        public bool Update(Rental rental, User authUser)
        {
            return true;
        }
    }
}
