/** @type {import('next').NextConfig} */
const nextConfig = {
  transpilePackages: ['@distrbute/ui', '@distrbute/shared-models'],
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
