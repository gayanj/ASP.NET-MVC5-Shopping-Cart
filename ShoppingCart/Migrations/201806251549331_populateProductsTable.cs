namespace ShoppingCart.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class populateProductsTable : DbMigration
    {
        public override void Up()
        {
            Sql("INSERT INTO Products " +
                "(Name, Description, Price, IsOrderable, Stock, Catergory)" +
                "VALUES " +
                "('Nike Air Jordan', 'These were worn by Jordan himself', 100.0, 1, 100, 'SHOES')");
            Sql("INSERT INTO Products " +
                "(Name, Description, Price, IsOrderable, Stock, Catergory)" +
                "VALUES " +
                "('Puma', 'Authentic Puma shoes', 70.0, 1, 100, 'SHOES')");
            Sql("INSERT INTO Products " +
                "(Name, Description, Price, IsOrderable, Stock, Catergory)" +
                "VALUES " +
                "('Dota2 Keyboard', 'Custom Keyboard for playing Dota2', 130.0, 1, 100, 'ELECTRONICS')");
            Sql("INSERT INTO Products " +
                "(Name, Description, Price, IsOrderable, Stock, Catergory)" +
                "VALUES " +
                "('Abans Fan', 'Three speed stand fan', 200.0, 1, 100, 'ELECTRONICS')");
        }
        
        public override void Down()
        {
        }
    }
}
