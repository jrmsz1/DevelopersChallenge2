using Microsoft.EntityFrameworkCore;
using SERVICESAPI.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SERVICESAPI.DataAccess
{
    public class OFXContext : DbContext
    {
        public OFXContext(DbContextOptions options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);

            builder.ForNpgsqlUseIdentityColumns();
            builder.ForSqlServerUseIdentityColumns();

            //Create Index
            builder.Entity<ACCOUNT>()
                    .HasIndex(t =>  t.FIRSTNAME);

            builder.Entity<ACCOUNT>()
                    .HasIndex(t => t.IDENTIFICATION);

            builder.Entity<ACCOUNT>()
                .HasIndex(t =>new { t.BANKID, t.ACCTID });

            builder.Entity<OFX_STMTTRN>()
                .HasIndex(t => new { t.AccountId, t.DTPOSTED, t.TRNTYPE, t.TRNAMT });
         

            //Many to Many
            //https://www.thereformedprogrammer.net/updating-many-to-many-relationships-in-entity-framework-core/

            builder.Entity<OFX_STMTTRN>()
               .HasOne(bc => bc.ACCOUNT)
                  .WithMany(w => w.STMTTRNS)
                    .HasForeignKey(b => b.AccountId);

            builder.Entity<OFX_STMTTRN>()
             .HasOne(bc => bc.BATCH)
                .WithMany(w => w.STMTTRNS)
                .HasForeignKey(h => h.BATCHId);


            builder.Entity<OFX_STMTTRN>()
           .Property(e => e.TRNAMT).HasColumnType("decimal(18,2)");

            builder.Entity<OFX_BATCH>()
           .Property(e => e.BALAMT).HasColumnType("decimal(18,2)");


            /* builder.Entity<ACCOUNT_HISTORY>()
                 .HasOne(bc => bc.ACCOUNT)
                    .WithMany(w => w.ACCCOUNTHISTORY)
                     .HasForeignKey(f => f.AccountId);*/


            //Remove delete cascade
            foreach (var relationship in builder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Cascade;
            }

            //SEED DATA - Xayah        
            builder.Seed(new List<ACCOUNT> {
              new ACCOUNT() {Id = 1, FIRSTNAME =  "Xayah", LASTNAME = "NIBO", DATECREATED = DateTimeOffset.UtcNow, BANKID = 341, ACCTID = 7037300576,  IDENTIFICATION = "0341-7037300576" } }
            );

        }

        DbSet<ACCOUNT> ACCOUNT { get; set; }        
        
        DbSet<OFX_BATCH> OFX_BATCH { get; set; }

        DbSet<OFX_STMTTRN> STMTTRN { get; set; }
        
    }
}
