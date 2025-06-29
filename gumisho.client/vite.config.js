import { fileURLToPath, URL } from 'node:url';
import { defineConfig } from 'vite';
import plugin from '@vitejs/plugin-vue';
import fs from 'fs';
import path from 'path';
import child_process from 'child_process';
import { env } from 'process';

const isProduction = process.env.NODE_ENV === 'production';

const baseFolder =
  env.APPDATA !== undefined && env.APPDATA !== ''
    ? `${env.APPDATA}/ASP.NET/https`
    : `${env.HOME}/.aspnet/https`;

const certificateName = 'gumisho.client';
const certFilePath = path.join(baseFolder, `${certificateName}.pem`);
const keyFilePath = path.join(baseFolder, `${certificateName}.key`);

let httpsConfig = false;

if (!isProduction) {
  // Only try to generate or read certs locally
  if (!fs.existsSync(certFilePath) || !fs.existsSync(keyFilePath)) {
    if (
      child_process.spawnSync(
        'dotnet',
        [
          'dev-certs',
          'https',
          '--export-path',
          certFilePath,
          '--format',
          'Pem',
          '--no-password',
        ],
        { stdio: 'inherit' }
      ).status !== 0
    ) {
      console.warn('⚠️ Could not generate HTTPS certificate. Falling back to HTTP.');
    }
  }

  if (fs.existsSync(certFilePath) && fs.existsSync(keyFilePath)) {
    httpsConfig = {
      key: fs.readFileSync(keyFilePath),
      cert: fs.readFileSync(certFilePath),
    };
  }
}

const target = env.ASPNETCORE_HTTPS_PORT
  ? `https://localhost:${env.ASPNETCORE_HTTPS_PORT}`
  : env.ASPNETCORE_URLS
<<<<<<< HEAD
    ? env.ASPNETCORE_URLS.split(';')[0]
    : 'https://localhost:7048';
=======
  ? env.ASPNETCORE_URLS.split(';')[0]
  : 'https://localhost:7048';
>>>>>>> 2ab7d85b14734d04da1997c4dc290d79e46498d4

export default defineConfig({
  plugins: [plugin()],
  resolve: {
    alias: {
      '@': fileURLToPath(new URL('./src', import.meta.url)),
    },
  },
  server: {
    port: 61333,
    https: httpsConfig,
    proxy: {
      '^/api': {
        target,
        secure: false,
      },
    },
  },
});
