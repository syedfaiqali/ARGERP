import {
  getExampleNumber,
  isSupportedCountry,
  parsePhoneNumberFromString,
} from "libphonenumber-js/max";
import type { CountryCode } from "libphonenumber-js";
import examples from "libphonenumber-js/mobile/examples";

export function institutionPhoneCountry(code?: string): CountryCode | undefined {
  const normalized = code?.trim().toUpperCase() === "UK" ? "GB" : code?.trim().toUpperCase();
  return normalized && isSupportedCountry(normalized) ? normalized as CountryCode : undefined;
}

export function institutionPhoneExample(code?: string): string {
  const country = institutionPhoneCountry(code);
  return country ? getExampleNumber(country, examples)?.formatNational() ?? "" : "";
}

export function institutionPhoneError(value: string, code?: string): string | undefined {
  if (!value.trim()) return "Contact No is required";
  const country = institutionPhoneCountry(code);
  if (!country) return "Select a country with a valid two-letter country code to validate Contact No";
  // Reject letters and embedded prose instead of extracting a number from them.
  if (!/^\+?[\d\s().-]+$/.test(value.trim())) return "Enter a valid Contact No for the selected country";
  const phone = parsePhoneNumberFromString(value.trim(), { defaultCountry: country, extract: false });
  if (!phone?.isValid() || phone.country !== country) return "Enter a valid Contact No for the selected country";
  return undefined;
}
