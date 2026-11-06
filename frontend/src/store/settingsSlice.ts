import { createSlice, PayloadAction } from '@reduxjs/toolkit';
import type { ApplicationTheme } from '../theme';
import { isAppLanguage, type AppLanguage } from '../constants/languages';

interface SettingsState {
    language: AppLanguage;
    theme: ApplicationTheme;
    organizationType: 'School' | 'College' | 'Corporate';
    terminology: Record<string, string>;
    translations: Record<string, string>;
}

const storedLanguage = localStorage.getItem('app_language');
// Ignore a stale or hand-edited value so the app never boots into an unknown language.
const savedLanguage = isAppLanguage(storedLanguage) ? storedLanguage : null;
const savedTheme = localStorage.getItem('app_theme') as ApplicationTheme | null;

const initialState: SettingsState = {
    language: savedLanguage || 'en',
    theme: savedTheme && ['green', 'blue', 'purple', 'orange', 'pink'].includes(savedTheme) ? savedTheme : 'green',
    organizationType: 'School',
    terminology: {
        'LABEL_LEVEL': 'Class',
        'LABEL_SUBLEVEL': 'Section',
        'LABEL_INSTRUCTOR': 'Teacher',
        'LABEL_LEARNER': 'Student',
    },
    translations: {}
};

const settingsSlice = createSlice({
    name: 'settings',
    initialState,
    reducers: {
        setLanguage: (state, action: PayloadAction<AppLanguage>) => {
            state.language = action.payload;
            localStorage.setItem('app_language', action.payload);
            localStorage.setItem('i18nextLng', action.payload);
            // In a real app, we might trigger a re-fetch of terminology or translations here
        },
        setTheme: (state, action: PayloadAction<ApplicationTheme>) => {
            state.theme = action.payload;
            localStorage.setItem('app_theme', action.payload);
        },
        setOrganizationType: (state, action: PayloadAction<'School' | 'College' | 'Corporate'>) => {
            state.organizationType = action.payload;
            // Update terminology based on type
            switch (action.payload) {
                case 'School':
                    state.terminology = {
                        'LABEL_LEVEL': 'Class',
                        'LABEL_SUBLEVEL': 'Section',
                        'LABEL_INSTRUCTOR': 'Teacher',
                        'LABEL_LEARNER': 'Student',
                    };
                    break;
                case 'College':
                    state.terminology = {
                        'LABEL_LEVEL': 'Course',
                        'LABEL_SUBLEVEL': 'Batch',
                        'LABEL_INSTRUCTOR': 'Lecturer',
                        'LABEL_LEARNER': 'Student',
                    };
                    break;
                case 'Corporate':
                    state.terminology = {
                        'LABEL_LEVEL': 'Training Module',
                        'LABEL_SUBLEVEL': 'Department',
                        'LABEL_INSTRUCTOR': 'Instructor',
                        'LABEL_LEARNER': 'Employee',
                    };
                    break;
            }
        },
        updateTerminology: (state, action: PayloadAction<Record<string, string>>) => {
            state.terminology = { ...state.terminology, ...action.payload };
        },
        cacheTranslation: (state, action: PayloadAction<{ key: string; value: string }>) => {
            state.translations[action.payload.key] = action.payload.value;
        },
        cacheTranslationsBatch: (state, action: PayloadAction<Record<string, string>>) => {
            state.translations = { ...state.translations, ...action.payload };
        }
    },
});

export const { setLanguage, setTheme, setOrganizationType, updateTerminology, cacheTranslation, cacheTranslationsBatch } = settingsSlice.actions;
export default settingsSlice.reducer;
