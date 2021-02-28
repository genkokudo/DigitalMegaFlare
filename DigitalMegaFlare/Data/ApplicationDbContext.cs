using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using DigitalMegaFlare.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DigitalMegaFlare.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {

        private readonly IHttpContextAccessor _httpContextAccessor;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IHttpContextAccessor httpContextAccessor)
            : base(options)
        {
            _httpContextAccessor = httpContextAccessor;

        }

        public DbSet<TestData> TestDatas { get; set; }
        public DbSet<ExcelFile> ExcelFiles { get; set; }
        public DbSet<RazorFile> RazorFiles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ユーザ名はユニーク制約を付ける
            modelBuilder.Entity<TestData>()
                .HasIndex(m => m.Name)
                .IsUnique();

            //modelBuilder.Entity<TestData>().HasQueryFilter(weight => weight.UserId == long.Parse(_httpContextAccessor.HttpContext.User.GetUserId()));

            //modelBuilder.Entity<TestData>().HasQueryFilter(b => EF.Property<string>(b, "userId") == _userId);

            //modelBuilder.Entity<TestData>().HasQueryFilter(p => !p.IsDeleted);
        }
    }

    public static class ClaimsPrincipalExtensions
    {
        public static string GetUserId(this ClaimsPrincipal principal)
        {
            return principal.FindFirstValue(ClaimTypes.NameIdentifier);
        }
    }

}
