interface PaginationProps {
  page: number
  totalPages: number
  onPageChange: (page: number) => void
}

export default function Pagination({
  page,
  totalPages,
  onPageChange,
}: PaginationProps) {
  if (totalPages <= 1) return null

  const prev = page > 1 ? page - 1 : null
  const next = page < totalPages ? page + 1 : null

  return (
    <div className="flex items-center" style={{ gap: '4px' }}>
      <NavButton
        onClick={() => onPageChange(1)}
        disabled={page <= 1}
        aria-label="Primeira página"
      >
        «
      </NavButton>

      <NavButton
        onClick={() => onPageChange(page - 1)}
        disabled={page <= 1}
        aria-label="Página anterior"
      >
        ‹
      </NavButton>

      {prev && (
        <PageButton onClick={() => onPageChange(prev)}>
          {prev}
        </PageButton>
      )}

      <PageButton active>{page}</PageButton>

      {next && (
        <PageButton onClick={() => onPageChange(next)}>
          {next}
        </PageButton>
      )}

      <NavButton
        onClick={() => onPageChange(page + 1)}
        disabled={page >= totalPages}
        aria-label="Próxima página"
      >
        ›
      </NavButton>

      <NavButton
        onClick={() => onPageChange(totalPages)}
        disabled={page >= totalPages}
        aria-label="Última página"
      >
        »
      </NavButton>
    </div>
  )
}

function PageButton({
  children,
  onClick,
  active = false,
}: {
  children: React.ReactNode
  onClick?: () => void
  active?: boolean
}) {
  return (
    <button
      onClick={onClick}
      className="rounded-md flex items-center justify-center text-xs font-medium transition-colors duration-150 cursor-pointer"
      style={{
        width: '30px',
        height: '30px',
        minWidth: '30px',
        background: active ? '#00d4ff' : '#21262d',
        color: active ? '#0d1117' : '#e6edf3',
        fontWeight: active ? 600 : 400,
      }}
    >
      {children}
    </button>
  )
}

function NavButton({
  children,
  onClick,
  disabled = false,
  ...props
}: {
  children: React.ReactNode
  onClick: () => void
  disabled?: boolean
  [key: string]: unknown
}) {
  return (
    <button
      onClick={onClick}
      disabled={disabled}
      className="rounded-md flex items-center justify-center text-sm transition-colors duration-150 cursor-pointer disabled:cursor-not-allowed"
      style={{
        width: '30px',
        height: '30px',
        minWidth: '30px',
        background: 'transparent',
        color: disabled ? '#484f58' : '#8b949e',
      }}
      {...props}
    >
      {children}
    </button>
  )
}
