using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SERVICESAPI.DataAccess;
using SERVICESAPI.DataAccess.Repository;
using SERVICESAPI.Models.Entities;
using SERVICESAPI.Proccess.Interfaces;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace SERVICESAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OFXController : ControllerBase
    {

        #region  Dependency Injection        
        private readonly IUnitOfWork _uow;
        #endregion

        public OFXController(IUnitOfWork uow)
        {
            _uow = uow;
        }

        // POST api/values
        [HttpPost]
        [Route("LoadFile")]

        public async Task<HttpResponseMessage> LoadFile([FromServices] IProcessFileOFX proccess)
        {

            IProcessFileOFX _proccess = proccess;
            proccess.setUOW(_uow);

            string content = string.Empty;
            
            try
            {
                using (StreamReader reader = new StreamReader(Request.Body))
                {
                    content = await reader.ReadToEndAsync();
                }
            }
            catch { }

            if (string.IsNullOrEmpty(content))
                return await Task.FromResult(new HttpResponseMessage(HttpStatusCode.NoContent));
            else
            {
                var result = await _proccess.ProcessFile(content);
            }

            return new HttpResponseMessage(HttpStatusCode.OK);
        }


        
         [HttpGet]
         public ActionResult<IEnumerable<string>> Get()
         {

             return new string[] { "System", "Actived" };

         }



    }
}
