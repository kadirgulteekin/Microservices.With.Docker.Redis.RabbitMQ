﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shared.Services
{
    public class SharedIdentityService : ISharedIdentityService
    {
        private IHttpContextAccessor _contextAccessor;

        public SharedIdentityService(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        //public string GetUserId => _contextAccessor.HttpContext.User.Claims.Where(
        //    x => x.Type == "sub").FirstOrDefault().Value;

        public string GetUserId => _contextAccessor.HttpContext.User.FindFirst("sub").Value;
    }
}
