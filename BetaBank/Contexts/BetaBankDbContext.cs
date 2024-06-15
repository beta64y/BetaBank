using BetaBank.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BetaBank.Contexts
{
    public class BetaBankDbContext : IdentityDbContext<AppUser>
    {
        public BetaBankDbContext(DbContextOptions<BetaBankDbContext> options) : base(options)
        {

        }
    }
}
