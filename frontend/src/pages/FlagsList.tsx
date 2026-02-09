import React, { useEffect, useState } from 'react'
import { listFlags, toggleFlag, updateFlag } from '../services/api'
import { Flag } from '../types/flag'
import { Link } from 'react-router-dom'

export default function FlagsList() {
  const [flags, setFlags] = useState<Flag[]>([])
  const [loading, setLoading] = useState(false)

  useEffect(() => {
    refresh()
  }, [])

  async function refresh() {
    setLoading(true)
    const f = await listFlags()
    setFlags(f)
    setLoading(false)
  }

  async function handleToggle(id: number) {
    // optimistic UI
    setFlags((prev) => prev.map((p) => (p.id === id ? { ...p, enabled: !p.enabled } : p)))
    await toggleFlag(id)
    await refresh()
  }

  async function handleRolloutSave(id: number, value: number) {
    setFlags((prev) => prev.map((p) => (p.id === id ? { ...p, rolloutPercentage: value } : p)))
    await updateFlag(id, { rolloutPercentage: value })
  }

  return (
    <section>
      <h2>Flags</h2>
      <Link to="/create"><button>Create Flag</button></Link>
      {loading ? (
        <p>Loading...</p>
      ) : (
        <table>
          <thead>
            <tr>
              <th>Name</th>
              <th>Enabled</th>
              <th>Rollout %</th>
              <th>Actions</th>
            </tr>
          </thead>
          <tbody>
            {flags.map((f) => (
              <tr key={f.id}>
                <td>{f.name}</td>
                <td>{f.enabled ? 'Yes' : 'No'}</td>
                <td>
                  <InlineRollout value={f.rolloutPercentage} onSave={(v) => handleRolloutSave(f.id, v)} />
                </td>
                <td>
                  <button onClick={() => handleToggle(f.id)}>{f.enabled ? 'Turn off' : 'Turn on'}</button>
                  {' '}
                  <Link to={`/edit/${f.id}`}><button>Edit</button></Link>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      )}
    </section>
  )
}

function InlineRollout({ value, onSave }: { value: number; onSave: (v: number) => void }) {
  const [editing, setEditing] = useState(false)
  const [v, setV] = useState(value)

  useEffect(() => setV(value), [value])

  if (!editing) {
    return <span>{value}% <button onClick={() => setEditing(true)}>Edit</button></span>
  }

  return (
    <span>
      <input type="number" min={0} max={100} value={v} onChange={(e) => setV(Number(e.target.value))} />
      <button onClick={() => { onSave(v); setEditing(false) }}>Save</button>
      <button onClick={() => { setV(value); setEditing(false) }}>Cancel</button>
    </span>
  )
}
