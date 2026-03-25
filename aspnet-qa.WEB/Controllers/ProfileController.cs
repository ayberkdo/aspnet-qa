using Microsoft.AspNetCore.Mvc;

namespace aspnet_qa.WEB.Controllers
{
    public class ProfileController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;

        public ProfileController(IConfiguration configuration, ILogger<HomeController> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        [Route("Profile")]
        [Route("Profile/Index")]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public IActionResult Index()
        {
            ViewBag.ApiBaseURL = _configuration["ApiBaseURL"];
            return View();
        }

        [Route("Profile/MyAnswers")]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public IActionResult MyAnswers()
        {
            ViewBag.ApiBaseURL = _configuration["ApiBaseURL"];
            return View();
        }

        [Route("Profile/{username}")]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public IActionResult UserProfile(string username)
        {
            ViewBag.ApiBaseURL = _configuration["ApiBaseURL"];
            ViewData["ProfileUserName"] = username;
            ViewData["Title"] = $"@{username} - Sorular";
            ViewData["ProfilePageTitle"] = $"@{username} - Sorular";
            ViewData["ProfilePageDescription"] = "Kullanıcının paylaştığı sorular.";
            ViewData["ProfileActiveTab"] = "questions";
            return View("Index");
        }

        [Route("Profile/{username}/Answers")]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public IActionResult UserProfileAnswers(string username)
        {
            ViewBag.ApiBaseURL = _configuration["ApiBaseURL"];
            ViewData["ProfileUserName"] = username;
            ViewData["Title"] = $"@{username} - Cevaplar";
            ViewData["ProfilePageTitle"] = $"@{username} - Cevaplar";
            ViewData["ProfilePageDescription"] = "Kullanıcının paylaştığı cevaplar.";
            ViewData["ProfileActiveTab"] = "answers";
            return View("MyAnswers");
        }
    }
}
