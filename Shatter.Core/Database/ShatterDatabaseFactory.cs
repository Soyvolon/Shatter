using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Shatter.Core.Database
{
	public class ShatterDatabaseFactory : IDesignTimeDbContextFactory<ShatterDatabaseContext>
	{
		public ShatterDatabaseContext CreateDbContext(string[] args)
		{
			var db = ConfigurationManager.RegisterDatabase(null).Result;

			if (db is null) return null;

			var optionsBuilder = new DbContextOptionsBuilder<ShatterDatabaseContext>();
			optionsBuilder.UseSqlite(db.DataSource);

			return new ShatterDatabaseContext(optionsBuilder.Options);
		}
	}
}
