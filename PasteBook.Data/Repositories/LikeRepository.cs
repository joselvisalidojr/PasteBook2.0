using Microsoft.EntityFrameworkCore;
using PasteBook.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PasteBook.Data.Repositories
{
    public interface ILikeRepository : IBaseRepository<Like>
    {
        Task<IEnumerable<Like>> FindByPostId(int id);
        Task<IEnumerable<Like>> FindByLikerId(int id);
        Task<Like> FindByPostAndUserId(int postId, int userId);
    }
    public class LikeRepository : GenericRepository<Like>, ILikeRepository
    {
        public LikeRepository(PasteBookDbContext context) : base(context)
        {
        }
        public async Task<IEnumerable<Like>> FindByPostId(int id)
        {
            var likes = await this.Context.Likes.Where(x => x.PostId == id).ToListAsync();
            if (likes != null)
            {
                return likes;
            }
            return null;
        }

        public async Task<IEnumerable<Like>> FindByLikerId(int id)
        {
            var likes = await this.Context.Likes.Where(x => x.LikerAccountId == id).ToListAsync();
            if (likes != null)
            {
                return likes;
            }
            return null;
        }

        public async Task<Like> FindByPostAndUserId(int postId, int userId)
        {
            var like = await this.Context.Likes.Where(x => x.PostId == postId && x.LikerAccountId == userId).FirstOrDefaultAsync();
            if (like != null)
            {
                return like;
            }
            return null;
        }
    }
}
