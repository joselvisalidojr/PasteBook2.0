using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PasteBook.Data;
using System.Threading.Tasks;

namespace PasteBook.WebApi.Controllers
{
    [ApiController]
    [Route("infinite-scroll")]
    public class InfiniteScrollController : ControllerBase
    {
        public IUnitOfWork UnitOfWork { get; private set; }
        public InfiniteScrollController(IUnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> GetNewsfeedPost([FromQuery] int pageNumber, int itemsPerScroll)
        {
            var posts = await this.UnitOfWork.PostRepository.InfiniteScrollList(pageNumber, itemsPerScroll);
            return Ok(posts);
        }
    }
}
