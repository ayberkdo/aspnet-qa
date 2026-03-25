using aspnet_qa.API.DTOs;
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
    public class AnswerController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly AnswerRepository _answerRepository;

        public AnswerController(
            IMapper mapper,
            AnswerRepository answerRepository)
        {
            _mapper = mapper;
            _answerRepository = answerRepository;
        }

        [HttpGet("my")]
        public async Task<ActionResult<List<AnswerDto>>> My()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized();
            }

            var answers = await _answerRepository
                .GetAnswersWithDetails()
                .Where(x => x.AppUserId == userId)
                .OrderByDescending(x => x.Created)
                .ToListAsync();

            var dto = _mapper.Map<List<AnswerDto>>(answers);
            return Ok(dto);
        }

        [HttpGet("user/{username}")]
        [AllowAnonymous]
        public async Task<ActionResult<List<AnswerDto>>> GetByUserName(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                return BadRequest();
            }

            var normalizedUserName = username.Trim().ToLower();

            var answers = await _answerRepository
                .GetAnswersWithDetails()
                .Where(x =>
                    x.AppUser != null &&
                    x.AppUser.UserName != null &&
                    x.AppUser.UserName.ToLower() == normalizedUserName)
                .OrderByDescending(x => x.Created)
                .ToListAsync();

            var dto = _mapper.Map<List<AnswerDto>>(answers);
            return Ok(dto);
        }

        [HttpPost("{answerId:int}/toggle-accept")]
        public async Task<IActionResult> ToggleAccept(int answerId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized();
            }

            var answer = await _answerRepository._dbSet
                .Include(a => a.Question)
                .FirstOrDefaultAsync(a => a.Id == answerId);

            if (answer == null)
            {
                return NotFound(new { message = "Cevap bulunamadı." });
            }

            if (answer.Question == null || answer.Question.AppUserId != userId)
            {
                return Forbid();
            }

            if (!answer.IsAccepted)
            {
                var acceptedAnswer = await _answerRepository
                    .Where(a => a.QuestionId == answer.QuestionId && a.IsAccepted)
                    .FirstOrDefaultAsync();

                if (acceptedAnswer != null)
                {
                    acceptedAnswer.IsAccepted = false;
                    acceptedAnswer.Updated = DateTime.UtcNow;
                    await _answerRepository.UpdateAsync(acceptedAnswer);
                }

                answer.IsAccepted = true;
            }
            else
            {
                answer.IsAccepted = false;
            }

            answer.Updated = DateTime.UtcNow;
            await _answerRepository.UpdateAsync(answer);

            return Ok(new { answerId = answer.Id, isAccepted = answer.IsAccepted });
        }
    }
}