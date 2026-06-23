import { BrowserRouter, Routes, Route } from 'react-router-dom';
import VehicleListPage from './pages/VehicleListPage';
import VehicleFormPage from './pages/VehicleFormPage';
import VehicleDetailPage from './pages/VehicleDetailPage';
import PartsListPage from './pages/PartsListPage';
import PartFormPage from './pages/PartFormPage';

function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<VehicleListPage />} />
        <Route path="/vehicles/new" element={<VehicleFormPage />} />
        <Route path="/vehicles/:vehicleId" element={<VehicleDetailPage />} />
        <Route path="/vehicles/:vehicleId/edit" element={<VehicleFormPage />} />
        <Route path="/parts" element={<PartsListPage />} />
        <Route path="/parts/new" element={<PartFormPage />} />
        <Route path="/parts/:partId/edit" element={<PartFormPage />} />
      </Routes>
    </BrowserRouter>
  );
}

export default App;
