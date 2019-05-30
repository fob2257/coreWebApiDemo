using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace coreWebApiDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly IConfiguration configuration;
        private readonly IDataProtector protector;

        public ValuesController(IConfiguration configuration, IDataProtectionProvider protectionProvider)
        {
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            this.protector = protectionProvider.CreateProtector("YouShouldNeverEverEverEverTellAnyoneYourSecrets");
        }

        [HttpGet("encryption")]
        public ActionResult<string> GetEncryption()
        {
            string plainText = "Lorem Ipsum Dolor Sit Amet";
            string encryptedText = protector.Protect(plainText);
            string decryptedText = protector.Unprotect(encryptedText);

            //var protectorTimeLimit = protector.ToTimeLimitedDataProtector();
            //string encryptedTextLimitedByTime = protectorTimeLimit.Protect(plainText, TimeSpan.FromSeconds(5));
            //Thread.Sleep(6000);
            //string decryptedTextLimitedByTime = protectorTimeLimit.Unprotect(encryptedTextLimitedByTime);

            var result = new
            {
                plainText,
                encryptedText,
                decryptedText,
                //encryptedTextLimitedByTime,
                //decryptedTextLimitedByTime
            };

            return Ok(result);
        }

        [HttpGet("message")]
        public ActionResult<string> GetMessage()
        {
            return configuration["message"];
        }

        [HttpGet("cache")]
        [ResponseCache(Duration = 15)]
        [Authorize]
        public ActionResult<string> GetSeconds()
        {
            return DateTime.Now.Second.ToString();
        }

        // GET api/values
        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
