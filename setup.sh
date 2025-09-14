#!/bin/bash

echo "🚀 Setting up Distrbute Monorepo..."

# Check if Node.js is installed
if ! command -v node &> /dev/null; then
    echo "❌ Node.js is not installed. Please install Node.js 18.0.0 or higher."
    exit 1
fi

# Check Node.js version
NODE_VERSION=$(node -v | cut -d'v' -f2 | cut -d'.' -f1)
if [ "$NODE_VERSION" -lt 18 ]; then
    echo "❌ Node.js version 18.0.0 or higher is required. Current version: $(node -v)"
    exit 1
fi

# Check if .NET is installed (optional for frontend-only setup)
if ! command -v dotnet &> /dev/null; then
    echo "⚠️  .NET is not installed. Backend APIs will not be available."
    echo "   To install .NET 8.0 SDK, visit: https://dotnet.microsoft.com/download"
    echo ""
else
    echo "✅ .NET SDK found: $(dotnet --version)"
fi

echo "✅ Prerequisites check passed"

# Install root dependencies
echo "📦 Installing root dependencies..."
npm install

# Install dependencies for each app
echo "📦 Installing frontend dependencies..."
cd apps/frontend/brand && npm install && cd ../../..
cd apps/frontend/distributor && npm install && cd ../../..
cd apps/frontend/marketing && npm install && cd ../../..

# Install dependencies for shared libraries
echo "📦 Installing shared library dependencies..."
cd libs/shared-models && npm install && cd ../..
cd libs/ui && npm install && cd ../..

# Build shared libraries
echo "🔨 Building shared libraries..."
cd libs/shared-models && npm run build && cd ../..
cd libs/ui && npm run build && cd ../..

echo "✅ Setup complete!"
echo ""
echo "📋 Version Information:"
echo "  - Next.js: 15.5.3 (with Turbopack)"
echo "  - React: 19.0.0"
echo "  - TypeScript: 5.7.2"
echo "  - Tailwind CSS: 4.1.11"
echo ""
echo "🎉 You can now start the applications:"
echo ""
echo "Frontend Applications:"
echo "  npm run dev:marketing    # http://localhost:3000"
echo "  npm run dev:brand        # http://localhost:3001"
echo "  npm run dev:distributor  # http://localhost:3002"
echo ""
echo "Backend APIs:"
echo "  cd apps/backend/brandapi && dotnet run        # https://localhost:5001"
echo "  cd apps/backend/distributorapi && dotnet run  # https://localhost:5002"
echo ""
echo "📚 For more information, see the README.md file"
