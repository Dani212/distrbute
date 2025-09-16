/** @type {import('next').NextConfig} */
const nextConfig = {
  transpilePackages: ['@distrbute/next-shared'],
  experimental: {
    turbo: {
      rules: {
        '*.svg': {
          loaders: ['@svgr/webpack'],
          as: '*.js',
        },
      },
    },
  },
}

module.exports = nextConfig
