using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using Microsoft.EntityFrameworkCore;
using StackOverflowLoaderAPI.Models.StackOverflowTags;
using static System.Net.Mime.MediaTypeNames;

namespace StackOverflowLoaderAPI.Data;

public partial class TagDataDataContext : DbContext
{
    private readonly IConfiguration _config;
    public string MyConnString { get; set; }
    
    public TagDataDataContext(string connString)
    {
        MyConnString = connString;
    }
        

    public TagDataDataContext(DbContextOptions<TagDataDataContext> options)
        : base(options)
    {
        
    }

    public virtual DbSet<Item> Items { get; set; }
    // public virtual DbSet<Collective> Collectives { get; set; }
    // public virtual DbSet<ExternalLink> ExternalLinks { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => optionsBuilder.UseSqlServer("Data Source = host.docker.internal,1433; User iD = sa; Password=Pass@word; Initial Catalog = SOTags; TrustServerCertificate=True;");
    //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //{
    //    if (!optionsBuilder.IsConfigured)
    //    {
    //        optionsBuilder.UseSqlServer(_config.);
    //    }
    //}
}
