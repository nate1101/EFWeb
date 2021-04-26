using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EventFully.Models
{
        

    public class UserToken
    {
        public string UserId { get; set; }
        public string Token { get; set; }
        public DateTime TokenExpiration { get; set; }
    }
}
