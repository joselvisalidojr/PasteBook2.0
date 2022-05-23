using PasteBook.Data.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PasteBook.WebApi.Services
{
    public class InfiniteScrollService<T> where T : class
    {
        public IEnumerable<T> GetScrollItems(int pageNumber, int itemsPerScroll, IEnumerable<T> items)
        {
            if(items.Count() <= 10)
            {
                return items;
            }
            return items.Take(pageNumber * itemsPerScroll).ToList();
        }
    }
}
