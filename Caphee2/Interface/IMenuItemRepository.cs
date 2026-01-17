using System;
using System.Collections.Generic;
using System.Text;

using DAL.Entities;

namespace DAL.Interface
{
    public interface IMenuItemRepository
    {
        Task<List<MenuItem>> GetAllActiveAsync();
        Task<MenuItem?> GetByIdAsync(int id);
        Task AddAsync(MenuItem item);
        Task UpdateAsync(MenuItem item);
        Task DeleteAsync(int id);
        Task AddWithSizesAsync(MenuItem item, List<MenuItemSize> sizes);
        Task UpdateWithSizesAsync(MenuItem item, List<MenuItemSize> sizes);
        Task SetStatusAsync(int menuItemId, bool status);
        Task<List<MenuItem>> GetAllAsync();

    }
}

