import { defineConfig } from 'vite'
import tailwindcss from '@tailwindcss/vite'
import react from '@vitejs/plugin-react'
import { Agent } from 'https';

// https://vite.dev/config/
export default defineConfig({
  plugins: [react(), tailwindcss()],
  server: {
    proxy: {
      '/api': {
        target: 'https://localhost:7063',
        changeOrigin: true,
        secure: false,
        agent: new Agent({
          rejectUnauthorized: false
        })
      }
    }
  }
})
