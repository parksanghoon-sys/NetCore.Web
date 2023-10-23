﻿using System.ComponentModel.DataAnnotations;

namespace NetCore.Data.Classes
{
    public class User
    {
        [Key]
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string? Password { get; set; }
        public string UserEmail { get; set; }
        public int AccessFailedCount { get; set; }
        public bool IsMembershipWithdrawn { get; set; }
        public System.DateTime JoinedUtcDate { get; set; }        
        

        public virtual ICollection<UserRolesByUser> UserRolesByUsers { get; set; }
    }
}