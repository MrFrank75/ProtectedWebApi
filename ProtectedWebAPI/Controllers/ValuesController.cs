using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace ProtectedWebAPI.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class ValuesController : Controller
    {
        // GET api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            //this should be readable only if I have the scope "this or that". How do I do this?
            System.Security.Claims.Claim scope = User.Claims.SingleOrDefault(claim => claim.Type.Equals("scope", StringComparison.CurrentCultureIgnoreCase));
            if (scope == null)
                throw new Exception("Scope is missing");
            if (scope.Value.Equals("scope.readaccess", StringComparison.CurrentCultureIgnoreCase) == false)
                throw new Exception("Invalid scope");

            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
