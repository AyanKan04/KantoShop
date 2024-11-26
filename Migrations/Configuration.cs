namespace KantoShop.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<KantoShop.Models.ShopKantoContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;  // Cho phép tự động áp dụng migrations khi thay đổi model
            AutomaticMigrationDataLossAllowed = true;  // Cho phép mất dữ liệu nếu cần thiết
        }

        protected override void Seed(KantoShop.Models.ShopKantoContext context)
        {
            // Thêm dữ liệu mẫu nếu cần thiết
        }
    }
}
