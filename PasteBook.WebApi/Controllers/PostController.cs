using Microsoft.AspNetCore.Mvc;
using PasteBook.Data;
using PasteBook.Data.DataTransferObjects;
using PasteBook.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PasteBook.WebApi.Controllers
{
    [Route("posts")]
    [ApiController]
    public class PostController: ControllerBase
    {
        private readonly IUnitOfWork UnitOfWork;
        public PostController(IUnitOfWork unitOfWork)
        {
            this.UnitOfWork = unitOfWork;
        }

        [HttpGet("get-posts-wall")]
        public async Task<IActionResult> GetPostsWall(int id)
        {
            var posts = await this.UnitOfWork.PostRepository.FindByUserAccountId(id);
            if (posts != null)
            {
                var postDTO = new List<PostDTO>();
                foreach (var post in posts)
                {
                    var userAccount = await UnitOfWork.UserAccountRepository.FindByPrimaryKey(post.UserAccountId);
                    var likes = await this.UnitOfWork.LikeRepository.FindByPostId(post.Id);
                    var comments = await this.UnitOfWork.CommentRepository.FindByPostId(post.Id);
                    postDTO.Add(new PostDTO
                    {
                        Id = post.Id,
                        UserAccountId = post.UserAccountId,

                        FirstName = userAccount.FirstName,
                        LastName = userAccount.LastName,
                        UserName = userAccount.UserName,
                        Active = userAccount.Active,
                        ProfileImagePath = userAccount.ProfileImagePath,
                        CoverImagePath = userAccount.CoverImagePath,

                        Visibility = post.Visibility,
                        TextContent = post.TextContent,
                        PostCreatedDate = post.CreatedDate,
                        AlbumId = post.AlbumId,
                        LikesCount = likes.ToList().Count,
                        CommentsCount = comments.ToList().Count
                    }); ;
                }
                return Ok(postDTO);
            }
            return BadRequest();
        }

        [HttpGet("get-posts-timeline")]
        public async Task<IActionResult> GetPostsTimeline(int id)
        {
            var postDTO = new List<PostDTO>();

            //get users posts
            var posts = await this.UnitOfWork.PostRepository.FindByUserAccountId(id);
            if (posts != null)
            {
                foreach (var post in posts)
                {
                    var userAccount = await UnitOfWork.UserAccountRepository.FindByPrimaryKey(post.UserAccountId);
                    var likes = await this.UnitOfWork.LikeRepository.FindByPostId(post.Id);
                    var comments = await this.UnitOfWork.CommentRepository.FindByPostId(post.Id);
                    postDTO.Add(new PostDTO
                    {
                        Id = post.Id,
                        UserAccountId = post.UserAccountId,

                        FirstName = userAccount.FirstName,
                        LastName = userAccount.LastName,
                        UserName = userAccount.UserName,
                        Active = userAccount.Active,
                        ProfileImagePath = userAccount.ProfileImagePath,
                        CoverImagePath = userAccount.CoverImagePath,

                        Visibility = post.Visibility,
                        TextContent = post.TextContent,
                        PostCreatedDate = post.CreatedDate,
                        AlbumId = post.AlbumId,
                        LikesCount = likes.ToList().Count,
                        CommentsCount = comments.ToList().Count
                    });
                }
            }
            //get friends post
            var friendListData = await this.UnitOfWork.FriendRepository.FindByUserAccountId(id);
            if (friendListData != null)
            {
                var accountId = 0;
                foreach (var friend in friendListData)
                {
                    if (friend.UserAccountId == id)
                    {
                        accountId = friend.FriendAccountId;
                    }
                    else
                    {
                        accountId = friend.UserAccountId;
                    }

                    var friendsPosts = await this.UnitOfWork.PostRepository.FindByUserAccountId(accountId);
                    if (friendsPosts != null)
                    {
                        foreach (var post in friendsPosts)
                        {
                            var FriendAccount = await UnitOfWork.UserAccountRepository.FindByPrimaryKey(post.UserAccountId);
                            var likes = await this.UnitOfWork.LikeRepository.FindByPostId(post.Id);
                            var comments = await this.UnitOfWork.CommentRepository.FindByPostId(post.Id);
                            postDTO.Add(new PostDTO
                            {
                                Id = post.Id,
                                UserAccountId = post.UserAccountId,

                                FirstName = FriendAccount.FirstName,
                                LastName = FriendAccount.LastName,
                                UserName = FriendAccount.UserName,
                                Active = FriendAccount.Active,
                                ProfileImagePath = FriendAccount.ProfileImagePath,
                                CoverImagePath = FriendAccount.CoverImagePath,

                                Visibility = post.Visibility,
                                TextContent = post.TextContent,
                                PostCreatedDate = post.CreatedDate,
                                AlbumId = post.AlbumId,

                                LikesCount = likes.ToList().Count,
                                CommentsCount = comments.ToList().Count
                            });
                        }
                    }
                }
            }
            if(postDTO.Count > 0)
            {
                return Ok(postDTO);
            }
            return BadRequest();
        }

        [HttpPost("new-post")]
        public async Task<IActionResult> NewPost([FromBody] NewPostDTO newPostDTO)
        {
            var newPost = new Post()
            {
                UserAccountId = newPostDTO.UserAccountId,
                Visibility = newPostDTO.Visibility,
                TextContent = newPostDTO.TextContent,
                CreatedDate = DateTime.Now,
                AlbumId = newPostDTO.AlbumId
            };

            await this.UnitOfWork.PostRepository.Insert(newPost);
            await this.UnitOfWork.CommitAsync();
            return Ok(newPost);
        }

        [HttpDelete("delete-post")]
        public async Task<IActionResult> DeletePost(int postId)
        {
            ////delete likes
            //var likes = await this.UnitOfWork.LikeRepository.FindByPostId(postId);
            //if (likes != null)
            //{
            //    foreach (var like in likes)
            //    {
            //        await this.UnitOfWork.LikeRepository.Delete(like.Id);
            //    }
            //}

            ////delete likes notifications
            //var likesNotification = await this.UnitOfWork.NotificationRepository.(postId);
            //if (likesNotification != null)
            //{
            //    foreach (var likeNotif in likesNotification)
            //    {
            //        await this.UnitOfWork.LikeRepository.Delete(likeNotif.Id);
            //    }
            //}

            ////delete comments
            //var comments = await this.UnitOfWork.CommentRepository.FindByPostId(postId);
            //if (comments != null)
            //{
            //    foreach (var comment in comments)
            //    {
            //        await this.UnitOfWork.LikeRepository.Delete(comment.Id);
            //    }
            //}

            //delete post
            var deletedPost = await this.UnitOfWork.PostRepository.Delete(postId);
            await this.UnitOfWork.CommitAsync();
            return Ok(deletedPost);
        }
    }
}
