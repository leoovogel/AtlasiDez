import { useState } from 'react'
import type { City } from '../types'

interface CityListProps {
  cities: City[]
  loading: boolean
  skeletonCount: number
}

function CityItem({ city }: { city: City }) {
  const [hovered, setHovered] = useState(false)

  return (
    <div
      className="rounded-lg transition-colors duration-150 cursor-default"
      style={{
        background: hovered ? 'rgba(0, 212, 255, 0.06)' : 'rgba(255,255,255,0.025)',
        margin: '2px 16px',
        padding: '8px 16px',
      }}
      onMouseEnter={() => setHovered(true)}
      onMouseLeave={() => setHovered(false)}
    >
      <span className="text-sm" style={{ color: '#e6edf3' }}>
        {city.name}
      </span>
      <div
        className="overflow-hidden transition-all duration-200 ease-in-out"
        style={{
          maxHeight: hovered ? '24px' : '0',
          opacity: hovered ? 1 : 0,
        }}
      >
        <span className="text-xs mt-1 block" style={{ color: '#00d4ff' }}>
          IBGE: {city.ibge_code}
        </span>
      </div>
    </div>
  )
}

export default function CityList({ cities, loading, skeletonCount }: CityListProps) {
  if (loading) {
    return (
      <div className="flex-1 overflow-hidden">
        {Array.from({ length: skeletonCount }).map((_, i) => (
          <div
            key={i}
            className="rounded-lg animate-pulse"
            style={{ margin: '2px 16px', padding: '8px 16px' }}
          >
            <div
              className="h-4 rounded"
              style={{
                background: '#21262d',
              }}
            />
          </div>
        ))}
      </div>
    )
  }

  if (cities.length === 0) {
    return (
      <div className="flex-1 flex items-center justify-center px-6">
        <p className="text-sm" style={{ color: '#484f58' }}>
          Nenhuma cidade encontrada
        </p>
      </div>
    )
  }

  return (
    <div className="pt-1 pb-1">
      {cities.map((city) => (
        <CityItem key={city.ibge_code} city={city} />
      ))}
    </div>
  )
}
