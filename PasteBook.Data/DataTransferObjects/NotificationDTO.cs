using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PasteBook.Data.DataTransferObjects
{
    public class NotificationDTO
    {
        public int UserAccountId { get; set; }
        public DateTime CreatedDate { get; set; }
        public string NotificationType { get; set; }
        public bool? Read { get; set; }
        public int? FriendRequestId { get; set; }
        public int? LikesId { get; set; }
        public int? CommentId { get; set; }
    }

    public class outNotificationDTO
    {
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public string NotificationType { get; set; }
        public bool? Read { get; set; }
        public int? FriendRequestId { get; set; }
        public int? LikesId { get; set; }
        public int? CommentId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }


}
