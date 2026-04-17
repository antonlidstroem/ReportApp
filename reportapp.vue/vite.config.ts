import { defineConfig } from 'vite'
import plugin from '@vitejs/plugin-vue'

export default defineConfig({
  plugins: [plugin()],
  server: {
    port: 61704,
    proxy: {
      // Proxy /api calls to the .NET backend - avoids CORS in dev
      '/api': {
        target: 'http://localhost:5207',
        changeOrigin: true,
        secure: false,
      }
    }
  }
})
