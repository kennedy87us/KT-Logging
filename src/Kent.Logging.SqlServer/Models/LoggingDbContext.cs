namespace Kent.Logging.SqlServer.Models
{
    using Kent.Logging.Abstractions;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using System;

    /// <summary>
    ///     Represents the logging db context.
    /// </summary>
    public partial class LoggingDbContext : DbContext
    {
        /// <summary>
        ///     Constructor method.
        /// </summary>
        public LoggingDbContext()
        {
        }

        /// <summary>
        ///     Constructor method.
        /// </summary>
        /// <param name="options">The options to be used by a <see cref="DbContext"/>.</param>
        public LoggingDbContext(DbContextOptions<LoggingDbContext> options) : base(options)
        {
        }

        /// <summary>
        ///     A DbSet can be used to query and save instance of <see cref="Models.LogMessageEntry"/>.
        /// </summary>
        public virtual DbSet<LogMessageEntry> LogMessageEntry { get; set; }

        /// <summary>
        ///     Override this method to configure the database (and other options) to be used for this context.
        /// </summary>
        /// <param name="optionsBuilder">A builder used to create or modify options for this context.</param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                throw new Exception("No DB configured");
            }
        }

        /// <summary>
        ///     Override this method to further configure the model that was discovered by convention from the entity types.
        /// </summary>
        /// <param name="modelBuilder">The builder being used to construct the model for this context.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LogMessageEntry>(entity =>
            {
                entity.Property(e => e.LogId).IsRequired().HasMaxLength(36)
                                             .IsUnicode(false).IsFixedLength();

                entity.Property(e => e.CategoryName).IsRequired().HasMaxLength(256);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Exception).HasConversion(
                    value => JsonConvert.SerializeObject(value),
                    value => JsonConvert.DeserializeObject<LogException>(value));

                entity.Property(e => e.LogLevel).IsRequired().HasMaxLength(16)
                                                .IsUnicode(false).HasConversion(
                    value => value.ToString(),
                    value => Enum.Parse<LogLevel>(value));

                entity.Property(e => e.Message).IsRequired().HasMaxLength(2000);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        /// <summary>
        ///     Parial method to configure the model.
        /// </summary>
        /// <param name="modelBuilder">The builder being used to construct the model for this context.</param>
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}