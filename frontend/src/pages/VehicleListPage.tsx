import { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import type { Vehicle } from '../types/vehicle';
import { getVehicles } from '../services/vehicleService';
import styles from './VehicleListPage.module.css';

export default function VehicleListPage() {
  const [vehicles, setVehicles] = useState<Vehicle[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    getVehicles()
      .then(setVehicles)
      .catch((err: Error) => setError(err.message))
      .finally(() => setLoading(false));
  }, []);

  return (
    <div className={styles.page}>
      <header className={styles.header}>
        <span className={styles.logo}>PitStop</span>
        <nav>
          <Link to="/parts">Parts Library</Link>
        </nav>
      </header>

      <main className={styles.main}>
        <div className={styles.toolbar}>
          <h1>Vehicles</h1>
          <button type="button" className={styles.primaryButton}>
            New Vehicle
          </button>
        </div>

        {loading && <p className={styles.status}>Loading...</p>}

        {error && <p className={styles.error}>{error}</p>}

        {!loading && !error && vehicles.length === 0 && (
          <p className={styles.empty}>No vehicles yet. Add your first one.</p>
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
                    <button type="button" className={styles.dangerButton}>
                      Delete
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
