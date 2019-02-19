using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StankinsHelperCommands;

namespace StankinsAliveMonitor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WhatExecuteSimpleController : ControllerBase
    {
        private static ResultTypeStankins[] results;
        private static object lockObj = new object();
        [HttpGet]
        public ResultTypeStankins[] Get()
        {
            if (results == null)
            {
                lock (lockObj)
                {
                    if (results != null)
                        return results;

                    results = FindAssembliesToExecute.AddReferences();
                }
            }
            
            return results;
        }




    }
}