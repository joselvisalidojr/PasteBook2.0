using Microsoft.EntityFrameworkCore;
using PasteBook.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PasteBook.Data.Repositories
{
    public interface IFriendRepository : IBaseRepository<Friend>
    {
        Task<IEnumerable<Friend>> FindByUserAccountId(int id);
        Task<Friend> FindByUserAccountIds(int userAccountId, int friendAccountId);
    }
    public class FriendRepository : GenericRepository<Friend>, IFriendRepository
    {
        public FriendRepository(PasteBookDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Friend>> FindByUserAccountId(int id)
        {
            var friendList = await this.Context.Friends.Where(x => x.UserAccountId == id || x.FriendAccountId == id).ToListAsync();
            if (friendList != null)
            {
                return friendList;
            }
            return null;
        }

        public async Task<Friend> FindByUserAccountIds(int userAccountId, int friendAccountId)
        {
            var friend = await this.Context.Friends.Where(x => x.UserAccountId == userAccountId && x.FriendAccountId == friendAccountId).FirstOrDefaultAsync();
            if (friend == null)
            {
                friend = await this.Context.Friends.Where(x => x.UserAccountId == friendAccountId && x.FriendAccountId == userAccountId).FirstOrDefaultAsync();
                if (friend == null)
                {
                    return null;
                }
            }
            return friend;
        }
    }
}
