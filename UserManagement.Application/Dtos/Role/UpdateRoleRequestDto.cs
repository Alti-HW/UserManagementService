using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserManagement.Application.Dtos.Role
{
    public class UpdateRoleRequestDto
    {
        public string Id { get; set; }            // Role ID (Required)
        public string NewRoleName { get; set; }   // New Role Name
        public string Description { get; set; }   // New Description
    }


}
