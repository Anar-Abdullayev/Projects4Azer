import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import { Agent } from 'https';

// https://vite.dev/config/
export default defineConfig({
  plugins: [react()],
  server: {
    proxy: {
      '/api': {
        target: 'https://localhost:7063', // your ASP.NET Core HTTPS port
        changeOrigin: true,
        secure: false,
        agent: new Agent({
          rejectUnauthorized: false
        })
      }
    }
  }
})
