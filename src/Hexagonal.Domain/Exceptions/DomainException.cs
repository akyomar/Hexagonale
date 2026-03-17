namespace Hexagonal.Domain.Exceptions;

/// <summary>
/// Exception de base pour les erreurs métier du domaine.
/// </summary>
public class DomainException : Exception
{
    public string? Code { get; }

    public DomainException(string message, string? code = null, Exception? inner = null)
        : base(message, inner)
    {
        Code = code;
    }
}
