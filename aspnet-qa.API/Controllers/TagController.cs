using aspnet_qa.API.DTOs;
using aspnet_qa.API.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace aspnet_qa.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TagController : ControllerBase
    {
        private readonly TagRepository _tagRepository;

        public TagController(TagRepository tagRepository)
        {
            _tagRepository = tagRepository;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<List<TagDto>>> List()
        {
            var tags = await _tagRepository
                .Where(x => x.IsActive)
                .OrderBy(x => x.Name)
                .Select(x => new TagDto
                {
                    Id = x.Id,
                    IsActive = x.IsActive,
                    Created = x.Created,
                    Updated = x.Updated,
                    Name = x.Name,
                    Slug = x.Slug
                })
                .ToListAsync();

            return Ok(tags);
        }

        [HttpGet("slug/{slug}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetBySlug(string slug)
        {
            TagQuestionsDto? result = await _tagRepository.GetTagWithQuestionsBySlugAsync(slug);
            if (result == null) return NotFound();

            return Ok(result);
        }
    }
}
