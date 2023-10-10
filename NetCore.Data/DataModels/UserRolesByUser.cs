using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace NetCore.Data.DataModels
{
    public class UserRolesByUser
    {
        [Key, StringLength(50), Column(TypeName = "varchar(50)")]
        public string UserId { get; set; } = string.Empty;
        [Key, StringLength(50), Column(TypeName = "varchar(50)")]
        public string? RoleId { get; set; }
        [Required]
        public DateTime OwnUtcDate { get; set; }
        
        public virtual User User { get; set; }
        public virtual UserRole UserRole { get; set; }
        
    }
}
