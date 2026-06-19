# PitStop

A vehicle maintenance tracking app. Log service history, track parts, and keep a record of what's been done to your vehicles.

---

## Tech Stack

- **Frontend** — React 19, TypeScript, Vite
- **Backend** — .NET 10, ASP.NET Core Web API
- **Database** — SQL Server 2022
- **Reverse Proxy** — nginx

---

## Project Structure

```
pit-stop/
├── backend/        # .NET 10 ASP.NET Core Web API
├── frontend/       # React 19 + TypeScript + Vite
├── proxy/          # nginx reverse proxy config
└── docker-compose.yml
```

See [`backend/README.md`](backend/README.md) for API reference and backend-specific details.

---

## Prerequisites

- [Docker Desktop](https://www.docker.com/products/docker-desktop/)
- [.NET 10 SDK](https://dotnet.microsoft.com/download) *(optional — local development only)*
- [Node.js 22+](https://nodejs.org/) *(optional — local development only)*

---

## Running the App

1. Clone the repository:

   ```bash
   git clone https://github.com/edwinpjdcesar/pit-stop.git
   cd pit-stop
   ```

2. Create a `.env` file in the project root:

   ```env
   SA_PASSWORD=YourStr0ng!Password
   ```

   > The password must meet SQL Server complexity requirements: at least 8 characters with uppercase, lowercase, a digit, and a special character.

3. Build and start all services:

   ```bash
   docker compose up --build
   ```

4. Open the app at [http://localhost:6789](http://localhost:6789).

To stop the app, press `Ctrl+C` or run:

```bash
docker compose down
```

---

## Development Environment

The Docker setup includes hot reload — frontend changes reflect in the browser automatically without a rebuild.

### First-time setup

Follow steps 1–3 from [Running the App](#running-the-app) above.

### Day-to-day development

Start the stack without rebuilding:

```bash
docker compose up
```

Only run `--build` when you change `package.json` dependencies or a `Dockerfile`.

### Making changes

| Area | Location | Notes |
|------|----------|-------|
| Frontend | `frontend/src/` | Vite HMR pushes changes to the browser instantly |
| Backend API | `backend/src/` | Restart the `api` container to pick up changes: `docker compose restart api` |
| Database schema | `backend/src/Data/` | Migrations run automatically on API startup |
| nginx config | `proxy/nginx.conf` | Restart the `proxy` container: `docker compose restart proxy` |

### Running the application locally (without Docker)

Docker can still be used just for the database:

```bash
docker compose up db
```

**Backend**

1. Navigate to the backend folder and run the API:

   ```bash
   cd backend
   dotnet run --project src/Api
   ```

2. The API will be available at `https://localhost:7226`. The Scalar API reference is at `https://localhost:7226/scalar/v1`.

**Frontend**

1. Navigate to the frontend folder and install dependencies:

   ```bash
   cd frontend
   npm install
   ```

2. Set `VITE_API_URL` in `frontend/.env.development` to point at the local backend:

   ```env
   VITE_API_URL=https://localhost:7226
   ```

3. Start the dev server:

   ```bash
   npm run dev
   ```

4. The app will be available at `http://localhost:5173`.
