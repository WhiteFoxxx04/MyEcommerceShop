namespace MyShop.DataAccess.SQL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddShoppingCart : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CartItems",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        CartId = c.String(),
                        ProductId = c.String(),
                        Quantity = c.Int(nullable: false),
                        CreatedAt = c.DateTimeOffset(nullable: false, precision: 7),
                        ShoppingCart_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ShoppingCarts", t => t.ShoppingCart_Id)
                .Index(t => t.ShoppingCart_Id);
            
            CreateTable(
                "dbo.ShoppingCarts",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        CreatedAt = c.DateTimeOffset(nullable: false, precision: 7),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CartItems", "ShoppingCart_Id", "dbo.ShoppingCarts");
            DropIndex("dbo.CartItems", new[] { "ShoppingCart_Id" });
            DropTable("dbo.ShoppingCarts");
            DropTable("dbo.CartItems");
        }
    }
}
