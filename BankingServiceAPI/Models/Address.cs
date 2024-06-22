namespace BankingServiceAPI.Models;

public class Address
{
    public string? Street { get; private set; }
    public string? Number { get; private set; }
    public string? Complement { get; private set; }
    public string? City { get; private set; }
    public string? State { get; private set; }
    public string? PostalCode { get; private set; }
    public string? Country { get; private set; }

    public void SetStreet(string? street) => Street = street;
    public void SetNumber(string? number) => Number = number;
    public void SetComplement(string? complement) => Complement = complement;
    public void SetCity(string? city) => City = city;
    public void SetState(string? state) => State = state;
    public void SetPostalCode(string? postalCode) => PostalCode = postalCode;
    public void SetCountry(string? country) => Country = country;
}