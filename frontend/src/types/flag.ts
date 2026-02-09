export type Flag = {
  id: number
  name: string
  key: string
  enabled: boolean
  rolloutPercentage: number
  targetUserIds?: string | null
}
