using System;
using System.Collections.Generic;
using System.Text;

using DAL.Entities;

namespace BLL.Interface
{
    public interface IMenuService
    {
        Task<List<MenuItem>> GetAllActiveAsync();
        Task AddMenuItemAsync(MenuItem item);
        Task UpdateMenuItemAsync(MenuItem item);
        Task DisableMenuItemAsync(int id);

        Task CreateMenuAsync(MenuItem menuItem, List<MenuItemSize> sizes);
        Task UpdateMenuAsync(MenuItem menuItem, List<MenuItemSize> sizes);
        Task SetMenuStatusAsync(int menuItemId, bool status);
        Task<List<MenuItem>> GetAllAsync();
    }

}

