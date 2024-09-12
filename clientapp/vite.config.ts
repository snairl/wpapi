import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

// https://vitejs.dev/config/
export default defineConfig({
  plugins: [react()],
  server:{
    host: true,
    proxy: {
      '/api': {
        target: 'http://net8:5171',
        // changeOrigin: true,
        // rewrite: (path) => path.replace(/^\/api/, '')
      }
    },
    watch: {
      usePolling: true,       
      interval: 3000,         
    },
  }
})
