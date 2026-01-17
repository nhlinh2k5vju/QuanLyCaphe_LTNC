using System;
using System.Windows.Forms;
using System.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

// ===== DAL =====
using DAL;
using DAL.Interface;
using DAL.Implementation;

// ===== BLL =====
using BLL.Interface;
using BLL.Implementation;

// ===== UI =====
using UI;

namespace Caphee2
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            var services = new ServiceCollection();

            services.AddDbContext<CoffeDbContext>(options =>
                options.UseSqlServer(
                    ConfigurationManager
                        .ConnectionStrings["QLSVConnection"]
                        .ConnectionString
                )
            );

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IMenuItemRepository, MenuItemRepository>();
            services.AddScoped<IPromotionRepository, PromotionRepository>();

            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IMenuService, MenuService>();
            services.AddScoped<IPromotionService, PromotionService>();

            services.AddTransient<LoginForm>();
            services.AddTransient<MainForm>();
            services.AddTransient<OrderForm>();

            var serviceProvider = services.BuildServiceProvider();

            var loginForm = serviceProvider.GetRequiredService<LoginForm>();
            Application.Run(loginForm);
        }
    }
}
