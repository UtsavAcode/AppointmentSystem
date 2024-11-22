﻿using AppointmentSystem.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace AppointmentSystem.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Post> Posts { get; set; }  
        public DbSet<Visitor> Visitors { get; set; }
    }
}
