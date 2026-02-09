import React, { useEffect, useState } from 'react'
import { listFlags, evaluateFlag } from '../services/api'
import { Flag } from '../types/flag'

export default function EvaluateFlag() {
  const [flags, setFlags] = useState<Flag[]>([])
  const [selected, setSelected] = useState<string>('')
  const [userId, setUserId] = useState('')
  const [result, setResult] = useState<any | null>(null)

  useEffect(() => {
    load()
  }, [])

  async function load() {
    const f = await listFlags()
    setFlags(f)
    if (f.length) setSelected(f[0].key)
  }

  async function onEvaluate() {
    const r = await evaluateFlag(selected, userId || 'anonymous')
    setResult(r)
  }

  return (
    <section>
      <h2>Evaluate</h2>
      <div className="form-row">
        <label>Feature: </label>
        <select value={selected} onChange={(e) => setSelected(e.target.value)}>
          {flags.map((f) => (
            <option key={f.id} value={f.key}>{f.name}</option>
          ))}
        </select>
      </div>
      <div className="form-row">
        <label>User ID: </label>
        <input value={userId} onChange={(e) => setUserId(e.target.value)} placeholder="user-123" />
        <button onClick={onEvaluate}>Evaluate</button>
      </div>

      {result && (
        <div>
          <h3>Evaluation Result</h3>
          <pre>{JSON.stringify(result, null, 2)}</pre>
        </div>
      )}
    </section>
  )
}
