﻿using System.Data.Entity;
using System.Data.Entity.SqlServer;

namespace LocalEF6.DAL
{
    public class SchoolConfiguration : DbConfiguration
    {
        public SchoolConfiguration()
        {
            SetExecutionStrategy("System.Data.SqlClient", 
                () => new SqlAzureExecutionStrategy());
        }
    }
}