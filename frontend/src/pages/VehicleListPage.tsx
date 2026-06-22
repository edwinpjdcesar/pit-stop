import { useEffect, useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import Header from '../components/Header';
import type { Vehicle } from '../types/vehicle';
import { deleteVehicle, getVehicles } from '../services/vehicleService';
import styles from './VehicleListPage.module.css';

export default function VehicleListPage() {
  const navigate = useNavigate();
  const [vehicles, setVehicles] = useState<Vehicle[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [deletingId, setDeletingId] = useState<string | null>(null);
  const [deleteError, setDeleteError] = useState<string | null>(null);

  useEffect(() => {
    getVehicles()
      .then(setVehicles)
      .catch((err: Error) => setError(err.message))
      .finally(() => setLoading(false));
  }, []);

  async function handleDelete(vehicleId: string) {
    if (!window.confirm('Delete this vehicle? This cannot be undone.')) return;
    setDeletingId(vehicleId);
    setDeleteError(null);
    try {
      await deleteVehicle(vehicleId);
      setVehicles((prev) => prev.filter((v) => v.vehicleId !== vehicleId));
    } catch (err: unknown) {
      setDeleteError(err instanceof Error ? err.message : 'Failed to delete vehicle.');
    } finally {
      setDeletingId(null);
    }
  }

  return (
    <div className={styles.page}>
      <Header />

      <main className={styles.main}>
        <div className={styles.toolbar}>
          <h1>Vehicles</h1>
          <button
            type="button"
            className={styles.primaryButton}
            onClick={() => navigate('/vehicles/new')}
          >
            New Vehicle
          </button>
        </div>

        {loading && <p className={styles.status}>Loading...</p>}
        {error && <p className={styles.error}>{error}</p>}
        {deleteError && <p className={styles.error}>{deleteError}</p>}

        {!loading && !error && vehicles.length === 0 && (
          <p className={styles.empty}>No vehicles found.</p>
        )}

        {!loading && !error && vehicles.length > 0 && (
          <table className={styles.table}>
            <thead>
              <tr>
                <th>Year</th>
                <th>Make</th>
                <th>Model</th>
                <th>License Plate</th>
                <th>VIN</th>
                <th>Mileage</th>
                <th></th>
              </tr>
            </thead>
            <tbody>
              {vehicles.map((vehicle) => (
                <tr key={vehicle.vehicleId}>
                  <td>{vehicle.year}</td>
                  <td>{vehicle.make.name}</td>
                  <td>{vehicle.model.name}</td>
                  <td>{vehicle.licensePlate ?? '—'}</td>
                  <td>{vehicle.vin ?? '—'}</td>
                  <td>{vehicle.mileage != null ? vehicle.mileage.toLocaleString() : '—'}</td>
                  <td className={styles.actions}>
                    <Link to={`/vehicles/${vehicle.vehicleId}`}>View</Link>
                    <button
                      type="button"
                      className={styles.dangerButton}
                      onClick={() => handleDelete(vehicle.vehicleId)}
                      disabled={deletingId === vehicle.vehicleId}
                    >
                      {deletingId === vehicle.vehicleId ? 'Deleting...' : 'Delete'}
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </main>
    </div>
  );
}
