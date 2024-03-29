namespace LocalEF6.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MaxLengthOfNamesAdded : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Student", "FirstMidName", c => c.String(maxLength: 15));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Student", "FirstMidName", c => c.String(maxLength: 10));
        }
    }
}
