using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PasteBook.Data;
using PasteBook.Data.DataTransferObjects;
using PasteBook.Data.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PasteBook.WebApi.Controllers
{
    [Route("friends")]
    [ApiController]
    //[Authorize]
    public class FriendController : ControllerBase
    {
        private readonly IUnitOfWork UnitOfWork;

        public FriendController(IUnitOfWork unitOfWork)
        {
            this.UnitOfWork = unitOfWork;
        }

        [HttpGet("get-friends")]
        public async Task<IActionResult> GetFriends(int id)
        {
            var friendListData = await this.UnitOfWork.FriendRepository.FindByUserAccountId(id);

            if(friendListData != null)
            {
                var friendListDTO = new List<FriendListDTO>();
                var accountId = 0;
                foreach (var friend in friendListData)
                {
                    if(friend.UserAccountId == id)
                    {
                        accountId = friend.FriendAccountId;
                    } else
                    {
                        accountId = friend.UserAccountId;
                    }

                    var FriendAccount = await UnitOfWork.UserAccountRepository.FindByPrimaryKey(accountId);

                    friendListDTO.Add(new FriendListDTO
                    {
                        Id = friend.Id,
                        UserAccountId = friend.UserAccountId,
                        FriendAccountId = friend.FriendAccountId,

                        FirstName = FriendAccount.FirstName,
                        LastName = FriendAccount.LastName,
                        EmailAddress = FriendAccount.EmailAddress,
                        UserName = FriendAccount.UserName,
                        Birthday = FriendAccount.Birthday,
                        Gender = FriendAccount.Gender,
                        AboutMe = FriendAccount.AboutMe,
                        ProfileImagePath = FriendAccount.ProfileImagePath,
                        CoverImagePath = FriendAccount.CoverImagePath,
                        Active = FriendAccount.Active,
                        CreatedDate = FriendAccount.CreatedDate,

                        AddedDate = friend.AddedDate
                    });
                }
                return Ok(friendListDTO);
            }
            return BadRequest();
        }

        [HttpPost("add-friend")]
        public async Task<IActionResult> AddFriend(int userAccountId, int friendAccountId)
        {
            var newFriend = new Friend()
            {
                UserAccountId = userAccountId,
                FriendAccountId = friendAccountId,
                AddedDate = DateTime.Now
            };
            await this.UnitOfWork.FriendRepository.Insert(newFriend);
            await this.UnitOfWork.CommitAsync();
            return Ok(newFriend);
        }

        [HttpDelete("delete-friend")]
        public async Task<IActionResult> DeleteFriend(int userAccountId, int friendAccountId)
        {
            var deleteFriend = await this.UnitOfWork.FriendRepository.FindByUserAccountIds(userAccountId, friendAccountId);
            if (deleteFriend != null)
            {
                await this.UnitOfWork.FriendRepository.Delete(deleteFriend.Id);
                await this.UnitOfWork.CommitAsync();
                return Ok(deleteFriend);
            }
            return StatusCode(StatusCodes.Status404NotFound);
        }

        [HttpPost("friend-request")]
        [Authorize]
        public async Task<IActionResult> FriendRequest([FromBody] FriendRequestDTO FriendRequest)
        {
            var friendRequest = new FriendRequest()
            {
                RequestReceiverId = FriendRequest.RequestReceiverId,
                RequestSenderId = FriendRequest.RequestSenderId
            };

            await this.UnitOfWork.FriendRequestRepository.Insert(friendRequest);
            await this.UnitOfWork.CommitAsync();

            //create notification
            var newNotif = new Notification()
            {
                UserAccountId = FriendRequest.RequestReceiverId,
                CreatedDate = DateTime.Now,
                NotificationType = "friendrequest",
                Read = false,
                FriendRequestId = friendRequest.Id
            };

            await this.UnitOfWork.NotificationRepository.Insert(newNotif);
            await this.UnitOfWork.CommitAsync();

            return StatusCode(StatusCodes.Status201Created, true);
        }

        [HttpDelete("decline-friend-request")]
        public async Task<IActionResult> DeclineFriendRequest(int id)
        {
            //delete notif data
            var notifications = await this.UnitOfWork.NotificationRepository.FindByFriendRequestId(id);
            if (notifications != null)
            {
                foreach (var notif in notifications)
                {
                    await this.UnitOfWork.NotificationRepository.Delete(notif.Id);
                }
            }

            var friendRequest = await this.UnitOfWork.FriendRequestRepository.Delete(id);
            await this.UnitOfWork.CommitAsync();
            return Ok(friendRequest);
        }


        [HttpPost("accept-friend-request")]
        public async Task<IActionResult> AcceptFriendRequest(int id)
        {
            //delete notif data
            var notifications = await this.UnitOfWork.NotificationRepository.FindByFriendRequestId(id);
            if (notifications != null)
            {
                foreach (var notif in notifications)
                {
                    await this.UnitOfWork.NotificationRepository.Delete(notif.Id);
                }
            }

            var friendRequest = await this.UnitOfWork.FriendRequestRepository.Delete(id);

            var newFriend = new Friend()
            {
                UserAccountId = friendRequest.RequestReceiverId,
                FriendAccountId = friendRequest.RequestSenderId
            };

            var addFriend = await this.UnitOfWork.FriendRepository.Insert(newFriend);
            await this.UnitOfWork.CommitAsync();
            return Ok(addFriend);
        }

        [HttpGet("check-if-friend")]
        public async Task<IActionResult> CheckIfFriend(int userId, int friendId)
        {
            var friend = await this.UnitOfWork.FriendRepository.FindByUserAccountIds(userId, friendId);
            if (friend != null)
            {
                return Ok(friend);
            }
            return BadRequest();
        }

        [HttpGet("get-friend-request-by-userid")]
        public async Task<IActionResult> GetFriendRequestByUserId(int userId)
        {
            var friendRequest = await this.UnitOfWork.FriendRequestRepository.FindByRequestReceiverId(userId);
            if (friendRequest != null)
            {
                var friendRequestDTO = new List<userFriendRequestDTO>();
                foreach (var friendReq in friendRequest)
                {
                    var user = await this.UnitOfWork.UserAccountRepository.FindByPrimaryKey(friendReq.RequestSenderId);
                    friendRequestDTO.Add(new userFriendRequestDTO
                    {
                        Id = friendReq.Id,
                        RequestReceiverId = friendReq.RequestReceiverId,
                        RequestSenderId = friendReq.RequestSenderId,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Active = user.Active,
                        ProfileImagePath = user.ProfileImagePath,
                        requestDate = friendReq.RequestDate
                    });
                }
                return Ok(friendRequestDTO);
            }
            return BadRequest();
        }
    }
}
