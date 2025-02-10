using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Polufabrikkat.Core.Interfaces;

namespace Polufabrikkat.Site.Controllers
{
    [Authorize]
    public class AiGeneratorController : BaseController
    {
        private readonly IAiGeneratePostService _aiGeneratePostService;

        public AiGeneratorController(IAiGeneratePostService aiGeneratePostService)
        {
            _aiGeneratePostService = aiGeneratePostService;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GenerateContent([FromQuery]string tags)
        {
            if (string.IsNullOrWhiteSpace(tags))
            {
                ModelState.AddModelError("", "Пожалуйста, введите теги.");
                return View("Index");
            }

            try
            {
                string query = tags.Replace(",", " ");
                string prompt = "Создать одну публикацию";

                byte[] resultImage = await _aiGeneratePostService.GetGeneratedImage(query, prompt);


                return File(resultImage, "image/jpeg");
            }
            catch (Exception)
            {
                // Логирование ошибки
                ModelState.AddModelError("", "Произошла ошибка при генерации контента. Пожалуйста, попробуйте еще раз.");
                return View("Index");
            }
        }
    }
}
