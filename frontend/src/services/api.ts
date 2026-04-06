import type { PagedResult } from '../types'

export async function fetchCities(
  uf: string,
  page: number = 1,
  pageSize: number = 10,
  name?: string
): Promise<PagedResult> {
  const params = new URLSearchParams({
    page: String(page),
    pageSize: String(pageSize),
  })

  if (name?.trim()) {
    params.set('name', name.trim())
  }

  const response = await fetch(`/api/cities/${uf}?${params}`)

  if (!response.ok) {
    const error = await response.json().catch(() => null)
    throw new Error(
      error?.detail || `Erro ao buscar cidades (${response.status})`
    )
  }

  return response.json()
}
