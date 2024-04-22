using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Data.Domain;
using System.Reflection.Emit;

namespace Data
{


    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        { }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<LogException> LogExceptions { get; set; }
        public DbSet<RefreshToken> RefreshToken { get; set; }

        public DbSet<LogActivity> LogActivity { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)

        {
            base.OnModelCreating(builder);
        }




    }
    //interface for data intailization
    public interface IDbInitializer
    {
        void Initialize();
    }

    public class DbInitializer : IDbInitializer
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _db;

        public DbInitializer(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext db)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _db = db;
        }


        public void Initialize()
        {

            //create roles if they are not created
            if (!_roleManager.RoleExistsAsync("Admin").GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole("Admin")).GetAwaiter().GetResult();

                //if roles are not created, then we will create admin user as well

                _userManager.CreateAsync(new ApplicationUser
                {
                    UserName = "admin",
                    Email = "admin@.com",
                    PhoneNumber = "123456789",
                    Name = "Admin",
                }, "asd@123A").GetAwaiter().GetResult();

                ApplicationUser user1 = _db.ApplicationUsers.FirstOrDefault(u => u.Email == "admin@.com");

                _userManager.AddToRoleAsync(user1, "Admin").GetAwaiter().GetResult();
            }
            if (!_roleManager.RoleExistsAsync("Customer").GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole("Customer")).GetAwaiter().GetResult();
            }
            return;
        }
    }
}
