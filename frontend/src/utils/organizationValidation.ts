type Row = Record<string, unknown>;
type TextRule = { field: string; label: string; maxLength?: number; duplicate?: boolean; scope?: string[] };

// Cached rows give immediate feedback; the API remains authoritative when a
// record is outside the loaded page or another user saves at the same time.
export function organizationTextErrors(
  data: Row, rows: Row[], rules: TextRule[], idField = "id",
): string[] {
  const normalized = (value: unknown) => String(value ?? "").trim().toUpperCase();
  const others = rows.filter(row => Number(row[idField] ?? row.id ?? 0) !== Number(data[idField] ?? data.id ?? 0));
  const errors: string[] = [];
  for (const rule of rules) {
    const value = String(data[rule.field] ?? "").trim();
    if (rule.maxLength && value.length > rule.maxLength)
      errors.push(`${rule.label} must be ${rule.maxLength} characters or less`);
    if (rule.duplicate && value && others.some(row =>
      normalized(row[rule.field]) === normalized(value) &&
      (rule.scope ?? []).every(key => Number(row[key]) === Number(data[key]))))
      errors.push(`${rule.label} already exists`);
  }
  return errors;
}
