using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using backend.Common;
using backend.Contexts;

namespace backend.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int NotificationId { get; set; }
        [JsonIgnore]
        public User? User { get; set; }
        [MaxLength(512)]
        public required string Content { get; set; }
        public DateTime TimeSent { get; set; }
        public bool Read { get; set; }

        public static async Task<Notification> Send(int userId, string content, Context context)
        {
            var notif = new Notification
            {
                UserId = userId,
                NotificationId = context.Notifications
                    .Where(x => x.UserId == userId)
                    .MaxOrZero(x => x.NotificationId) + 1,
                Content = content,
                TimeSent = DateTime.Now,
                Read = false,
            };

            await context.Notifications.AddAsync(notif);
            await context.SaveChangesAsync();

            return notif;
        }
    }
}
