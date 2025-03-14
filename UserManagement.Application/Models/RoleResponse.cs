﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserManagement.Application.Models
{
    public class RoleResponse
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Composite { get; set; }
        public bool ClientRole { get; set; }
        public string ContainerId { get; set; }

        public List<RoleResponse> CompositeRoles { get; set; } = new List<RoleResponse>();
    }
}
