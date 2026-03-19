using aspnet_qa.API.DTOs;
using aspnet_qa.API.Repositories;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace aspnet_qa.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TagController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly TagRepository _tagRepository;

        public TagController(IConfiguration configuration, IMapper mapper, TagRepository tagRepository)
        {
            _configuration = configuration;
            _mapper = mapper;
            _tagRepository = tagRepository;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<List<TagDto>> List()
        {
            var tags = await _tagRepository.GetAllAsync();
            return _mapper.Map<List<TagDto>>(tags);
        }
    }
}
