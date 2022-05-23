using Microsoft.AspNetCore.Mvc;
using PasteBook.Data;
using PasteBook.Data.DataTransferObjects;
using PasteBook.Data.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PasteBook.WebApi.Controllers
{
    [Route("notifications")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly IUnitOfWork UnitOfWork;

        public NotificationController(IUnitOfWork unitOfWork)
        {
            this.UnitOfWork = unitOfWork;
        }

        [HttpGet("get-notification-by-userid")]
        public async Task<IActionResult> GetNotificationByUserId(int userId)
        {
            var notifications = await this.UnitOfWork.NotificationRepository.FindByUserId(userId);
            if(notifications != null)
            {
                var friendRequestDTO = new List<outNotificationDTO>();
                foreach (var notification in notifications)
                {
                    if (notification.FriendRequestId != null)
                    {
                        var friendRequest = await this.UnitOfWork.FriendRequestRepository.FindByPrimaryKey((Int32)notification.FriendRequestId);
                        var user = await this.UnitOfWork.UserAccountRepository.FindByPrimaryKey(friendRequest.RequestSenderId);
                        friendRequestDTO.Add(new outNotificationDTO
                        {
                            Id = notification.Id,
                            CreatedDate = notification.CreatedDate,
                            NotificationType = notification.NotificationType,
                            Read = notification.Read,
                            FriendRequestId = notification.FriendRequestId,
                            LikesId = notification.LikesId,
                            CommentId = notification.CommentId,
                            FirstName = user.FirstName,
                            LastName = user.LastName
                        });
                    }
                    if (notification.LikesId != null)
                    {
                        var like = await this.UnitOfWork.LikeRepository.FindByPrimaryKey((Int32)notification.LikesId);
                        var user = await this.UnitOfWork.UserAccountRepository.FindByPrimaryKey(like.LikerAccountId);
                        friendRequestDTO.Add(new outNotificationDTO
                        {
                            Id = notification.Id,
                            CreatedDate = notification.CreatedDate,
                            NotificationType = notification.NotificationType,
                            Read = notification.Read,
                            FriendRequestId = notification.FriendRequestId,
                            LikesId = notification.LikesId,
                            CommentId = notification.CommentId,
                            FirstName = user.FirstName,
                            LastName = user.LastName
                        });
                    }
                    if (notification.CommentId != null)
                    {
                        var comment = await this.UnitOfWork.CommentRepository.FindByPrimaryKey((Int32)notification.CommentId);
                        var user = await this.UnitOfWork.UserAccountRepository.FindByPrimaryKey(comment.CommentingUserId);
                        friendRequestDTO.Add(new outNotificationDTO
                        {
                            Id = notification.Id,
                            CreatedDate = notification.CreatedDate,
                            NotificationType = notification.NotificationType,
                            Read = notification.Read,
                            FriendRequestId = notification.FriendRequestId,
                            LikesId = notification.LikesId,
                            CommentId = notification.CommentId,
                            FirstName = user.FirstName,
                            LastName = user.LastName
                        });
                    }
                }
                return Ok(friendRequestDTO);   
            }
            return BadRequest();
        }
    }
}
