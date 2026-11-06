// Single source of truth for the back-office UI languages. Each code matches a
// folder under public/locales/<code>/ holding the labels/messages/errors
// namespaces; a key missing there falls back to English through i18next and,
// failing that, translate() shows the English source text (keys are English).
export const APP_LANGUAGE_CODES = [
    'en', 'ur', 'ar', 'fa', 'ps', 'hi',
    'de', 'ru', 'fr', 'es', 'it', 'pl', 'nl', 'tr', 'zh', 'pt', 'bn', 'id', 'ja', 'ko',
] as const;

export type AppLanguage = typeof APP_LANGUAGE_CODES[number];

export interface AppLanguageOption {
    code: AppLanguage;
    // Legacy languages show an English name passed through translate(); the
    // languages added later show their own native name in every UI language.
    label: string;
    nativeName?: string;
    // BCP 47 locale for Intl date/number/currency formatting. Legacy languages
    // leave it unset so their existing fixed English formats stay unchanged.
    intlLocale?: string;
    // dayjs locale module name for date picker month/day names.
    dayjsLocale?: string;
    // Google Fonts family covering the script, loaded only while active.
    googleFont?: string;
    fontFamily?: string;
}

export const APP_LANGUAGES: AppLanguageOption[] = [
    { code: 'en', label: 'English (Legacy)' },
    { code: 'ur', label: 'Urdu' },
    { code: 'ar', label: 'Arabic' },
    { code: 'fa', label: 'Farsi' },
    { code: 'ps', label: 'Pashto' },
    { code: 'hi', label: 'Hindi' },
    { code: 'de', label: 'German', nativeName: 'Deutsch', intlLocale: 'de-DE', dayjsLocale: 'de' },
    {
        code: 'ru', label: 'Russian', nativeName: 'Русский', intlLocale: 'ru-RU', dayjsLocale: 'ru',
        // Outfit has no Cyrillic glyphs; Inter does, Noto Sans backs it up.
        googleFont: 'Noto+Sans:wght@400;500;600;700;800', fontFamily: '"Inter", "Noto Sans", sans-serif',
    },
    { code: 'fr', label: 'French', nativeName: 'Français', intlLocale: 'fr-FR', dayjsLocale: 'fr' },
    { code: 'es', label: 'Spanish', nativeName: 'Español', intlLocale: 'es-ES', dayjsLocale: 'es' },
    { code: 'it', label: 'Italian', nativeName: 'Italiano', intlLocale: 'it-IT', dayjsLocale: 'it' },
    { code: 'pl', label: 'Polish', nativeName: 'Polski', intlLocale: 'pl-PL', dayjsLocale: 'pl' },
    { code: 'nl', label: 'Dutch', nativeName: 'Nederlands', intlLocale: 'nl-NL', dayjsLocale: 'nl' },
    { code: 'tr', label: 'Turkish', nativeName: 'Türkçe', intlLocale: 'tr-TR', dayjsLocale: 'tr' },
    {
        code: 'zh', label: 'Chinese (Simplified)', nativeName: '中文（简体）', intlLocale: 'zh-CN', dayjsLocale: 'zh-cn',
        googleFont: 'Noto+Sans+SC:wght@400;500;700;800', fontFamily: '"Outfit", "Noto Sans SC", "Microsoft YaHei", "PingFang SC", sans-serif',
    },
    { code: 'pt', label: 'Portuguese', nativeName: 'Português', intlLocale: 'pt-PT', dayjsLocale: 'pt' },
    {
        code: 'bn', label: 'Bengali', nativeName: 'বাংলা', intlLocale: 'bn-BD', dayjsLocale: 'bn',
        googleFont: 'Noto+Sans+Bengali:wght@400;500;600;700;800', fontFamily: '"Outfit", "Noto Sans Bengali", "Nirmala UI", sans-serif',
    },
    { code: 'id', label: 'Indonesian', nativeName: 'Bahasa Indonesia', intlLocale: 'id-ID', dayjsLocale: 'id' },
    {
        code: 'ja', label: 'Japanese', nativeName: '日本語', intlLocale: 'ja-JP', dayjsLocale: 'ja',
        googleFont: 'Noto+Sans+JP:wght@400;500;700;800', fontFamily: '"Outfit", "Noto Sans JP", "Meiryo", "Hiragino Sans", sans-serif',
    },
    {
        code: 'ko', label: 'Korean', nativeName: '한국어', intlLocale: 'ko-KR', dayjsLocale: 'ko',
        googleFont: 'Noto+Sans+KR:wght@400;500;700;800', fontFamily: '"Outfit", "Noto Sans KR", "Malgun Gothic", "Apple SD Gothic Neo", sans-serif',
    },
];

export const DEFAULT_FONT_FAMILY = '"Outfit", "Inter", sans-serif';

const RTL_LANGUAGES: readonly string[] = ['ur', 'ar', 'fa', 'ps'];

export const isRtlLanguage = (language: string | null | undefined) =>
    RTL_LANGUAGES.includes(String(language || ''));

export const isAppLanguage = (value: unknown): value is AppLanguage =>
    typeof value === 'string' && (APP_LANGUAGE_CODES as readonly string[]).includes(value);

export const getLanguageOption = (language: string | null | undefined) =>
    APP_LANGUAGES.find((option) => option.code === language);

export const getLanguageFontFamily = (language: string | null | undefined) =>
    getLanguageOption(language)?.fontFamily || DEFAULT_FONT_FAMILY;

// Undefined for legacy languages: callers keep their existing format for those.
export const getLanguageIntlLocale = (language: string | null | undefined) =>
    getLanguageOption(language)?.intlLocale;
