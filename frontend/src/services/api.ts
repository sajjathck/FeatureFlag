import axios from 'axios'
import { Flag } from '../types/flag'

const api = axios.create({ baseURL: 'http://localhost:5000/api' })

export async function listFlags(): Promise<Flag[]> {
  const res = await api.get('/flags')
  return res.data
}

export async function createFlag(payload: { name: string; rolloutPercentage: number; enabled: boolean; targetUserIds?: string }) {
  const res = await api.post('/flags', payload)
  return res.data
}

export async function updateFlag(id: number, payload: Partial<{ name: string; rolloutPercentage: number; enabled: boolean; targetUserIds: string }>) {
  const res = await api.put(`/flags/${id}`, payload)
  return res.data
}

export async function toggleFlag(id: number) {
  const res = await api.post(`/flags/${id}/toggle`)
  return res.data
}

export async function evaluateFlag(flagName: string, userId: string) {
  const res = await api.get('/flags/evaluate', { params: { flagName, userId } })
  return res.data
}

export default api
