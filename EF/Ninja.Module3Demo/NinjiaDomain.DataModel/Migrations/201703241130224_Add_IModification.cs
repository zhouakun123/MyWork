namespace NinjiaDomain.DataModel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_IModification : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Clans", "DateModified", c => c.DateTime(nullable: false));
            AddColumn("dbo.Clans", "DateCreated", c => c.DateTime(nullable: false));
            AddColumn("dbo.Ninjas", "DateModified", c => c.DateTime(nullable: false));
            AddColumn("dbo.Ninjas", "DateCreated", c => c.DateTime(nullable: false));
            AddColumn("dbo.NinjaEquipments", "DateModified", c => c.DateTime(nullable: false));
            AddColumn("dbo.NinjaEquipments", "DateCreated", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.NinjaEquipments", "DateCreated");
            DropColumn("dbo.NinjaEquipments", "DateModified");
            DropColumn("dbo.Ninjas", "DateCreated");
            DropColumn("dbo.Ninjas", "DateModified");
            DropColumn("dbo.Clans", "DateCreated");
            DropColumn("dbo.Clans", "DateModified");
        }
    }
}
