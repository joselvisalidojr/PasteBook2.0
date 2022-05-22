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
    [Route("comments")]
    [ApiController]
    public class CommentController: ControllerBase
    {
        private readonly IUnitOfWork UnitOfWork;
        public CommentController(IUnitOfWork unitOfWork)
        {
            this.UnitOfWork = unitOfWork;
        }

        [HttpGet("get-comments")]
        public async Task<IActionResult> GetComments(int id)
        {
            var comments = await this.UnitOfWork.CommentRepository.FindByPostId(id);

            if (comments != null)
            {
                var commentListDTO = new List<CommentDTO>();
                foreach (var comment in comments)
                {
                    var CommentingUser = await UnitOfWork.UserAccountRepository.FindByPrimaryKey(comment.CommentingUserId);
                    commentListDTO.Add(new CommentDTO
                    {
                        Id = comment.Id,
                        PostId = comment.PostId,
                        CommentingUserId = comment.CommentingUserId,
                        FirstName = CommentingUser.FirstName,
                        LastName = CommentingUser.LastName,
                        ProfileImagePath = CommentingUser.ProfileImagePath,
                        Active = CommentingUser.Active,
                        CommentContent = comment.CommentContent,
                        CreatedDate = comment.CreatedDate
                    });
                }
                return Ok(commentListDTO);
            }
            return NotFound(null);
        }

        [HttpPost("new-comment")]
        public async Task<IActionResult> AddNewComment([FromBody] AddNewCommentDTO commentDTO)
        {
            var newComment = new Comment()
            {
                PostId = commentDTO.PostId,
                CommentingUserId = commentDTO.CommentingUserId,
                CommentContent = commentDTO.CommentContent,
                CreatedDate = DateTime.Now
            };

            await this.UnitOfWork.CommentRepository.Insert(newComment);
            await this.UnitOfWork.CommitAsync();

            //get post detail to get userid of post owner
            var post = await this.UnitOfWork.PostRepository.FindByPrimaryKey(commentDTO.PostId);

            //create notification
            var newNotif = new Notification()
            {
                UserAccountId = post.UserAccountId,
                CreatedDate = DateTime.Now,
                NotificationType = "comment",
                Read = false,
                CommentId = newComment.Id
            };

            await this.UnitOfWork.NotificationRepository.Insert(newNotif);
            await this.UnitOfWork.CommitAsync();

            return StatusCode(StatusCodes.Status201Created, true);
            //return Ok(newComment);
        }

        [HttpDelete("delete-comment")]
        public async Task<IActionResult> DeleteComment(int commentId)
        {
            //delete notification
            var notifications = await this.UnitOfWork.NotificationRepository.FindByCommentId(commentId);
            if(notifications != null)
            {
                foreach(var notif in notifications)
                {
                    await this.UnitOfWork.NotificationRepository.Delete(notif.Id);
                }
            }

            //delete comment
            var comment = await this.UnitOfWork.CommentRepository.Delete(commentId);
            await this.UnitOfWork.CommitAsync();

            return Ok(comment);
        }
    }
}
