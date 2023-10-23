using Microsoft.EntityFrameworkCore;
using NetCore.Data.DataModels;

namespace NetCore.Services.Data
{
    /// <summary>
    /// CodeFirstDbContext : 자식클래스
    /// DbContext : 부모클래스
    /// </summary>
    public class CodeFirstDbContext : DbContext
    {
        /// <summary>
        /// 생성자 상속
        /// </summary>
        /// <param name="options"></param>
        public CodeFirstDbContext(DbContextOptions<CodeFirstDbContext> options) : base(options)
        {
            
        }
        // DB 테이블 리스트 지정
        public DbSet<User> Users { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<UserRolesByUser> UserRolseByUsers { get; set; }
        // Method 상속
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // 4가지 작업
            // Db 테이블 이름 변경
            modelBuilder.Entity<User>().ToTable(name: "User");
            modelBuilder.Entity<UserRole>().ToTable(name: "UserRole");
            modelBuilder.Entity<UserRolesByUser>().ToTable(name: "UserRolesByUser");
            // 복합키 지정
            modelBuilder.Entity<UserRolesByUser>().HasKey(c => new { c.UserId, c.RoleId });
            // 컬럼 기본값 지정
            modelBuilder.Entity<User>(e =>
            {
                e.Property(c => c.IsMembershipWithdrawn).HasDefaultValue(value: false);
                //e.Property(c => c.JoinedUtcDate).HasDefaultValue(value: DateTime.UtcNow); 실제 동작 안됨
            });
            // 인덱스 지정
            modelBuilder.Entity<User>().HasIndex(c => new {c.UserEmail}).IsUnique(unique:true);
        }
    }
}
