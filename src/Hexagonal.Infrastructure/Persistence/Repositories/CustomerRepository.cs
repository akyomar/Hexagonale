using Hexagonal.Application.Ports.Out;
using Hexagonal.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Hexagonal.Infrastructure.Persistence.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly ApplicationDbContext _context;

    public CustomerRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Customer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await _context.Customers.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default) =>
        await _context.Customers.AnyAsync(c => c.Id == id, cancellationToken);

    public async Task<Customer> AddAsync(Customer customer, CancellationToken cancellationToken = default)
    {
        await _context.Customers.AddAsync(customer, cancellationToken);
        return customer;
    }
}
