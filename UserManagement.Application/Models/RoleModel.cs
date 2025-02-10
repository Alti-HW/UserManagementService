using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserManagement.Application.Models
{
    public class RoleModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; } // Nullable if missing in some responses
        public bool Composite { get; set; }
        public bool ClientRole { get; set; }
        public string ContainerId { get; set; }
    }

}
