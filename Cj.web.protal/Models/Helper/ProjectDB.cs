using Cj.web.protal.Models;
using System.Data.Entity;


namespace Cj.web.Models
{
    public class ProjectDb : DbContext
    {
        public ProjectDb() : base("TEL_DMS_ConnectionString")
        {
            this.Configuration.ProxyCreationEnabled = false;
            Database.SetInitializer<ProjectDb>(null);
            this.Database.CommandTimeout = 180;
        }
        //Cj user model
        public DbSet<User> Users { get; set; }
        public DbSet<DistributionPlanDetails> DistributionPlanDetails { get; set; }
        public DbSet<TDAttendanceDetail> TDAttendanceDetail { get; set; }
        public DbSet<MarketGroup> MarketGroup { get; set; }
        public DbSet<Sales> Sales { get; set; }
        
    }
}