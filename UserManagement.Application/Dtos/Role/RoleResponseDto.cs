using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserManagement.Application.Models;

namespace UserManagement.Application.Dtos.Role
{
    public class RoleResponseDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ContainerId { get; set; }
        public List<RoleResponse> CompositeRoles { get; set; } = new List<RoleResponse>();
    }

}
