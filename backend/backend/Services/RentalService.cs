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

        public override bool Equals(object? obj)
        {
            if (obj is Result<T> result) return result.StatusCode == StatusCode;
            
            return false;
        }
    }
    
    public class RentalService
    {
        private readonly Context _context;

        public RentalService(Context context) 
        {
            _context = context;
        }

        private void HandleStatusChange(Rental curr, Rental change, User authUser)
        {
            if (RentalStatus.Finished < change.Status)
            {
                curr.Status = change.Status;
                return;
            }
            
            var bases = new[] { 
                (int)RentalStatus.RenterOffer,
                (int)RentalStatus.RenterPickupAccepted,
                (int)RentalStatus.RenterFinishAccepted,
            };

            foreach (var b in bases)
            {
                if ((int)change.Status != b + 2) continue;

                bool otherWaiting = (int)curr.Status == (authUser.Id == curr.RenterId ? b + 1 : b);
                curr.Status = otherWaiting
                    ? (RentalStatus)(b + 2)
                    : (RentalStatus)(authUser.Id == curr.RenterId ? b : b + 1);

                // TODO: Ha OfferAccepted, akkor itt elmeletileg valahogy torolni
                //       kene az osszes tobbi erre az idoszakra vontakozo rental offert,
                //       es kuldeni azok kuldoinek egy ertesitest, hogy nem az o ajanlatukat
                //       fogadtak el.

                switch (curr.Status)
                {
                    case RentalStatus.OfferAccepted:
                        {
                            _context.Rentals.RemoveRange(
                                _context.Rentals
                                    .Where(x =>
                                        !(x.End < curr.Start && curr.End < x.Start)
                                    )
                            );

                            Notification.Send(
                                curr.RenterId,
                                $"",
                                _context
                            );
                        }
                        break;
                }

                return;
            }
            
            if (curr.Status < RentalStatus.OfferAccepted)
                curr.Status = authUser.Id == curr.RenterId ? RentalStatus.RenterOffer : RentalStatus.OwnerOffer;
        }
        
        public Result<Rental> Update(Rental curr, Rental changed, User authUser)
        {
            HandleStatusChange(curr, changed, authUser);
            
            return Result<Rental>.Ok(curr);
        }
    }
}
