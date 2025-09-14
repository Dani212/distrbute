# AI Agent Prompt

Create a monorepo project structure with the following specifications:

## Root Structure
- `apps/`
  - `frontend/`
    - `brand/` → Next.js starter project
    - `distributor/` → Next.js starter project
    - `marketing/` → Next.js project for landing pages and SEO-related pages
  - `backend/`
    - `brandapi/` → .NET Web API starter repository (basic runnable implementation)
    - `distributorapi/` → .NET Web API starter repository (basic runnable implementation)

- `libs/`
  - `ui/` → Shared library for Next.js projects that contains:
    - ShadCN UI components
    - Frontend utilities (validation, hooks, constants, helpers)
  - `shared-models/` → Data models (DTOs, contracts, schemas) used across frontend & backend

## Requirements
1. Each Next.js project under `frontend` must be a basic runnable Next.js app with TypeScript.
2. Each backend project under `backend` must be a basic runnable .NET Web API implementation.
3. The `libs/ui` library must be installable as a dependency in all Next.js apps (e.g., `import { Button } from '@project/ui'`).
4. The `libs/shared-models` folder should contain shared data contracts (DTOs in C# and TypeScript) to ensure type safety and consistency across services.
5. Ensure folder and project naming follows the convention above.
6. Provide setup instructions (e.g., commands to run frontend projects, backend projects, and build steps for shared libraries).

## Deliverable
Generate:
- Folder structure with boilerplate code for each project
- Basic Next.js apps (`brand`, `distributor`, `marketing`)
- Basic .NET Web API projects (`brandapi`, `distributorapi`)
- `ui` library with reusable ShadCN components and frontend utilities
- `shared-models` library for cross-cutting contracts and DTOs
