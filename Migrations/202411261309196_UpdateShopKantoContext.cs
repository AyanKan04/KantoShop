namespace KantoShop.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateShopKantoContext : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AdminUsers", "PhoneNumber", c => c.String());
            AddColumn("dbo.AdminUsers", "Address", c => c.String());
            AddColumn("dbo.AdminUsers", "Avatar", c => c.String());
            AddColumn("dbo.AdminUsers", "BackgroundImage", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AdminUsers", "BackgroundImage");
            DropColumn("dbo.AdminUsers", "Avatar");
            DropColumn("dbo.AdminUsers", "Address");
            DropColumn("dbo.AdminUsers", "PhoneNumber");
        }
    }
}
