using System;
using System.Collections.Generic;
using System.Text;

using DAL.Entities;
using DAL.Interface;
using Microsoft.EntityFrameworkCore;

namespace DAL.Implementation
{
    public class MenuItemRepository : IMenuItemRepository
    {
        private readonly CoffeDbContext _context;

        public MenuItemRepository(CoffeDbContext context)
        {
            _context = context;
        }

        public async Task<List<MenuItem>> GetAllActiveAsync()
        {
            return await _context.MenuItems
                .Where(m => m.Status)
                .Include(m => m.MenuItemSizes)
                .ToListAsync();
        }

        public async Task<MenuItem?> GetByIdAsync(int id)
        {
            return await _context.MenuItems
                .Include(m => m.MenuItemSizes)
                .FirstOrDefaultAsync(m => m.MenuItemId == id);
        }

        public async Task AddAsync(MenuItem item)
        {
            await _context.MenuItems.AddAsync(item);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(MenuItem item)
        {
            _context.MenuItems.Update(item);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var item = await _context.MenuItems.FindAsync(id);
            if (item != null)
            {
                item.Status = false;
                await _context.SaveChangesAsync();
            }
        }
        public async Task AddWithSizesAsync(MenuItem item, List<MenuItemSize> sizes)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                await _context.MenuItems.AddAsync(item);
                await _context.SaveChangesAsync();

                foreach (var size in sizes)
                {
                    size.MenuItemId = item.MenuItemId;
                    await _context.MenuItemSizes.AddAsync(size);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        public async Task UpdateWithSizesAsync(MenuItem item, List<MenuItemSize> sizes)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                _context.MenuItems.Update(item);
                await _context.SaveChangesAsync();

                var oldSizes = await _context.MenuItemSizes
                    .Where(x => x.MenuItemId == item.MenuItemId)
                    .ToListAsync();

                _context.MenuItemSizes.RemoveRange(oldSizes);
                await _context.SaveChangesAsync();

                foreach (var size in sizes)
                {
                    size.MenuItemId = item.MenuItemId;
                    await _context.MenuItemSizes.AddAsync(size);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        public async Task SetStatusAsync(int menuItemId, bool status)
        {
            var item = await _context.MenuItems.FindAsync(menuItemId);
            if (item == null) return;

            item.Status = status;
            await _context.SaveChangesAsync();
        }
        public async Task<List<MenuItem>> GetAllAsync()
        {
            return await _context.MenuItems
                .Include(m => m.MenuItemSizes)
                .ToListAsync();
        }

    }
}

