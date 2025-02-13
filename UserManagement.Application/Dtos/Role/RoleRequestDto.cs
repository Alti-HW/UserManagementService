using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserManagement.Application.Dtos.Role
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using Microsoft.AspNetCore.Mvc.ModelBinding;
    using Swashbuckle.AspNetCore.Annotations;

    public class RoleRequestDto
    {
        /// <summary>
        /// Name of the role.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Description of the role.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Indicates whether this role is a composite role.
        /// </summary>
        ///  

        /// <summary>
        /// List of child roles that will be part of this composite role.
        /// </summary>
        public List<ClientRoleDto> CompositeRoles { get; set; } = new List<ClientRoleDto>();
    }

    /// <summary>
    /// Represents a client role that can be part of a composite role.
    /// </summary>
    public class ClientRoleDto
    {
        /// <summary>
        /// The unique ID of the client role in Keycloak.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The name of the client role.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Specifies that this is a client role.
        /// </summary>
        public bool ClientRole { get; set; } = true;

        /// <summary>
        /// The ID of the client that owns this role.
        /// </summary>
        public string ContainerId { get; set; }
    }


}
