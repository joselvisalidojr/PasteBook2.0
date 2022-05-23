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
    [Route("likes")]
    [ApiController]
    public class LikeController : ControllerBase
    {
        private readonly IUnitOfWork UnitOfWork;

        public LikeController(IUnitOfWork unitOfWork)
        {
            this.UnitOfWork = unitOfWork;
        }

        [HttpGet("get-likes")]
        public async Task<IActionResult> GetLikes(int id)
        {
            var likes = await this.UnitOfWork.LikeRepository.FindByPostId(id);

            if (likes != null)
            {
                var likesDTO = new List<LikeDTO>();
                foreach (var like in likes)
                {
                    var LikerAccount = await UnitOfWork.UserAccountRepository.FindByPrimaryKey(like.LikerAccountId);
                    likesDTO.Add(new LikeDTO
                    {
                        Id = like.Id,
                        PostId = like.PostId,
                        LikerAccountId = like.LikerAccountId,
                        FirstName = LikerAccount.FirstName,
                        LastName = LikerAccount.LastName,
                        Active = LikerAccount.Active,
                        ProfileImagePath = LikerAccount.ProfileImagePath
                    });
                }
                return Ok(likesDTO);
            }
            return NotFound(null);
        }
        [HttpGet("get-likes-by-useraccountid")]
        public async Task<IActionResult> GetLikesByUserId(int id)
        {
            var likes = await this.UnitOfWork.LikeRepository.FindByLikerId(id);

            if (likes != null)
            {
                var likesDTO = new List<LikeDTO>();
                foreach (var like in likes)
                {
                    var LikerAccount = await UnitOfWork.UserAccountRepository.FindByPrimaryKey(like.LikerAccountId);
                    likesDTO.Add(new LikeDTO
                    {
                        Id = like.Id,
                        PostId = like.PostId,
                        LikerAccountId = like.LikerAccountId,
                        FirstName = LikerAccount.FirstName,
                        LastName = LikerAccount.LastName,
                        Active = LikerAccount.Active,
                        ProfileImagePath = LikerAccount.ProfileImagePath
                    });
                }
                return Ok(likesDTO);
            }
            return NotFound(null);
        }

        [HttpPost("new-like")]
        public async Task<IActionResult> AddNewLike([FromBody] NewLikeDTO newLikeDTO)
        {
            var newLike = new Like()
            {
                PostId = newLikeDTO.PostId,
                LikerAccountId = newLikeDTO.LikerAccountId
            };

            await this.UnitOfWork.LikeRepository.Insert(newLike);
            await this.UnitOfWork.CommitAsync();

            //get post detail to get userid of post owner
            var post = await this.UnitOfWork.PostRepository.FindByPrimaryKey(newLikeDTO.PostId);

            //create notification
            var newNotif = new Notification()
            {
                UserAccountId = post.UserAccountId,
                CreatedDate = DateTime.Now,
                NotificationType = "like",
                Read = false,
                LikesId = newLike.Id
            };

            await this.UnitOfWork.NotificationRepository.Insert(newNotif);
            await this.UnitOfWork.CommitAsync();

            return StatusCode(StatusCodes.Status201Created, true);
            //return Ok(newComment);
        }

        [HttpDelete("unlike-post")]
        public async Task<IActionResult> UnlikePost(int likeId)
        {
            //delete notification
            var notifications = await this.UnitOfWork.NotificationRepository.FindByLikesId(likeId);
            if (notifications != null)
            {
                foreach (var notif in notifications)
                {
                    await this.UnitOfWork.NotificationRepository.Delete(notif.Id);
                }
            }

            //delete like
            var like = await this.UnitOfWork.LikeRepository.Delete(likeId);
            await this.UnitOfWork.CommitAsync();

            return Ok(like);
        }

        [HttpGet("check-like-status")]
        public async Task<IActionResult> CheckLikeStatus(int postId, int userId)
        {
            var like = await this.UnitOfWork.LikeRepository.FindByPostAndUserId(postId, userId);
            if (like != null)
            {
                return Ok(like);
            }
            return BadRequest();
        }
    }
}
