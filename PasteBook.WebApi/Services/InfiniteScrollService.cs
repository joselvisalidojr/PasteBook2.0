using PasteBook.Data.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PasteBook.WebApi.Services
{
    public class InfiniteScrollService<T> where T : BaseEntity
    {
        public IEnumerable<T> GetScrollItems(int pageNumber, int itemsPerScroll, IEnumerable<T> items)
        {
            return items.Take(pageNumber * itemsPerScroll).ToList();
        }
    }
}
