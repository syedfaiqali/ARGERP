import { useEffect, useState } from "react";
import dayjs from "dayjs";
import { getLanguageIntlLocale, getLanguageOption, isAppLanguage, type AppLanguage } from "@/constants/languages";

// The settings slice persists the selection here and the page reloads on change,
// so module-level formatters can read it without subscribing to the store.
export const LANGUAGE_STORAGE_KEY = "app_language";

export const getActiveLanguage = (): AppLanguage => {
  try {
    const saved = localStorage.getItem(LANGUAGE_STORAGE_KEY);
    return isAppLanguage(saved) ? saved : "en";
  } catch {
    return "en";
  }
};

// Languages that predate per-language formatting keep the locale their screen
// already used; only the newer languages switch to their own conventions.
export const getNumberLocale = (legacyLocale = "en-US") =>
  getLanguageIntlLocale(getActiveLanguage()) || legacyLocale;

// dayjs only bundles English; each locale is a separate module loaded on demand.
const DAYJS_LOCALE_LOADERS: Record<string, () => Promise<unknown>> = {
  de: () => import("dayjs/locale/de"),
  ru: () => import("dayjs/locale/ru"),
  fr: () => import("dayjs/locale/fr"),
  es: () => import("dayjs/locale/es"),
  it: () => import("dayjs/locale/it"),
  pl: () => import("dayjs/locale/pl"),
  nl: () => import("dayjs/locale/nl"),
  tr: () => import("dayjs/locale/tr"),
  "zh-cn": () => import("dayjs/locale/zh-cn"),
  pt: () => import("dayjs/locale/pt"),
  bn: () => import("dayjs/locale/bn"),
  id: () => import("dayjs/locale/id"),
  ja: () => import("dayjs/locale/ja"),
  ko: () => import("dayjs/locale/ko"),
};

const isDayjsLocaleLoaded = (locale: string) => Boolean((dayjs as unknown as { Ls: Record<string, unknown> }).Ls[locale]);

export const loadDayjsLocale = async (language: string): Promise<string | undefined> => {
  const locale = getLanguageOption(language)?.dayjsLocale;
  const loader = locale ? DAYJS_LOCALE_LOADERS[locale] : undefined;
  if (!locale || !loader) return undefined;
  if (!isDayjsLocaleLoaded(locale)) {
    try { await loader(); } catch { return undefined; }
  }
  return locale;
};

/**
 * Locale for MUI's AdapterDayjs (month and weekday names in date pickers).
 * Undefined keeps the adapter's English default, as before, for legacy languages.
 */
export const useDayjsAdapterLocale = () => {
  const language = getActiveLanguage();
  const target = getLanguageOption(language)?.dayjsLocale;
  const [locale, setLocale] = useState<string | undefined>(() =>
    target && isDayjsLocaleLoaded(target) ? target : undefined,
  );
  useEffect(() => {
    if (!target || locale === target) return;
    let active = true;
    loadDayjsLocale(language).then((loaded) => { if (active && loaded) setLocale(loaded); });
    return () => { active = false; };
  }, [language, target, locale]);
  return locale;
};
