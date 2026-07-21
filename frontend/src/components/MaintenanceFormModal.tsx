import { useState } from 'react';
import type { MaintenanceRecord } from '../types/maintenance';
import { createMaintenanceRecord, updateMaintenanceRecord } from '../services/maintenanceService';
import styles from './MaintenanceFormModal.module.css';

type Props = {
  vehicleId: string;
  existing?: MaintenanceRecord;
  onSaved: (record: MaintenanceRecord) => void;
  onClose: () => void;
};

type FormErrors = {
  description?: string;
  mileage?: string;
  serviceDate?: string;
};

function toDateInputValue(isoString: string): string {
  return isoString.split('T')[0];
}

export default function MaintenanceFormModal({ vehicleId, existing, onSaved, onClose }: Props) {
  const isEditing = !!existing;

  const [description, setDescription] = useState(existing?.description ?? '');
  const [mileage, setMileage] = useState(existing != null ? String(existing.mileage) : '');
  const [serviceDate, setServiceDate] = useState(
    existing ? toDateInputValue(existing.serviceDate) : ''
  );
  const [errors, setErrors] = useState<FormErrors>({});
  const [submitError, setSubmitError] = useState<string | null>(null);
  const [submitting, setSubmitting] = useState(false);

  function validate(): FormErrors {
    const errs: FormErrors = {};

    if (!description.trim()) errs.description = 'Description is required.';

    if (!mileage) {
      errs.mileage = 'Mileage is required.';
    } else {
      const m = Number(mileage);
      if (!Number.isInteger(m) || isNaN(m) || m < 0)
        errs.mileage = 'Mileage must be a non-negative whole number.';
    }

    if (!serviceDate) errs.serviceDate = 'Service date is required.';

    return errs;
  }

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault();
    setSubmitError(null);

    const errs = validate();
    if (Object.keys(errs).length > 0) {
      setErrors(errs);
      return;
    }
    setErrors({});
    setSubmitting(true);

    const request = {
      description: description.trim(),
      mileage: Number(mileage),
      serviceDate: new Date(serviceDate).toISOString(),
    };

    try {
      const record = isEditing
        ? await updateMaintenanceRecord(vehicleId, existing.maintenanceId, request)
        : await createMaintenanceRecord(vehicleId, request);
      onSaved(record);
    } catch (err: unknown) {
      setSubmitError(err instanceof Error ? err.message : 'Something went wrong. Please try again.');
    } finally {
      setSubmitting(false);
    }
  }

  return (
    <div className={styles.overlay} onClick={onClose}>
      <div className={styles.modal} onClick={(e) => e.stopPropagation()}>
        <div className={styles.modalHeader}>
          <h2>{isEditing ? 'Edit Maintenance Record' : 'Add Maintenance Record'}</h2>
          <button type="button" className={styles.closeButton} onClick={onClose} aria-label="Close">
            ✕
          </button>
        </div>

        <form onSubmit={handleSubmit} noValidate>
          <div className={styles.field}>
            <label htmlFor="description">
              Description <span className={styles.required}>*</span>
            </label>
            <textarea
              id="description"
              value={description}
              onChange={(e) => setDescription(e.target.value)}
              placeholder="e.g. Oil change and filter replacement"
              rows={3}
            />
            {errors.description && <span className={styles.fieldError}>{errors.description}</span>}
          </div>

          <div className={styles.row}>
            <div className={styles.field}>
              <label htmlFor="serviceDate">
                Service Date <span className={styles.required}>*</span>
              </label>
              <input
                id="serviceDate"
                type="date"
                value={serviceDate}
                onChange={(e) => setServiceDate(e.target.value)}
              />
              {errors.serviceDate && <span className={styles.fieldError}>{errors.serviceDate}</span>}
            </div>

            <div className={styles.field}>
              <label htmlFor="mileage">
                Mileage <span className={styles.required}>*</span>
              </label>
              <input
                id="mileage"
                type="number"
                value={mileage}
                onChange={(e) => setMileage(e.target.value)}
                placeholder="0"
                min="0"
              />
              {errors.mileage && <span className={styles.fieldError}>{errors.mileage}</span>}
            </div>
          </div>

          {submitError && <p className={styles.submitError}>{submitError}</p>}

          <div className={styles.actions}>
            <button type="button" className={styles.cancelButton} onClick={onClose}>
              Cancel
            </button>
            <button type="submit" className={styles.submitButton} disabled={submitting}>
              {submitting ? 'Saving...' : isEditing ? 'Save Changes' : 'Add Record'}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}
