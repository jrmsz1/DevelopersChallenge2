using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace VIEWERWEB.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private IWebHostEnvironment _environment;

        [BindProperty]
        public string URL { get; set; }

        [BindProperty]
        public string Result { get; set; }

        public IndexModel(ILogger<IndexModel> logger, IWebHostEnvironment environment)
        {
            URL = "https://localhost:5001/";
            _logger = logger;
            _environment = environment;
        }

        public void OnGet()
        {

        }

  

        [BindProperty]
        public IFormFile Upload { get; set; }
        public async Task<ContentResult> OnPostAsync()
        {
            if (!Directory.Exists(Path.Combine(_environment.ContentRootPath, "uploads")))
            {
                Directory.CreateDirectory(Path.Combine(_environment.ContentRootPath, "uploads"));
            }
            if (!string.IsNullOrEmpty(Upload.FileName))
            {
                //I could to make in memory
                var file = Path.Combine(_environment.ContentRootPath, "uploads", Upload.FileName);
                using (var fileStream = new FileStream(file, FileMode.Create))
                {
                    await Upload.CopyToAsync(fileStream);
                }

                string content = System.IO.File.ReadAllText(file);

                Result = await ConsumeWebApi(content);
                
            }

            return Content($"<br /><br /><div class=\"text - center\"> <h3 class=\"display - 4\">Result</h3>{Result} </div>");
        }


        public async Task<string> ConsumeWebApi(string content)
        {
            if (!string.IsNullOrEmpty(URL))
            {
             
                
                HttpClient _client = new HttpClient { BaseAddress = new Uri(URL) };
                if (_client.BaseAddress.ToString().ToUpper().StartsWith("HTTPS"))
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

                // string credentials = string.Format("{0}:{1}", username, password);
                // _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes(credentials)));
                _client.DefaultRequestHeaders.Accept.Clear();
                _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/xml"));


                var requestBody = new StringContent(content, Encoding.UTF8, "text/xml");

                var response = await _client.PostAsync("api/ofx/loadfile", requestBody);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();

                }
                return await Task.FromResult(string.Format("ERROR", response.StatusCode.ToString()));
            }

            return await Task.FromResult(string.Empty);
        }

    }
}
