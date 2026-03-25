using aspnet_qa.API.DTOs;
using aspnet_qa.API.Models;
using aspnet_qa.API.Repositories;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace aspnet_qa.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class QuestionController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly QuestionRepository _questionRepository;
        private readonly AnswerRepository _answerRepository;

        public QuestionController(
            IMapper mapper,
            QuestionRepository questionRepository,
            AnswerRepository answerRepository)
        {
            _mapper = mapper;
            _questionRepository = questionRepository;
            _answerRepository = answerRepository;
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

        [HttpGet("my")]
        public async Task<ActionResult<List<QuestionDto>>> My()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized();
            }

            var questions = await _questionRepository
                .GetQuestionsWithDetails()
                .Where(x => x.AppUserId == userId)
                .OrderByDescending(x => x.Created)
                .ToListAsync();

            var dto = _mapper.Map<List<QuestionDto>>(questions);
            return Ok(dto);
        }

        [HttpGet("slug/{slug}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetBySlug(string slug)
        {
            var question = await _questionRepository
                .GetQuestionsWithDetails()
                .FirstOrDefaultAsync(x => x.Slug == slug);

            if (question == null)
            {
                return NotFound();
            }

            var trackedQuestion = await _questionRepository.GetByIdAsync(question.Id);
            if (trackedQuestion != null)
            {
                trackedQuestion.ViewCount += 1;
                trackedQuestion.Updated = DateTime.UtcNow;
                await _questionRepository.UpdateAsync(trackedQuestion);

                // Response'ta da güncel görünmesi için
                question.ViewCount = trackedQuestion.ViewCount;
            }

            var questionDto = _mapper.Map<QuestionDto>(question);

            var answers = await _answerRepository
                .GetAnswersWithDetails()
                .Where(a => a.QuestionId == question.Id)
                .OrderByDescending(a => a.Created)
                .Select(a => new AnswerDto
                {
                    Id = a.Id,
                    IsActive = a.IsActive,
                    Created = a.Created,
                    Updated = a.Updated,
                    Content = a.Content,
                    IsAccepted = a.IsAccepted,
                    QuestionId = a.QuestionId,
                    QuestionTitle = question.Title,
                    QuestionSlug = question.Slug,
                    AppUserId = a.AppUserId,
                    AuthorName = a.AppUser != null ? a.AppUser.FullName : "Kullanıcı",
                    AuthorUserName = a.AppUser != null ? a.AppUser.UserName : "kullanici",
                    AuthorPhotoUrl = a.AppUser != null ? a.AppUser.PhotoUrl : "default-profile-photo.jpg"
                })
                .ToListAsync();

            return Ok(new
            {
                question = questionDto,
                answers
            });
        }

        [HttpGet("user/{username}")]
        [AllowAnonymous]
        public async Task<ActionResult<List<QuestionDto>>> GetByUserName(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                return BadRequest();
            }

            var normalizedUserName = username.Trim().ToLower();

            var questions = await _questionRepository
                .GetQuestionsWithDetails()
                .Where(x =>
                    x.AppUser != null &&
                    x.AppUser.UserName != null &&
                    x.AppUser.UserName.ToLower() == normalizedUserName)
                .OrderByDescending(x => x.Created)
                .ToListAsync();

            var dto = _mapper.Map<List<QuestionDto>>(questions);
            return Ok(dto);
        }

        [HttpPost("{questionId:int}/answers")]
        public async Task<IActionResult> AddAnswer(int questionId, [FromBody] AddAnswerRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Content))
            {
                return BadRequest(new { message = "Cevap içeriği zorunludur." });
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized();
            }

            var question = await _questionRepository
                .Where(x => x.Id == questionId)
                .FirstOrDefaultAsync();

            if (question == null)
            {
                return NotFound(new { message = "Soru bulunamadı." });
            }

            var answer = new Answer
            {
                IsActive = true,
                Created = DateTime.UtcNow,
                Updated = DateTime.UtcNow,
                Content = request.Content.Trim(),
                IsAccepted = false,
                QuestionId = questionId,
                AppUserId = userId
            };

            await _answerRepository.AddAsync(answer);

            question.Updated = DateTime.UtcNow;
            await _questionRepository.UpdateAsync(question);

            var created = await _answerRepository
                .GetAnswersWithDetails()
                .FirstOrDefaultAsync(a => a.Id == answer.Id);

            var dto = _mapper.Map<AnswerDto>(created);
            return Ok(new { answer = dto });
        }

        public sealed class AddAnswerRequest
        {
            public string Content { get; set; }
        }
    }
}
