using Organization.Service.DTOs;
using Organization.Service.Entities;

namespace Organization.Service.Mappers
{
    public static class CountryV3Mapper
    {
        public static CountryV3Dto ToDto(Country country)
        {
            if (country == null)
                throw new ArgumentNullException(nameof(country));

            return new CountryV3Dto
            {
                CountryId = country.Id,
                CountryCode = country.CountryCode,
                CountryName = country.CountryName,
                IsoCode = country.CountryCode3 ?? string.Empty,
                DialCode = country.DialCode ?? string.Empty,
                //CurrencyCode = country.CurrencyCode,
                //CurrencyName = country.CurrencyName,
                //CurrencySymbol = country.CurrencySymbol,
                //IsActive = country.IsActive
            };
        }
    }
}
