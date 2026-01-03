using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Core.Dtos
{
    public class UpdateRoleDto
    {
        [Required(ErrorMessage = "UserName is required")]
        public string UserName { get; set; }
        // admin,manager,user ---and must be uppercase
        public string NewRole { get; set; }
    }
}


