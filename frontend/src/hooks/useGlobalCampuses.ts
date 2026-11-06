import React from 'react'
import { fetchGlobalCampuses } from '@/constants/globalItems'
import { setCampusesRedux } from '@/store/authSlice'
import { useAppDispatch } from './useAppDispatch'
import { useAppSelector } from './useAppSelector'

export const useGlobalCampuses = () => {
  const dispatch = useAppDispatch()
  const campuses = useAppSelector((state) => state.auth.campuses)
  const [loading, setLoading] = React.useState(false)

  React.useEffect(() => {
    let mounted = true

    if (campuses.length > 0) return undefined

    setLoading(true)
    fetchGlobalCampuses()
      .then((items) => {
        if (mounted && items.length > 0) {
          dispatch(setCampusesRedux(items))
        }
      })
      .finally(() => {
        if (mounted) setLoading(false)
      })

    return () => {
      mounted = false
    }
  }, [campuses, dispatch])

  return { campuses, loading }
}
