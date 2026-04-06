import { useState, memo, useCallback } from 'react'
import {
  ComposableMap,
  Geographies,
  Geography,
  Annotation,
} from 'react-simple-maps'
import { STATE_NAMES } from '../data/states'

const GEO_URL = '/brazil-states.json'

const PROJECTION_CONFIG = {
  rotate: [54, 14, -2] as [number, number, number],
  center: [0, 0] as [number, number],
  scale: 650,
}

const SMALL_UF_POSITION: Record<string, [number, number]> = {
  DF: [-41.5, -15.8],
  SE: [-10, 0],
  AL: [-5, -5],
  RN: [-20, -30],
  PB: [0, -10],
  PE: [10, -5],
  ES: [-36, -20],
  RJ: [-39.5, -19.5]
}

const SUBJECT_OVERRIDE: Record<string, [number, number]> = {
  PE: [-37.5, -8.3],
}

const UF_POSITION: Record<string, [number, number]> = {
  RS: [-50, -35],
  SC: [-40, -30],
  MG: [-30, -17],
  BA: [-35, -22],
  PI: [-30, 0],
  TO: [-50, 0],
  PA: [-55, 10],
  RR: [-55, 0],
  AP: [-45, 5]
}

interface BrazilMapProps {
  selectedState: string | null
  onStateSelect: (uf: string) => void
}

function BrazilMap({ selectedState, onStateSelect }: BrazilMapProps) {
  const [hoveredState, setHoveredState] = useState<string | null>(null)
  const [tooltipPos, setTooltipPos] = useState({ x: 0, y: 0 })

  const handleMouseMove = useCallback(
    (e: React.MouseEvent) => {
      if (hoveredState) {
        setTooltipPos({ x: e.clientX, y: e.clientY })
      }
    },
    [hoveredState]
  )

  return (
    <div className="relative flex-1 flex items-center justify-center min-h-0" onMouseMove={handleMouseMove}>
      <ComposableMap
        projection="geoMercator"
        projectionConfig={PROJECTION_CONFIG}
        width={600}
        height={700}
        className="w-full h-full max-h-[calc(100vh-64px)]"
      >
        <Geographies geography={GEO_URL}>
          {({ geographies }) =>
            geographies.flatMap((geo) => {
              const uf = geo.properties.sigla as string
              const isSelected = selectedState === uf
              const rawCenter = getCenter(geo)
              const center = SUBJECT_OVERRIDE[uf] || rawCenter
              const labelStyle = {
                fontSize: 10,
                fill: isSelected ? '#00d4ff' : '#8b949e',
                fontWeight: isSelected ? 700 : 400,
              }

              const elements = [
                <Geography
                  key={`geo-${uf}`}
                  geography={geo}
                  onClick={() => onStateSelect(uf)}
                  onMouseEnter={() => setHoveredState(uf)}
                  onMouseLeave={() => setHoveredState(null)}
                  className="cursor-pointer outline-none focus:outline-none"
                  style={{
                    default: {
                      fill: isSelected ? 'rgba(0, 212, 255, 0.25)' : '#1a2332',
                      stroke: isSelected ? '#00d4ff' : '#2d4a6f',
                      strokeWidth: isSelected ? 1.5 : 0.5,
                      transition: 'all 0.2s ease',
                    },
                    hover: {
                      fill: isSelected
                        ? 'rgba(0, 212, 255, 0.35)'
                        : 'rgba(0, 212, 255, 0.12)',
                      stroke: '#00d4ff',
                      strokeWidth: 1,
                      transition: 'all 0.2s ease',
                    },
                    pressed: {
                      fill: 'rgba(0, 212, 255, 0.4)',
                      stroke: '#00d4ff',
                      strokeWidth: 1.5,
                    },
                  }}
                />,
              ]

              if (SMALL_UF_POSITION[uf]) {
                elements.push(
                  <Annotation
                    key={`label-${uf}`}
                    subject={center}
                    dx={SMALL_UF_POSITION[uf][0] - center[0]}
                    dy={SMALL_UF_POSITION[uf][1] - center[1]}
                    connectorProps={{
                      stroke: isSelected ? '#00d4ff' : '#3a5a7c',
                      strokeWidth: 0.5,
                    }}
                  >
                    <text
                      textAnchor="middle"
                      alignmentBaseline="middle"
                      className="select-none pointer-events-none"
                      style={labelStyle}
                    >
                      {uf}
                    </text>
                  </Annotation>
                )
              } else if (UF_POSITION[uf]) {
                elements.push(
                  <Annotation
                    key={`label-${uf}`}
                    subject={center}
                    dx={UF_POSITION[uf][0] - center[0]}
                    dy={UF_POSITION[uf][1] - center[1]}
                    connectorProps={{ stroke: 'none' }}
                  >
                    <text
                      textAnchor="middle"
                      alignmentBaseline="middle"
                      className="select-none pointer-events-none"
                      style={labelStyle}
                    >
                      {uf}
                    </text>
                  </Annotation>
                )
              } else {
                elements.push(
                  <Annotation
                    key={`label-${uf}`}
                    subject={center}
                    dx={0}
                    dy={0}
                    connectorProps={{ stroke: 'none' }}
                  >
                    <text
                      textAnchor="middle"
                      alignmentBaseline="middle"
                      className="select-none pointer-events-none"
                      style={labelStyle}
                    >
                      {uf}
                    </text>
                  </Annotation>
                )
              }

              return elements
            })
          }
        </Geographies>
      </ComposableMap>

      {hoveredState && (
        <div
          className="fixed z-50 px-3 py-2 rounded-md text-sm pointer-events-none"
          style={{
            left: tooltipPos.x + 12,
            top: tooltipPos.y - 10,
            background: '#161b22',
            border: '1px solid #00d4ff',
            boxShadow: '0 4px 12px rgba(0,0,0,0.4)',
          }}
        >
          <span className="font-semibold" style={{ color: '#00d4ff' }}>
            {STATE_NAMES[hoveredState] || hoveredState}
          </span>
          <span className="text-gray-400 ml-2 text-xs">({hoveredState})</span>
        </div>
      )}
    </div>
  )
}

function getCenter(geo: { geometry: { type: string; coordinates: number[][][][] | number[][][] } }): [number, number] {
  const coords =
    geo.geometry.type === 'MultiPolygon'
      ? (geo.geometry.coordinates as number[][][][]).flat(2)
      : (geo.geometry.coordinates as number[][][]).flat()

  const lngs = coords.map((c) => c[0])
  const lats = coords.map((c) => c[1])

  return [
    (Math.min(...lngs) + Math.max(...lngs)) / 2,
    (Math.min(...lats) + Math.max(...lats)) / 2,
  ]
}

export default memo(BrazilMap)
