using Microsoft.EntityFrameworkCore;
using PasteBook.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PasteBook.Data.Repositories
{
    public interface IPostRepository : IBaseRepository<Post>
    {
        Task<IEnumerable<Post>> FindByUserAccountId(int id);
    }
    public class PostRepository : GenericRepository<Post>, IPostRepository
    {
        public PostRepository(PasteBookDbContext context) : base(context)
        {
        }
        public async Task<IEnumerable<Post>> FindByUserAccountId(int id)
        {
            var posts = await this.Context.Posts.Where(x => x.UserAccountId == id).ToListAsync();
            if (posts != null)
            {
                return posts;
            }
            return null;
        }
    }
}
