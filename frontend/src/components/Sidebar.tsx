import { useState, useEffect, useCallback, useRef } from 'react'
import type { StateInfo, City } from '../types'
import { fetchCities } from '../services/api'
import CityList from './CityList'
import Pagination from './Pagination'

interface SidebarProps {
  selectedState: StateInfo | null
}

const PAGE_SIZE_OPTIONS = [10, 15, 20, 30, 50]
const DEFAULT_PAGE_SIZE = 15

function useDebounce<T>(value: T, delay: number): T {
  const [debounced, setDebounced] = useState(value)
  useEffect(() => {
    const timer = setTimeout(() => setDebounced(value), delay)
    return () => clearTimeout(timer)
  }, [value, delay])
  return debounced
}

export default function Sidebar({ selectedState }: SidebarProps) {
  const [cities, setCities] = useState<City[]>([])
  const [page, setPage] = useState(1)
  const [totalCount, setTotalCount] = useState(0)
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState<string | null>(null)
  const [searchTerm, setSearchTerm] = useState('')
  const [pageSize, setPageSize] = useState(DEFAULT_PAGE_SIZE)

  const debouncedSearch = useDebounce(searchTerm, 300)
  const totalPages = Math.ceil(totalCount / pageSize)

  const loadCities = useCallback(async (uf: string, p: number, ps: number, name?: string) => {
    setLoading(true)
    setError(null)
    try {
      const result = await fetchCities(uf, p, ps, name || undefined)
      setCities(result.items)
      setTotalCount(result.total_count)
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Erro desconhecido')
      setCities([])
      setTotalCount(0)
    } finally {
      setLoading(false)
    }
  }, [])

  useEffect(() => {
    setPage(1)
    setSearchTerm('')
    if (!selectedState) {
      setCities([])
      setTotalCount(0)
    }
  }, [selectedState])

  useEffect(() => {
    if (!selectedState) return
    setPage(1)
    loadCities(selectedState.uf, 1, pageSize, debouncedSearch || undefined)
  }, [selectedState, pageSize, debouncedSearch, loadCities])

  const handlePageChange = useCallback(
    (newPage: number) => {
      if (!selectedState) return
      setPage(newPage)
      loadCities(selectedState.uf, newPage, pageSize, debouncedSearch || undefined)
    },
    [selectedState, pageSize, debouncedSearch, loadCities]
  )

  const [dropdownOpen, setDropdownOpen] = useState(false)
  const dropdownRef = useRef<HTMLDivElement>(null)

  useEffect(() => {
    if (!dropdownOpen) return
    const handleClick = (e: MouseEvent) => {
      if (dropdownRef.current && !dropdownRef.current.contains(e.target as Node)) {
        setDropdownOpen(false)
      }
    }
    document.addEventListener('mousedown', handleClick)
    return () => document.removeEventListener('mousedown', handleClick)
  }, [dropdownOpen])

  const handlePageSizeChange = (newSize: number) => {
    setPageSize(newSize)
    setPage(1)
    setDropdownOpen(false)
  }

  return (
    <div
      className="flex flex-col min-h-0"
      style={{
        width: '380px',
        minWidth: '380px',
        background: '#161b22',
        borderLeft: '1px solid #21262d',
      }}
    >
      {!selectedState ? (
        <div className="flex-1 flex items-center justify-center">
          <div className="text-center px-8">
            <div className="text-4xl mb-3 opacity-40">🗺️</div>
            <p className="text-sm" style={{ color: '#484f58' }}>
              Selecione um estado no mapa para ver suas cidades
            </p>
          </div>
        </div>
      ) : (
        <>
          <div
            className="px-6 py-4 text-center"
            style={{
              borderBottom: '1px solid #21262d',
              borderTop: '1px solid #21262d',
            }}
          >
            <div className="flex items-center justify-center gap-2 mb-2">
              <div
                className="w-2.5 h-2.5 rounded-full"
                style={{ background: '#00d4ff' }}
              />
              <span
                className="text-lg font-semibold"
                style={{ color: '#e6edf3' }}
              >
                {selectedState.name}
              </span>
            </div>
            <span className="text-xs" style={{ color: '#8b949e' }}>
              {totalCount} cidade{totalCount !== 1 ? 's' : ''} encontrada
              {totalCount !== 1 ? 's' : ''}
            </span>
          </div>

          <div style={{ padding: '12px 16px' }}>
            <div
              className="flex items-center gap-3 rounded-xl"
              style={{
                background: '#0d1117',
                border: '1px solid #30363d',
                boxShadow: '0 4px 12px rgba(0, 0, 0, 0.4), 0 1px 3px rgba(0, 0, 0, 0.2)',
                padding: '10px 16px',
              }}
            >
              <span className="text-sm" style={{ color: '#484f58' }}>
                🔍
              </span>
              <input
                type="text"
                placeholder="Buscar cidade..."
                value={searchTerm}
                onChange={(e) => setSearchTerm(e.target.value)}
                className="flex-1 bg-transparent border-none outline-none text-sm placeholder:text-gray-600"
                style={{ color: '#e6edf3' }}
              />
            </div>
          </div>

          {error && (
            <div className="px-6 py-3">
              <p className="text-xs text-red-400">{error}</p>
            </div>
          )}

          <div className="flex-1 min-h-0 overflow-y-auto">
            <CityList cities={cities} loading={loading} skeletonCount={pageSize} />
          </div>

          <div
            className="flex items-center justify-between shrink-0"
            style={{
              borderTop: '1px solid #21262d',
              padding: '12px 16px',
            }}
          >
            <div className="relative" ref={dropdownRef}>
              <button
                onClick={() => setDropdownOpen(!dropdownOpen)}
                className="flex items-center gap-2 rounded-lg text-sm font-medium transition-colors duration-150 cursor-pointer"
                style={{
                  background: '#21262d',
                  border: '1px solid #30363d',
                  color: '#e6edf3',
                  padding: '8px 14px',
                }}
              >
                <span style={{ color: '#8b949e' }}>Exibir:</span>
                <span style={{ color: '#00d4ff' }}>{pageSize}</span>
                <svg
                  width="10"
                  height="6"
                  viewBox="0 0 10 6"
                  fill="none"
                  style={{
                    transform: dropdownOpen ? 'rotate(180deg)' : 'rotate(0deg)',
                    transition: 'transform 150ms ease',
                    marginLeft: '2px',
                  }}
                >
                  <path d="M1 1L5 5L9 1" stroke="#8b949e" strokeWidth="1.5" strokeLinecap="round" strokeLinejoin="round" />
                </svg>
              </button>

              {dropdownOpen && (
                <div
                  className="absolute bottom-full left-0 mb-2 rounded-lg overflow-hidden"
                  style={{
                    background: '#1c2128',
                    border: '1px solid #30363d',
                    boxShadow: '0 8px 24px rgba(0, 0, 0, 0.5)',
                    minWidth: '120px',
                    zIndex: 50,
                  }}
                >
                  {PAGE_SIZE_OPTIONS.map((size) => (
                    <button
                      key={size}
                      onClick={() => handlePageSizeChange(size)}
                      className="w-full text-left text-sm font-medium transition-colors duration-100 cursor-pointer"
                      style={{
                        background: size === pageSize ? 'rgba(0, 212, 255, 0.1)' : 'transparent',
                        color: size === pageSize ? '#00d4ff' : '#e6edf3',
                        borderLeft: size === pageSize ? '2px solid #00d4ff' : '2px solid transparent',
                        padding: '8px 16px',
                      }}
                      onMouseEnter={(e) => {
                        if (size !== pageSize) {
                          e.currentTarget.style.background = 'rgba(255,255,255,0.05)'
                        }
                      }}
                      onMouseLeave={(e) => {
                        e.currentTarget.style.background = size === pageSize ? 'rgba(0, 212, 255, 0.1)' : 'transparent'
                      }}
                    >
                      {size} itens
                    </button>
                  ))}
                </div>
              )}
            </div>

            <Pagination
              page={page}
              totalPages={totalPages}
              onPageChange={handlePageChange}
            />
          </div>
        </>
      )}
    </div>
  )
}
