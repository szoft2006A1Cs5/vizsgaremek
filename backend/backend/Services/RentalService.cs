using backend.Contexts;
using backend.Models;

namespace backend.Services
{
    public class RentalService
    {
        private readonly Context _context;

        public RentalService(Context context) 
        {
            _context = context;
        }

        public bool Update(Rental rental, User user)
        {
            return true;
        }
    }
}
