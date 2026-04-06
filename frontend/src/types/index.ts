export interface City {
  name: string
  ibge_code: string
}

export interface PagedResult {
  items: City[]
  page: number
  page_size: number
  total_count: number
}

export interface StateInfo {
  uf: string
  name: string
}
