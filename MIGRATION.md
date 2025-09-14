# Migration Guide: Next.js 15.5.3 & Tailwind CSS 4.1.11

This document outlines the key changes and migration steps for updating to the latest versions of Next.js and Tailwind CSS.

## 🚀 What's New

### Next.js 15.5.3

- **Turbopack**: New Rust-based bundler for faster builds
- **React 19 Support**: Latest React features and improvements
- **Improved Performance**: Better build times and runtime performance
- **Enhanced Developer Experience**: Better error messages and debugging

### Tailwind CSS 4.1.11

- **High-Performance Engine**: Faster CSS generation
- **Automatic Content Detection**: Smarter purging of unused styles
- **Improved Configuration**: Better configuration experience
- **Enhanced Developer Tools**: Better IntelliSense and debugging

## 📦 Updated Dependencies

### Frontend Applications

- **Next.js**: `14.0.0` → `15.5.3`
- **React**: `^18.2.0` → `^19.0.0`
- **React DOM**: `^18.2.0` → `^19.0.0`
- **TypeScript**: `^5.2.2` → `^5.7.2`
- **Tailwind CSS**: `^3.3.0` → `^4.1.11`
- **ESLint**: `^8.0.0` → `^9.17.0`

### Shared Libraries

- **@distrbute/ui**: Updated to use React 19 and Tailwind CSS 4
- **@distrbute/shared-models**: Updated TypeScript target to ES2022

## 🔧 Configuration Changes

### Next.js Configuration

- Removed deprecated `appDir: true` experimental flag
- Added Turbopack configuration for better performance
- Updated TypeScript target to ES2022

### Tailwind CSS Configuration

- Updated keyframes syntax for better compatibility
- Maintained existing design system and color tokens
- Enhanced content detection for better purging

### TypeScript Configuration

- Updated target from ES5 to ES2022
- Updated lib array to include ES2022 features
- Maintained strict type checking

## 🛠️ Migration Steps

### 1. Update Dependencies

```bash
# Install updated dependencies
npm install

# Build shared libraries
npm run build:shared-models
npm run build:ui
```

### 2. Test Applications

```bash
# Test all frontend applications
npm run dev:marketing    # http://localhost:3000
npm run dev:brand        # http://localhost:3001
npm run dev:distributor  # http://localhost:3002
```

### 3. Verify Build Process

```bash
# Build all applications
npm run build

# Check for any build errors
npm run lint
```

## ⚠️ Breaking Changes

### React 19

- **New JSX Transform**: Automatic JSX runtime (no breaking changes for most apps)
- **Concurrent Features**: Better performance with automatic batching
- **Strict Mode**: Enhanced development warnings

### Tailwind CSS 4

- **Configuration**: Some configuration options may have changed
- **Purging**: Improved content detection may affect CSS output
- **Performance**: Faster builds but may require cache clearing

### TypeScript 5.7.2

- **Stricter Type Checking**: Enhanced type safety
- **ES2022 Features**: Access to latest JavaScript features
- **Better Error Messages**: Improved developer experience

## 🐛 Troubleshooting

### Common Issues

1. **Build Errors**

   ```bash
   # Clear Next.js cache
   rm -rf .next
   npm run build
   ```

2. **TypeScript Errors**

   ```bash
   # Update type definitions
   npm install @types/react@latest @types/react-dom@latest
   ```

3. **Tailwind CSS Issues**
   ```bash
   # Clear Tailwind cache
   rm -rf node_modules/.cache
   npm run dev
   ```

### Performance Issues

1. **Slow Builds**

   - Ensure Turbopack is enabled in development
   - Check for large bundle sizes
   - Optimize imports and dependencies

2. **Runtime Issues**
   - Check React 19 compatibility
   - Verify component lifecycle methods
   - Test concurrent features

## 📚 Resources

- [Next.js 15 Migration Guide](https://nextjs.org/docs/app/building-your-application/upgrading/version-15)
- [React 19 Release Notes](https://react.dev/blog/2024/12/05/react-19)
- [Tailwind CSS v4 Documentation](https://tailwindcss.com/docs)
- [TypeScript 5.7 Release Notes](https://devblogs.microsoft.com/typescript/announcing-typescript-5-7/)

## ✅ Verification Checklist

- [ ] All applications start without errors
- [ ] Build process completes successfully
- [ ] UI components render correctly
- [ ] TypeScript compilation passes
- [ ] Tailwind CSS styles apply properly
- [ ] No console errors or warnings
- [ ] Performance is maintained or improved

## 🆘 Support

If you encounter issues during migration:

1. Check the troubleshooting section above
2. Review the official documentation
3. Create an issue in the repository
4. Contact the development team

---

**Note**: This migration maintains backward compatibility for most features while providing significant performance improvements and new capabilities.
