using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserManagement.Application.Dtos.User
{
    public class UpdatePasswordDto
    {
        [Required]
        public string UserId { get; set; }

        [Required]
        [MinLength(8)]
        public string Password { get; set; }
    }

}
