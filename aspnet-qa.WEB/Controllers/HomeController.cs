using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace aspnet_qa.WEB.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;
        public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            ViewBag.ApiBaseURL = _configuration["ApiBaseURL"];
            return View();
        }

        [Route("Login")]
        public IActionResult Login()
        {
            var ApiBaseUrl = _configuration.GetValue<string>("ApiBaseUrl");
            ViewBag.ApiBaseUrl = ApiBaseUrl;
            return View();
        }

        [Route("Register")]
        public IActionResult Register()
        {
            var ApiBaseURL = _configuration["ApiBaseURL"];
            ViewBag.ApiBaseURL = ApiBaseURL;
            return View();
        }

        [Route("Logout")]
        public IActionResult Logout()
        {
            return View();
        }

        [Route("Questions")]
        public IActionResult Questions()
        {
            var ApiBaseURL = _configuration["ApiBaseURL"];
            ViewBag.ApiBaseURL = ApiBaseURL;
            return View();
        }

        [Route("Tags")]
        public IActionResult Tags()
        {
            var ApiBaseURL = _configuration["ApiBaseURL"];
            ViewBag.ApiBaseURL = ApiBaseURL;
            return View();
        }

        [Route("Users")]
        public IActionResult Users()
        {
            var ApiBaseURL = _configuration["ApiBaseURL"];
            ViewBag.ApiBaseURL = ApiBaseURL;
            return View();
        }

        [Route("Questions/{slug}")]
        public IActionResult QuestionDetail(string slug)
        {
            ViewBag.ApiBaseURL = _configuration["ApiBaseURL"];
            ViewBag.Slug = slug;
            return View();
        }

        [Route("Tags/{slug}")]
        public IActionResult TagDetail(string slug)
        {
            ViewBag.ApiBaseURL = _configuration["ApiBaseURL"];
            ViewBag.Slug = slug;
            return View();
        }
    }
}
