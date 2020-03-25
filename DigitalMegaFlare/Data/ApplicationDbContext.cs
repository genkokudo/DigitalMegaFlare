using System;
using System.Collections.Generic;
using System.Text;
using DigitalMegaFlare.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DigitalMegaFlare.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public DbSet<TestData> TestDatas { get; set; }
        public DbSet<ExcelFile> ExcelFiles { get; set; }
        public DbSet<RazorFile> RazorFiles { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
    }
}
