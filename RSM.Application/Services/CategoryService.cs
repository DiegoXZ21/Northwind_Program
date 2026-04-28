using RSM.Domain.Models;
using RSM.Infrastructure.Data;

namespace RSM.Application.Services
{
    public interface ICategoryService
    {
        List<Category> GetCategories();
    }

    public class CategoryService : ICategoryService
    {
        private readonly ApplicationDBContext _context;
        
        public CategoryService(ApplicationDBContext context)
        {
            _context = context;
        }

        public List<Category> GetCategories()
        {
            return _context.Categories.ToList();
        }
    }
}