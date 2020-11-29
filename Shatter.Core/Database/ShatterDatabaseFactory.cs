
using System;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Shatter.Core.Database
{
	public class ShatterDatabaseFactory : IDesignTimeDbContextFactory<ShatterDatabaseContext>
	{
		public ShatterDatabaseContext CreateDbContext(string[] args)
		{
			var db = ConfigurationManager.RegisterDatabase(null).Result;

			if (db is null)
			{
				throw new InvalidConfigException("Failed to register database");
			}

			var optionsBuilder = new DbContextOptionsBuilder<ShatterDatabaseContext>();
			optionsBuilder.UseSqlite(db.DataSource);

			return new ShatterDatabaseContext(optionsBuilder.Options);
		}

		private class InvalidConfigException : Exception
		{
			public InvalidConfigException() : base() { }
			public InvalidConfigException(string? message) : base(message) { }
		}
	}
}
