import { useEffect, useState } from 'react';
import { Link, useNavigate, useParams } from 'react-router-dom';
import Header from '../components/Header';
import { deleteVehicle, getVehicle } from '../services/vehicleService';
import type { Vehicle } from '../types/vehicle';
import styles from './VehicleDetailPage.module.css';

export default function VehicleDetailPage() {
  const { vehicleId } = useParams<{ vehicleId: string }>();
  const navigate = useNavigate();

  const [vehicle, setVehicle] = useState<Vehicle | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [deleting, setDeleting] = useState(false);
  const [deleteError, setDeleteError] = useState<string | null>(null);

  useEffect(() => {
    if (!vehicleId) return;
    getVehicle(vehicleId)
      .then(setVehicle)
      .catch((err: Error) => setError(err.message))
      .finally(() => setLoading(false));
  }, [vehicleId]);

  async function handleDelete() {
    if (!vehicleId || !window.confirm('Delete this vehicle? This cannot be undone.')) return;
    setDeleting(true);
    setDeleteError(null);
    try {
      await deleteVehicle(vehicleId);
      navigate('/');
    } catch (err: unknown) {
      setDeleteError(err instanceof Error ? err.message : 'Failed to delete vehicle.');
      setDeleting(false);
    }
  }

  if (loading) {
    return (
      <div className={styles.page}>
        <Header />
        <main className={styles.main}>
          <p className={styles.status}>Loading...</p>
        </main>
      </div>
    );
  }

  if (error || !vehicle) {
    return (
      <div className={styles.page}>
        <Header />
        <main className={styles.main}>
          <p className={styles.error}>{error ?? 'Vehicle not found.'}</p>
        </main>
      </div>
    );
  }

  const title = `${vehicle.year} ${vehicle.make.name} ${vehicle.model.name}`;

  return (
    <div className={styles.page}>
      <Header />

      <main className={styles.main}>
        <div className={styles.toolbar}>
          <div>
            <Link to="/" className={styles.back}>← Vehicles</Link>
            <h1 className={styles.title}>{title}</h1>
          </div>
          <div className={styles.toolbarActions}>
            <Link to={`/vehicles/${vehicleId}/edit`} className={styles.editButton}>
              Edit
            </Link>
            <button
              type="button"
              className={styles.deleteButton}
              onClick={handleDelete}
              disabled={deleting}
            >
              {deleting ? 'Deleting...' : 'Delete'}
            </button>
          </div>
        </div>

        {deleteError && <p className={styles.deleteError}>{deleteError}</p>}

        <div className={styles.card}>
          <div className={styles.section}>
            <h2 className={styles.sectionTitle}>Vehicle Info</h2>
            <dl className={styles.fields}>
              <div className={styles.field}>
                <dt>Year</dt>
                <dd>{vehicle.year}</dd>
              </div>
              <div className={styles.field}>
                <dt>Make</dt>
                <dd>{vehicle.make.name}</dd>
              </div>
              <div className={styles.field}>
                <dt>Model</dt>
                <dd>{vehicle.model.name}</dd>
              </div>
              <div className={styles.field}>
                <dt>VIN</dt>
                <dd>{vehicle.vin ?? '—'}</dd>
              </div>
              <div className={styles.field}>
                <dt>License Plate</dt>
                <dd>{vehicle.licensePlate ?? '—'}</dd>
              </div>
              <div className={styles.field}>
                <dt>Current Mileage</dt>
                <dd>{vehicle.mileage != null ? vehicle.mileage.toLocaleString() : '—'}</dd>
              </div>
            </dl>
          </div>

          <div className={styles.divider} />

          <div className={styles.section}>
            <h2 className={styles.sectionTitle}>Purchase Info</h2>
            <dl className={styles.fields}>
              <div className={styles.field}>
                <dt>Purchase Date</dt>
                <dd>{vehicle.purchaseDate ?? '—'}</dd>
              </div>
              <div className={styles.field}>
                <dt>Purchase Price</dt>
                <dd>
                  {vehicle.purchasePrice != null
                    ? `$${vehicle.purchasePrice.toLocaleString()}`
                    : '—'}
                </dd>
              </div>
              <div className={styles.field}>
                <dt>Mileage at Purchase</dt>
                <dd>
                  {vehicle.mileageAtPurchase != null
                    ? vehicle.mileageAtPurchase.toLocaleString()
                    : '—'}
                </dd>
              </div>
            </dl>
          </div>
        </div>
      </main>
    </div>
  );
}
