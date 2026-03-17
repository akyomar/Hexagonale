namespace Hexagonal.Application.Ports.Out;

/// <summary>
/// Port sortant : génération du numéro de facture (règle métier déléguée à l'infra).
/// </summary>
public interface IInvoiceNumberGenerator
{
    Task<string> GenerateAsync(CancellationToken cancellationToken = default);
}
