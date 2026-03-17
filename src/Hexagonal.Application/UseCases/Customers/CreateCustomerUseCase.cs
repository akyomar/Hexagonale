using Hexagonal.Application.Common;
using Hexagonal.Application.DTOs.Customers;
using Hexagonal.Application.Ports.In;
using Hexagonal.Application.Ports.Out;
using Hexagonal.Domain.Entities;
using Hexagonal.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace Hexagonal.Application.UseCases.Customers;

public class CreateCustomerUseCase : ICreateCustomerInputPort
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateCustomerUseCase> _logger;

    public CreateCustomerUseCase(
        ICustomerRepository customerRepository,
        IUnitOfWork unitOfWork,
        ILogger<CreateCustomerUseCase> logger)
    {
        _customerRepository = customerRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<CustomerDto>> ExecuteAsync(CreateCustomerRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var email = Email.Create(request.Email);
            Address? address = null;
            if (!string.IsNullOrWhiteSpace(request.Street) && !string.IsNullOrWhiteSpace(request.City))
            {
                address = new Address(
                    request.Street,
                    request.City,
                    request.PostalCode ?? "",
                    request.Country ?? "");
            }

            var customer = Customer.Create(request.Name, email, address);
            await _customerRepository.AddAsync(customer, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Client créé : {CustomerId}", customer.Id);

            return Result<CustomerDto>.Success(MapToDto(customer));
        }
        catch (ArgumentException ex)
        {
            return Result<CustomerDto>.Failure(ex.Message, "VALIDATION");
        }
    }

    private static CustomerDto MapToDto(Customer c) => new()
    {
        Id = c.Id,
        Name = c.Name,
        Email = c.Email.Value,
        Street = c.Address?.Street,
        City = c.Address?.City,
        PostalCode = c.Address?.PostalCode,
        Country = c.Address?.Country,
        CreatedAtUtc = c.CreatedAtUtc
    };
}
