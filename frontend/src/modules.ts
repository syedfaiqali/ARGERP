export const modules = [
 { key: 'hr', name: 'Human Resources', description: 'Employees, attendance, leave and payroll.' },
 { key: 'organization', name: 'Organization', description: 'Branches, departments, designations and reference data.' },
 { key: 'auth', name: 'Auth', description: 'Users, sign-in and account passwords.' },
] as const
export const moduleForGroup = (group: string) => group === 'Auth' ? 'auth' : group === 'Organization' ? 'organization' : 'hr'
