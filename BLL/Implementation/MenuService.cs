using System; 
using System.Collections.Generic;
using System.Linq; 
using System.Threading.Tasks;
using BLL.Interface;
using DAL.Entities;
using DAL.Interface;

namespace BLL.Implementation
{
    public class MenuService : IMenuService
    {
        private readonly IMenuItemRepository _menuRepo;

        public MenuService(IMenuItemRepository menuRepo)
        {
            _menuRepo = menuRepo;
        }

        public async Task<List<MenuItem>> GetAllActiveAsync()
        {
            return await _menuRepo.GetAllActiveAsync();
        }

        public async Task AddMenuItemAsync(MenuItem item)
        {
            await _menuRepo.AddAsync(item);
        }

        public async Task UpdateMenuItemAsync(MenuItem item)
        {
            await _menuRepo.UpdateAsync(item);
        }

        public async Task DisableMenuItemAsync(int id)
        {
            await _menuRepo.DeleteAsync(id);
        }

        public async Task CreateMenuAsync(MenuItem menuItem, List<MenuItemSize> sizes)
        {
            if (string.IsNullOrWhiteSpace(menuItem.Name))
                throw new Exception("Tên món không hợp lệ");

            if (!sizes.Any())
                throw new Exception("Phải có ít nhất 1 size");

            await _menuRepo.AddWithSizesAsync(menuItem, sizes);
        }

        public async Task UpdateMenuAsync(MenuItem menuItem, List<MenuItemSize> sizes)
        {
            await _menuRepo.UpdateWithSizesAsync(menuItem, sizes);
        }

        public async Task SetMenuStatusAsync(int id, bool status)
        {
            var item = await _menuRepo.GetByIdAsync(id);

            if (item != null)
            {
                item.Status = status;
                await _menuRepo.UpdateAsync(item);
            }
        }
        public async Task<List<MenuItem>> GetAllAsync()
        {
            return await _menuRepo.GetAllAsync();
        }
    }
}