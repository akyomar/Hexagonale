using System.Text.RegularExpressions;

namespace Hexagonal.Domain.ValueObjects;

/// <summary>
/// Value Object pour une adresse email avec validation.
/// </summary>
public sealed class Email : IEquatable<Email>
{
    private static readonly Regex EmailRegex = new(
        @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public string Value { get; }

    private Email(string value)
    {
        Value = value;
    }

    public static Email Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("L'email ne peut pas être vide.", nameof(value));
        if (!EmailRegex.IsMatch(value))
            throw new ArgumentException("Format d'email invalide.", nameof(value));
        return new Email(value.Trim().ToLowerInvariant());
    }

    public bool Equals(Email? other) => other is not null && Value == other.Value;
    public override bool Equals(object? obj) => obj is Email other && Equals(other);
    public override int GetHashCode() => Value.GetHashCode();
    public override string ToString() => Value;
}
