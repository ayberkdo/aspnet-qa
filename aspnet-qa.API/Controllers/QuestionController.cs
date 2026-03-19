using aspnet_qa.API.DTOs;
using aspnet_qa.API.Repositories;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace aspnet_qa.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class QuestionController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly QuestionRepository _questionRepository;
        private readonly UserRepository _userRepository;
        private readonly TagRepository _tagRepository;

        public QuestionController(
            IConfiguration configuration,
            IMapper mapper,
            QuestionRepository questionRepository,
            UserRepository userRepository,
            TagRepository tagRepository)
        {
            _configuration = configuration;
            _mapper = mapper;
            _questionRepository = questionRepository;
            _userRepository = userRepository;
            _tagRepository = tagRepository;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<List<QuestionDto>> List()
        {
            var questions = await _questionRepository
                .GetQuestionsWithDetails()
                .OrderByDescending(x => x.Created)
                .ToListAsync();

            return _mapper.Map<List<QuestionDto>>(questions);
        }
    }
}
