import { useState, useCallback } from 'react'
import type { StateInfo } from './types'
import { STATE_NAMES } from './data/states'
import BrazilMap from './components/BrazilMap'
import Sidebar from './components/Sidebar'

export default function App() {
  const [selectedState, setSelectedState] = useState<StateInfo | null>(null)

  const handleStateSelect = useCallback((uf: string) => {
    const name = STATE_NAMES[uf]
    if (name) {
      setSelectedState((prev) =>
        prev?.uf === uf ? null : { uf, name }
      )
    }
  }, [])

  return (
    <div className="h-screen flex flex-col" style={{ background: '#1a1a1a' }}>
      <header
        className="flex items-center justify-center px-8 py-5 shrink-0"
        style={{
          background: '#1a1a1a',
          borderBottom: '1px solid #3a3a3a',
        }}
      >
        <div className="flex items-center gap-3">
          <span className="text-3xl">🗺️</span>
          <div className="flex flex-col items-center">
            <span
              className="text-2xl font-bold tracking-wider"
              style={{ color: '#e6edf3' }}
            >
              AtlasiDez
            </span>
            <span className="text-xs tracking-wide" style={{ color: '#8b949e' }}>
              Atlas de Cidades do Brasil
            </span>
          </div>
        </div>
      </header>

      <main className="flex flex-1 min-h-0">
        <BrazilMap
          selectedState={selectedState?.uf ?? null}
          onStateSelect={handleStateSelect}
        />
        <Sidebar selectedState={selectedState} />
      </main>
    </div>
  )
}
