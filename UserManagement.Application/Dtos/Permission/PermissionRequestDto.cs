using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Swashbuckle.AspNetCore.Annotations;

namespace UserManagement.Application.Dtos.Permission
{
    public class PermissionRequestDto
    {
        public required string Name { get; set; }
        public string? Description { get; set; }
    }
}
