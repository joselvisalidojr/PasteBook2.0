using Microsoft.EntityFrameworkCore;
using PasteBook.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PasteBook.Data.Repositories
{
    public interface INotificationRepository : IBaseRepository<Notification>
    {
        Task<IEnumerable<Notification>> FindByFriendRequestId(int id);
        Task<IEnumerable<Notification>> FindByLikesId(int id);
        Task<IEnumerable<Notification>> FindByCommentId(int id);
        Task<IEnumerable<Notification>> FindByUserId(int id);
    }
    public class NotificationRepository : GenericRepository<Notification>, INotificationRepository
    {
        public NotificationRepository(PasteBookDbContext context) : base(context)
        {
        }
        public async Task<IEnumerable<Notification>> FindByFriendRequestId(int id)
        {
            var notifications = await this.Context.Notifications.Where(x => x.FriendRequestId == id).ToListAsync();
            if (notifications != null)
            {
                return notifications;
            }
            return null;
        }
        public async Task<IEnumerable<Notification>> FindByLikesId(int id)
        {
            var notifications = await this.Context.Notifications.Where(x => x.LikesId == id).ToListAsync();
            if (notifications != null)
            {
                return notifications;
            }
            return null;
        }

        public async Task<IEnumerable<Notification>> FindByCommentId(int id)
        {
            var notifications = await this.Context.Notifications.Where(x => x.CommentId == id).ToListAsync();
            if (notifications != null)
            {
                return notifications;
            }
            return null;
        }
        public async Task<IEnumerable<Notification>> FindByUserId(int id)
        {
            var notifications = await this.Context.Notifications.Where(x => x.UserAccountId == id).ToListAsync();
            if (notifications != null)
            {
                return notifications;
            }
            return null;
        }
    }
}
