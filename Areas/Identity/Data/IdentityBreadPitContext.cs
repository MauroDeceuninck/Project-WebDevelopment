using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Project.Data;

public class IdentityBreadPitContext : IdentityDbContext<IdentityUser>
{
    public IdentityBreadPitContext(DbContextOptions<IdentityBreadPitContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
    }
}
