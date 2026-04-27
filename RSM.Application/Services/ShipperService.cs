using RSM.Domain.Models;
using RSM.Infrastructure.Data;

namespace RSM.Application.Services
{
    public interface IShipperService
    {
        List<Shipper> GetShippers();
    }

    public class ShipperService : IShipperService
    {
        private readonly ApplicationDBContext _context;
        public ShipperService(ApplicationDBContext context)
        {
            _context = context;
        }

        public List<Shipper> GetShippers()
        {
            return _context.Shippers.ToList();
        }
    }
}