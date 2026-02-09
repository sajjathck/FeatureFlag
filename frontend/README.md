Feature Flags Frontend (React + TypeScript)

Overview
--------
Minimal React frontend to manage feature flags and evaluate whether a flag is enabled for a given user.

Pages
-----
- Flags List: view flags, toggle on/off, inline edit rollout %, edit details.
- Create/Edit Flag: set name, rollout %, enabled, and target user IDs (CSV).
- Evaluate: choose a flag and enter a `userId` to get evaluation JSON and reason.

API
---
The frontend expects the backend API at `https://localhost:5001/api`.
Adjust `src/services/api.ts` `baseURL` if your backend runs on a different port.

Run locally
-----------
From `frontend` folder:

npm install
npm run dev

Then open the Vite URL (default http://localhost:5173).
