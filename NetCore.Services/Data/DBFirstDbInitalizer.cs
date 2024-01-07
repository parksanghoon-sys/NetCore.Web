using NetCore.Data.Classes;
using NetCore.Services.Interfaces;

namespace NetCore.Services.Data
{
    public class DBFirstDbInitalizer
    {
        private readonly DBFirstDbContext _context;
        private readonly IPasswordHasher _hasher;
        public DBFirstDbInitalizer(DBFirstDbContext context, IPasswordHasher passwordHasher)
        {
            _context = context;
            _hasher = passwordHasher;
        }
        /// <summary>
        /// 초기 데이터를 심어주는 작업 Method
        /// </summary>
        public async Task<int> PlantSeedData()
        {
            string userId = "testid1";
            string password = "123456";
            var utcDate = DateTime.UtcNow;
            var passwordInfo = _hasher.SetPasswordInfo(userId, password);
            _context.Database.EnsureCreated();

            if (_context.Users.Any() == false)
            {
                var users = new List<User>()
                {
                    new User()
                    {
                        UserId = userId.ToLower(),
                        UserName = "Seed 사용자",
                        UserEmail = "power93266@nave.com",
                        GUIDSalt = passwordInfo.GUIDSalt,
                        RNGSalt = passwordInfo.RNGSalt,
                        PasswordHash = passwordInfo.PasswodHash,
                        AccessFailedCount = 0,
                        IsMembershipWithdrawn = false,
                        JoinedUtcDate = utcDate
                    }
                };
                await _context.Users.AddRangeAsync(users);                
            }
            if (_context.UserRoles.Any() == false)
            {
                var UserRoles = new List<UserRole>()
                {
                    new UserRole()
                    {
                        RoleId = "AssociateUser",
                        RoleName = "준사용자",
                        RolePriority = 1,
                        ModifiedUtcDate = utcDate
                    },
                    new UserRole()
                    {
                        RoleId = "GeneralUser",
                        RoleName = "일반사용자",
                        RolePriority = 2,
                        ModifiedUtcDate = utcDate
                    },
                    new UserRole()
                    {
                        RoleId = "SuperUser",
                        RoleName = "향상된 사용자",
                        RolePriority = 3,
                        ModifiedUtcDate = utcDate
                    },
                    new UserRole()
                    {
                        RoleId = "SystemUser",
                        RoleName = "시스템 사용자",
                        RolePriority = 4,
                        ModifiedUtcDate = utcDate
                    }

                };
                await _context.UserRoles.AddRangeAsync(UserRoles);                
            }
            if (_context.UserRolesByUsers.Any() == false)
            {
                var userRolesByUsers = new List<UserRolesByUser>()
                {
                    new UserRolesByUser()
                    {
                        UserId = userId.ToLower(),
                        RoleId = "GeneralUser",
                        OwnedUtcDate = utcDate,
                    },
                    new UserRolesByUser()
                    {
                        UserId = userId.ToLower(),
                        RoleId = "SuperUser",
                        OwnedUtcDate = utcDate,
                    },
                    new UserRolesByUser()
                    {
                        UserId = userId.ToLower(),
                        RoleId = "SystemUser",
                        OwnedUtcDate = utcDate,
                    }
                };
                await _context.UserRolesByUsers.AddRangeAsync(userRolesByUsers);                
            }
            var rowAffect = await _context.SaveChangesAsync();
            if(rowAffect > 0)
                return rowAffect;
            return 0;

        }
    }
}
