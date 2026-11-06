import React from 'react'
const translate = (value: string | number | null | undefined) => value == null ? '' : String(value)
const context = { translate, isRTL: false, language: 'en', isCorporate: true }
export const useTranslation = () => context
export const useOptionalTranslation = useTranslation
export const isCorporateExecutiveData = () => true
export const isUniversityExecutiveData = () => false
export const normalizeInstitutionType = (value = '') => value.toLowerCase().replace(/[^a-z]/g, '')
export const syncExecutiveInstitutionType = (..._args: unknown[]) => {}
export const AdaptiveTranslationProvider = ({ children }: { children: React.ReactNode }) => <>{children}</>
export const T = ({ children }: { children: React.ReactNode }) => <>{children}</>
export const Translated = T
