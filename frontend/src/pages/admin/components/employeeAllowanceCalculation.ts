export interface AllowanceAmount {
  value: number | "";
  isPercentage?: boolean;
}

// Convert decimal values without binary floating point rounding at half cents.
const decimalParts = (value: number) => {
  const [mantissa, exponent = "0"] = value.toString().toLowerCase().split("e");
  const [whole, fraction = ""] = mantissa.split(".");
  const scale = fraction.length - Number(exponent);
  const coefficient = BigInt(whole + fraction);
  return scale < 0
    ? { coefficient: coefficient * 10n ** BigInt(-scale), scale: 0 }
    : { coefficient, scale };
};

export const calculateAllowanceAmount = (line: AllowanceAmount, baseSalary: number): number => {
  if (!line.isPercentage) return Number(line.value) || 0;
  const rate = Number(line.value);
  if (!Number.isFinite(baseSalary) || baseSalary < 0 || !Number.isFinite(rate) || rate < 0) return 0;
  const base = decimalParts(baseSalary);
  const percentage = decimalParts(rate);
  // Dividing the product by 100 and converting to cents cancel each other.
  const divisor = 10n ** BigInt(base.scale + percentage.scale);
  const cents = (base.coefficient * percentage.coefficient + divisor / 2n) / divisor;
  return Number(cents) / 100;
};

export const formatPayrollAmount = (value: number | "", percentage = false): string =>
  value === "" ? "" : new Intl.NumberFormat("en-US", {
    useGrouping: !percentage, minimumFractionDigits: 2, maximumFractionDigits: 2,
  }).format(value);

export const serializeAllowanceAmount = (line: AllowanceAmount) => ({
  value: Number(line.value),
  isPercentage: Boolean(line.isPercentage),
});
