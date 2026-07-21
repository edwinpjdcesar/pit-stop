import { useEffect, useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import Header from '../components/Header';
import { deletePart, getParts } from '../services/partService';
import type { Part } from '../types/part';
import styles from './PartsListPage.module.css';

export default function PartsListPage() {
  const navigate = useNavigate();
  const [parts, setParts] = useState<Part[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [deletingId, setDeletingId] = useState<string | null>(null);
  const [deleteError, setDeleteError] = useState<string | null>(null);

  useEffect(() => {
    getParts()
      .then(setParts)
      .catch((err: Error) => setError(err.message))
      .finally(() => setLoading(false));
  }, []);

  async function handleDelete(partId: string) {
    if (!window.confirm('Delete this part? This cannot be undone.')) return;
    setDeletingId(partId);
    setDeleteError(null);
    try {
      await deletePart(partId);
      setParts((prev) => prev.filter((p) => p.partId !== partId));
    } catch (err: unknown) {
      setDeleteError(err instanceof Error ? err.message : 'Failed to delete part.');
    } finally {
      setDeletingId(null);
    }
  }

  return (
    <div className={styles.page}>
      <Header />

      <main className={styles.main}>
        <div className={styles.toolbar}>
          <div>
            <Link to="/" className={styles.back}>← Vehicles</Link>
            <h1>Parts Library</h1>
          </div>
          <button
            type="button"
            className={styles.primaryButton}
            onClick={() => navigate('/parts/new')}
          >
            New Part
          </button>
        </div>

        {loading && <p className={styles.status}>Loading...</p>}
        {error && <p className={styles.error}>{error}</p>}
        {deleteError && <p className={styles.error}>{deleteError}</p>}

        {!loading && !error && parts.length === 0 && (
          <p className={styles.empty}>No parts found.</p>
        )}

        {!loading && !error && parts.length > 0 && (
          <table className={styles.table}>
            <thead>
              <tr>
                <th>Name</th>
                <th>Model Number</th>
                <th>Description</th>
                <th></th>
              </tr>
            </thead>
            <tbody>
              {parts.map((part) => (
                <tr key={part.partId}>
                  <td>{part.name}</td>
                  <td>{part.modelNumber ?? '—'}</td>
                  <td>{part.description}</td>
                  <td className={styles.actions}>
                    <Link to={`/parts/${part.partId}/edit`} className={styles.editLink}>
                      Edit
                    </Link>
                    <button
                      type="button"
                      className={styles.dangerButton}
                      onClick={() => handleDelete(part.partId)}
                      disabled={deletingId === part.partId}
                    >
                      {deletingId === part.partId ? 'Deleting...' : 'Delete'}
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
