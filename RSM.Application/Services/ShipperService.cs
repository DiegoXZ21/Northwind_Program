using RSM.Infrastructure.Data;
using RSM.Application.Dtos;

namespace RSM.Application.Services
{
    public interface IShipperService
    {
        List<ShipperDto> GetShippers();
    }

    public class ShipperService : IShipperService
    {
        private readonly ApplicationDBContext _context;
        public ShipperService(ApplicationDBContext context)
        {
            _context = context;
        }

        public List<ShipperDto> GetShippers()
        {
            return _context.Shippers
                .Select(s => new ShipperDto
                {
                    ShipperId = s.ShipperId,
                    CompanyName = s.CompanyName
                })
                .ToList();
        }
    }
}