import React from 'react'
import { Routes, Route, Link } from 'react-router-dom'
import FlagsList from './pages/FlagsList'
import CreateEditFlag from './pages/CreateEditFlag'
import EvaluateFlag from './pages/EvaluateFlag'

export default function App() {
  return (
    <div className="app">
      <header>
        <h1>Feature Flags (Admin)</h1>
        <nav>
          <Link to="/">Flags</Link>
          {' | '}
          <Link to="/create">Create</Link>
          {' | '}
          <Link to="/evaluate">Evaluate</Link>
        </nav>
      </header>
      <main>
        <Routes>
          <Route path="/" element={<FlagsList />} />
          <Route path="/create" element={<CreateEditFlag />} />
          <Route path="/edit/:id" element={<CreateEditFlag />} />
          <Route path="/evaluate" element={<EvaluateFlag />} />
        </Routes>
      </main>
    </div>
  )
}
