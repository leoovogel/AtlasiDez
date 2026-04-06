import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import tailwindcss from '@tailwindcss/vite'

export default defineConfig({
  plugins: [react(), tailwindcss()],
  server: {
    port: 3000,
    proxy: {
      '/api': {
        target: 'https://atlasidez-api.proudriver-27e08694.brazilsouth.azurecontainerapps.io',
        changeOrigin: true
      }
    }
  }
})
