using FluentValidation;
using Hexagonal.Application.Ports.In;
using Hexagonal.Application.UseCases.Customers;
using Hexagonal.Application.UseCases.Invoices;
using Hexagonal.Application.UseCases.Orders;
using Hexagonal.Application.UseCases.Products;
using Hexagonal.Application.Validation;
using Microsoft.Extensions.DependencyInjection;

namespace Hexagonal.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Ports entrants (use cases)
        services.AddScoped<ICreateCustomerInputPort, CreateCustomerUseCase>();
        services.AddScoped<IGetCustomerByIdInputPort, GetCustomerByIdUseCase>();
        services.AddScoped<ICreateProductInputPort, CreateProductUseCase>();
        services.AddScoped<ICreateOrderInputPort, CreateOrderUseCase>();
        services.AddScoped<IGetOrderByIdInputPort, GetOrderByIdUseCase>();
        services.AddScoped<IConfirmOrderInputPort, ConfirmOrderUseCase>();
        services.AddScoped<IGenerateInvoiceInputPort, GenerateInvoiceUseCase>();

        // Validation FluentValidation
        services.AddValidatorsFromAssemblyContaining<CreateCustomerRequestValidator>();

        return services;
    }
}
