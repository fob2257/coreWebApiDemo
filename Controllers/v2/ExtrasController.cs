using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;

using coreWebApiDemo.Business.Services;

namespace coreWebApiDemo.Controllers.v2
{
    [Route("api/v2/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ExtrasController : ControllerBase
    {
        private readonly IConfiguration configuration;
        private readonly IDataProtector protector;
        private readonly IHashService hashService;
        private readonly IDIService diService;

        public ExtrasController(
            IConfiguration configuration,
            IDataProtectionProvider protectionProvider,
            IHashService hashService,
            IDIService diService)
        {
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            this.protector = protectionProvider.CreateProtector("YouShouldNeverEverEverEverTellAnyoneYourSecrets");
            this.hashService = hashService;
            this.diService = diService;
        }

        // GET: api/caching
        // GET: api/Extras/caching
        [HttpGet("/api/caching")]
        [HttpGet("caching")]
        [ResponseCache(Duration = 15)]
        public ActionResult<DateTime> GetDateTime() => DateTime.UtcNow;

        // GET: api/dependency-injection
        // GET: api/Extras/dependency-injection
        [HttpGet("/api/dependency-injection")]
        [HttpGet("dependency-injection")]
        public ActionResult<string> GetDI() => diService.DoSomethingMethod("ayyylmao");

        [HttpGet("hash")]
        public ActionResult<string> GetHash()
        {
            string plainText = "Lorem Ipsum Dolor Sit Amet";
            var hash = hashService.HashString(plainText);

            var result = new
            {
                plainText,
                hash,
            };

            return Ok(result);
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

        [HttpGet("configuration-message")]
        public ActionResult<string> GetMessage() => configuration["message"];

        [HttpGet("caching-auth")]
        [ResponseCache(Duration = 15)]
        public ActionResult<DateTime> GetDateTimeAuth() => DateTime.UtcNow;

        //[HttpGet("check-bearer")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        //public ActionResult<IEnumerable<string>> Bearer()
        //{
        //    return new string[] { "value1", "value2" };
        //}
    }
}