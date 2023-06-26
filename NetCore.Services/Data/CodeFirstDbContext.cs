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
        public CodeFirstDbContext(DbContextOptions<CodeFirstDbContext> options) : base(options)
        {
            
        }
        // DB 테이블 리스트 지정
        public DbSet<User> Users { get; set; }
        // Method 상속
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // 4가지 작업
            // Db 테이블 이름 변경
            modelBuilder.Entity<User>().ToTable(name: "User");
            // 복합키 지정
            modelBuilder.Entity<UserRolesByUser>().HasKey(c => new { c.UserId, c.RoleId });
            // 컬럼 기본값 지정
            modelBuilder.Entity<User>(e =>
            {
                e.Property(c => c.IsMembershipWithdrawn).HasDefaultValue(value: false);
            });
            // 인덱스 지정
            modelBuilder.Entity<User>().HasIndex(c => new {c.UserEmail});
        }
    }
}
