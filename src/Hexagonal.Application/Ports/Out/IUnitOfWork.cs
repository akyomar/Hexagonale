namespace Hexagonal.Application.Ports.Out;

/// <summary>
/// Port sortant : unité de travail pour committer une transaction.
/// </summary>
public interface IUnitOfWork
{
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
