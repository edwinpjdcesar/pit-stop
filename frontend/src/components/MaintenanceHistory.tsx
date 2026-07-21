import { Fragment, useEffect, useState } from 'react';
import MaintenanceFormModal from './MaintenanceFormModal';
import MaintenancePartsPanel from './MaintenancePartsPanel';
import { deleteMaintenanceRecord, getMaintenanceRecords } from '../services/maintenanceService';
import type { MaintenanceRecord } from '../types/maintenance';
import type { MaintenancePart } from '../types/maintenancePart';
import styles from './MaintenanceHistory.module.css';

type Props = {
  vehicleId: string;
};

export default function MaintenanceHistory({ vehicleId }: Props) {
  const [records, setRecords] = useState<MaintenanceRecord[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [showModal, setShowModal] = useState(false);
  const [editingRecord, setEditingRecord] = useState<MaintenanceRecord | null>(null);
  const [deletingId, setDeletingId] = useState<string | null>(null);
  const [deleteError, setDeleteError] = useState<string | null>(null);
  const [expandedIds, setExpandedIds] = useState<Set<string>>(new Set());

  useEffect(() => {
    getMaintenanceRecords(vehicleId)
      .then(setRecords)
      .catch((err: Error) => setError(err.message))
      .finally(() => setLoading(false));
  }, [vehicleId]);

  function handleOpenAdd() {
    setEditingRecord(null);
    setShowModal(true);
  }

  function handleOpenEdit(record: MaintenanceRecord) {
    setEditingRecord(record);
    setShowModal(true);
  }

  function handleCloseModal() {
    setEditingRecord(null);
    setShowModal(false);
  }

  function handleRecordSaved(record: MaintenanceRecord) {
    setRecords((prev) => {
      if (editingRecord) {
        return prev.map((r) => (r.maintenanceId === record.maintenanceId ? record : r));
      }
      return [record, ...prev];
    });
    handleCloseModal();
  }

  function toggleExpanded(maintenanceId: string) {
    setExpandedIds((prev) => {
      const next = new Set(prev);
      if (next.has(maintenanceId)) {
        next.delete(maintenanceId);
      } else {
        next.add(maintenanceId);
      }
      return next;
    });
  }

  function handlePartsChanged(maintenanceId: string, parts: MaintenancePart[]) {
    setRecords((prev) =>
      prev.map((r) => (r.maintenanceId === maintenanceId ? { ...r, maintenanceParts: parts } : r))
    );
  }

  async function handleDelete(maintenanceId: string) {
    if (!window.confirm('Delete this record? This cannot be undone.')) return;
    setDeletingId(maintenanceId);
    setDeleteError(null);
    try {
      await deleteMaintenanceRecord(vehicleId, maintenanceId);
      setRecords((prev) => prev.filter((r) => r.maintenanceId !== maintenanceId));
    } catch (err: unknown) {
      setDeleteError(err instanceof Error ? err.message : 'Failed to delete record.');
    } finally {
      setDeletingId(null);
    }
  }

  return (
    <section>
      {showModal && (
        <MaintenanceFormModal
          vehicleId={vehicleId}
          existing={editingRecord ?? undefined}
          onSaved={handleRecordSaved}
          onClose={handleCloseModal}
        />
      )}

      <div className={styles.header}>
        <h2 className={styles.title}>Maintenance History</h2>
        <button type="button" className={styles.addButton} onClick={handleOpenAdd}>
          Add Record
        </button>
      </div>

      {loading && <p className={styles.status}>Loading...</p>}
      {error && <p className={styles.error}>{error}</p>}
      {deleteError && <p className={styles.error}>{deleteError}</p>}

      {!loading && !error && records.length === 0 && (
        <p className={styles.empty}>No maintenance records yet.</p>
      )}

      {!loading && !error && records.length > 0 && (
        <table className={styles.table}>
          <thead>
            <tr>
              <th className={styles.expanderCol}></th>
              <th>Date</th>
              <th>Description</th>
              <th>Mileage</th>
              <th>Parts</th>
              <th></th>
            </tr>
          </thead>
          <tbody>
            {records.map((record) => {
              const isExpanded = expandedIds.has(record.maintenanceId);
              return (
                <Fragment key={record.maintenanceId}>
                  <tr>
                    <td>
                      <button
                        type="button"
                        className={styles.expander}
                        aria-label={isExpanded ? 'Collapse parts' : 'Expand parts'}
                        aria-expanded={isExpanded}
                        onClick={() => toggleExpanded(record.maintenanceId)}
                      >
                        {isExpanded ? '▾' : '▸'}
                      </button>
                    </td>
                    <td>{new Date(record.serviceDate).toLocaleDateString()}</td>
                    <td>{record.description}</td>
                    <td>{record.mileage.toLocaleString()}</td>
                    <td>{record.maintenanceParts.length}</td>
                    <td className={styles.tableActions}>
                      <button
                        type="button"
                        className={styles.editButton}
                        onClick={() => handleOpenEdit(record)}
                      >
                        Edit
                      </button>
                      <button
                        type="button"
                        className={styles.deleteButton}
                        onClick={() => handleDelete(record.maintenanceId)}
                        disabled={deletingId === record.maintenanceId}
                      >
                        {deletingId === record.maintenanceId ? 'Deleting...' : 'Delete'}
                      </button>
                    </td>
                  </tr>
                  {isExpanded && (
                    <tr>
                      <td colSpan={6} className={styles.expandedCell}>
                        <MaintenancePartsPanel
                          vehicleId={vehicleId}
                          maintenanceId={record.maintenanceId}
                          parts={record.maintenanceParts}
                          onPartsChanged={(parts) =>
                            handlePartsChanged(record.maintenanceId, parts)
                          }
                        />
                      </td>
                    </tr>
                  )}
                </Fragment>
              );
            })}
          </tbody>
        </table>
      )}
    </section>
  );
}
