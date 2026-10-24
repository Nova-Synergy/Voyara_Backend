using Voyara.SharedKernel.Entities;

namespace Voyara.Domain.Entities.LocalInfo;

public sealed class EmbassyContact : AuditableEntity
{
    public string Name { get; private set; } = string.Empty;

    public string HostCountryCode { get; private set; } = string.Empty;

    public string RepresentedNationalityCode { get; private set; } = string.Empty;

    public string PhoneNumber { get; private set; } = string.Empty;

    public string? Address { get; private set; }

    private EmbassyContact()
    {
    }

    private EmbassyContact(
        string name,
        string hostCountryCode,
        string representedNationalityCode,
        string phoneNumber,
        string? address)
    {
        Name = name;
        HostCountryCode = hostCountryCode;
        RepresentedNationalityCode = representedNationalityCode;
        PhoneNumber = phoneNumber;
        Address = address;
    }

    public static EmbassyContact Create(
        string name,
        string hostCountryCode,
        string representedNationalityCode,
        string phoneNumber,
        string? address = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(hostCountryCode);
        ArgumentException.ThrowIfNullOrWhiteSpace(representedNationalityCode);
        ArgumentException.ThrowIfNullOrWhiteSpace(phoneNumber);

        return new EmbassyContact(
            name.Trim(),
            ValidateCountryCode(hostCountryCode, nameof(hostCountryCode)),
            ValidateCountryCode(
                representedNationalityCode,
                nameof(representedNationalityCode)),
            phoneNumber.Trim(),
            string.IsNullOrWhiteSpace(address) ? null : address.Trim());
    }

    public void Update(
        string name,
        string phoneNumber,
        string? address)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(phoneNumber);

        Name = name.Trim();
        PhoneNumber = phoneNumber.Trim();
        Address = string.IsNullOrWhiteSpace(address)
            ? null
            : address.Trim();
    }

    private static string ValidateCountryCode(
        string countryCode,
        string parameterName)
    {
        var normalizedCode = countryCode.Trim().ToUpperInvariant();

        if (normalizedCode.Length != 2 ||
            normalizedCode.Any(c => c is < 'A' or > 'Z'))
        {
            throw new ArgumentException(
                "Country code must contain exactly two English letters.",
                parameterName);
        }

        return normalizedCode;
    }
}