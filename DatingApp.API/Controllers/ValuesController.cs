﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingApp.API.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly DataContext _dataContext;
        public ValuesController(DataContext dataContext)
        {
            this._dataContext = dataContext;
        }
        // GET api/values
        [HttpGet]
        public async Task<IActionResult> GetValues()
        {
            var values = await this._dataContext.Values.ToListAsync();
            return Ok(values); 
        }

        // GET api/values/5
        [AllowAnonymous]
        [HttpGet("{id}")]
        public IEnumerable<string> GetValue(int id)
        {
            // var value = await this._dataContext.Values.FirstOrDefaultAsync(val=> val.Id == id);
            IEnumerable<string> omri = new string[]{"wtv","Wtv"};  
            string[] wtv = new string[]{"asd"};
            wtv.ToList();
            omri.ToList();
            
            return new string[] {"omri","guy"};
            // return Ok(value);
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
