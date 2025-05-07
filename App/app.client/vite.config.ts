import { fileURLToPath, URL } from 'node:url';

import { defineConfig } from 'vitest/config';
import plugin from '@vitejs/plugin-react';
import fs from 'fs';
import path from 'path';
import child_process from 'child_process';
import { env } from 'process';

const baseFolder =
    env.APPDATA !== undefined && env.APPDATA !== ''
        ? `${env.APPDATA}/ASP.NET/https`
        : `${env.HOME}/.aspnet/https`;

const certificateName = "app.client";
const certFilePath = path.join(baseFolder, `${certificateName}.pem`);
const keyFilePath = path.join(baseFolder, `${certificateName}.key`);

if (!fs.existsSync(certFilePath) || !fs.existsSync(keyFilePath)) {
    if (0 !== child_process.spawnSync('dotnet', [
        'dev-certs',
        'https',
        '--export-path',
        certFilePath,
        '--format',
        'Pem',
        '--no-password',
    ], { stdio: 'inherit', }).status) {
        throw new Error("Could not create certificate.");
    }
}

const target = env.ASPNETCORE_HTTPS_PORT ? `https://localhost:${env.ASPNETCORE_HTTPS_PORT}` :
    env.ASPNETCORE_URLS ? env.ASPNETCORE_URLS.split(';')[0] : 'https://localhost:7136';

const hubTarget = 'https://localhost:7136';
// https://vitejs.dev/config/
export default defineConfig({
    plugins: [plugin()],
    test: {
        globals: true,
        environment: 'jsdom',
        setupFiles: './vitest.setup.ts',
    },
    resolve: {
        alias: {
            '@': fileURLToPath(new URL('./src', import.meta.url))
        }
    },
    server: {
        proxy: {
            '^/api/Attendence': {
                target,
                secure: false
            },
            '^/api/Plan': {
                target,
                secure: false
            },
            '^/api/Ranger': {
                target,
                secure: false
            },
            '^/api/Route': {
                target,
                secure: false
            },
            '^/api/User': {
                target,
                secure: false
            },
            '^/api/District': {
                target,
                secure: false
            },
            '^/api/Vehicle': {
                target,
                secure: false
            },
            '^/api/Lock': {
                target,
                secure: false
            },
            '^/districtHub': {
                target : target,
                secure: false,
                ws: true
            },
            '^/rangerScheduleHub': {
                target: target,
                secure: false,
                ws: true
            },   
        },
        port: 5173,
        https: {
            key: fs.readFileSync(keyFilePath),
            cert: fs.readFileSync(certFilePath),
        }
    }
})
