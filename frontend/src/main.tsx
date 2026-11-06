import React from 'react'
import ReactDOM from 'react-dom/client'
import { Provider } from 'react-redux'
import { BrowserRouter } from 'react-router-dom'
import { CssBaseline, ThemeProvider } from '@mui/material'
import { Toaster } from 'react-hot-toast'
import { store } from './store/store'
import { getTheme } from './theme'
import { AdaptiveTranslationProvider } from './components/AdaptiveTranslationProvider'
import App from './App'
import './index.css'
ReactDOM.createRoot(document.getElementById('root')!).render(
  <React.StrictMode><Provider store={store}><BrowserRouter><ThemeProvider theme={getTheme('ltr', 'en')}><CssBaseline/><AdaptiveTranslationProvider><Toaster/><App/></AdaptiveTranslationProvider></ThemeProvider></BrowserRouter></Provider></React.StrictMode>,
)
