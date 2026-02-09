import React, { useEffect, useState } from 'react'
import { useNavigate, useParams } from 'react-router-dom'
import { createFlag, listFlags, updateFlag } from '../services/api'

export default function CreateEditFlag() {
  const navigate = useNavigate()
  const params = useParams()
  const editId = params.id ? Number(params.id) : null

  const [name, setName] = useState('')
  const [rollout, setRollout] = useState(0)
  const [enabled, setEnabled] = useState(false)
  const [targetUserIds, setTargetUserIds] = useState('')

  useEffect(() => {
    if (editId) {
      load(editId)
    }
  }, [editId])

  async function load(id: number) {
    const flags = await listFlags()
    const f = flags.find((x) => x.id === id)
    if (!f) return
    setName(f.name)
    setRollout(f.rolloutPercentage)
    setEnabled(f.enabled)
    setTargetUserIds(f.targetUserIds || '')
  }

  async function submit(e: React.FormEvent) {
    e.preventDefault()
    if (!name) return
    if (editId) {
      await updateFlag(editId, { name, rolloutPercentage: rollout, enabled, targetUserIds })
    } else {
      await createFlag({ name, rolloutPercentage: rollout, enabled, targetUserIds })
    }
    navigate('/')
  }

  return (
    <section>
      <h2>{editId ? 'Edit Flag' : 'Create Flag'}</h2>
      <form onSubmit={submit}>
        <div className="form-row">
          <label>Feature name: </label>
          <input value={name} onChange={(e) => setName(e.target.value)} />
        </div>
        <div className="form-row">
          <label>Rollout %: </label>
          <input type="number" min={0} max={100} value={rollout} onChange={(e) => setRollout(Number(e.target.value))} />
        </div>
        <div className="form-row">
          <label>Enabled: </label>
          <input type="checkbox" checked={enabled} onChange={(e) => setEnabled(e.target.checked)} />
        </div>
        <div className="form-row">
          <label>Target User IDs (comma separated): </label>
          <input value={targetUserIds} onChange={(e) => setTargetUserIds(e.target.value)} />
        </div>
        <button type="submit">Save</button>
      </form>
    </section>
  )
}
