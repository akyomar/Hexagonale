using Hexagonal.Application.Common;
using Hexagonal.Application.DTOs.Customers;
using Hexagonal.Application.Ports.In;
using Hexagonal.Application.Ports.Out;
using Microsoft.Extensions.Logging;

namespace Hexagonal.Application.UseCases.Customers;

public class GetCustomerByIdUseCase : IGetCustomerByIdInputPort
{
    private readonly ICustomerRepository _customerRepository;
    private readonly ILogger<GetCustomerByIdUseCase> _logger;

    public GetCustomerByIdUseCase(ICustomerRepository customerRepository, ILogger<GetCustomerByIdUseCase> logger)
    {
        _customerRepository = customerRepository;
        _logger = logger;
    }

    public async Task<Result<CustomerDto>> ExecuteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var customer = await _customerRepository.GetByIdAsync(id, cancellationToken);
        if (customer == null)
            return Result<CustomerDto>.Failure($"Client non trouvé : {id}", "NOT_FOUND");

        return Result<CustomerDto>.Success(new CustomerDto
        {
            Id = customer.Id,
            Name = customer.Name,
            Email = customer.Email.Value,
            Street = customer.Address?.Street,
            City = customer.Address?.City,
            PostalCode = customer.Address?.PostalCode,
            Country = customer.Address?.Country,
            CreatedAtUtc = customer.CreatedAtUtc
        });
    }
}
