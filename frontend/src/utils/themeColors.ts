/** Apply opacity to both theme CSS variables and ordinary data/chart colors. */
export const withColorOpacity = (color: string, opacity: number): string =>
    `color-mix(in srgb, ${color} ${Math.max(0, Math.min(1, opacity)) * 100}%, transparent)`;
