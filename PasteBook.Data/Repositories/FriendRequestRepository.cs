using Microsoft.EntityFrameworkCore;
using PasteBook.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PasteBook.Data.Repositories
{
    public interface IFriendRequestRepository : IBaseRepository<FriendRequest>
    {
        Task<IEnumerable<FriendRequest>> FindByRequestReceiverId(int id);
    }
    public class FriendRequestRepository : GenericRepository<FriendRequest>, IFriendRequestRepository
    {
        public FriendRequestRepository(PasteBookDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<FriendRequest>> FindByRequestReceiverId(int id)
        {
            var friendRequests = await this.Context.FriendRequests.Where(x => x.RequestReceiverId == id).ToListAsync();
            if (friendRequests != null)
            {
                return friendRequests;
            }
            return null;
        }
    }
}
