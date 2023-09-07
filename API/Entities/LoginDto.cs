﻿using System.ComponentModel.DataAnnotations;

namespace API.Entities
{
    public class LoginDto
    {

        [Required]
        public string username { get; set; }

        [Required]
        public string password { get; set; }
    }
}
