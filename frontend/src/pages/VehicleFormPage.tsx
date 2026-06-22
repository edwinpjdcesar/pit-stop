import { useEffect, useRef, useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import Header from '../components/Header';
import { getMakes, getModels } from '../services/makeService';
import { createVehicle, getVehicle, updateVehicle } from '../services/vehicleService';
import type { Make } from '../types/make';
import type { Model } from '../types/model';
import styles from './VehicleFormPage.module.css';

type FormErrors = {
  makeId?: string;
  modelId?: string;
  year?: string;
  purchasePrice?: string;
  mileageAtPurchase?: string;
  mileage?: string;
};

export default function VehicleFormPage() {
  const { vehicleId } = useParams<{ vehicleId: string }>();
  const navigate = useNavigate();
  const isEditing = !!vehicleId;

  const [makeId, setMakeId] = useState('');
  const [modelId, setModelId] = useState('');
  const [year, setYear] = useState('');
  const [vin, setVin] = useState('');
  const [licensePlate, setLicensePlate] = useState('');
  const [purchaseDate, setPurchaseDate] = useState('');
  const [purchasePrice, setPurchasePrice] = useState('');
  const [mileageAtPurchase, setMileageAtPurchase] = useState('');
  const [mileage, setMileage] = useState('');

  const [makes, setMakes] = useState<Make[]>([]);
  const [models, setModels] = useState<Model[]>([]);
  const [loadingMakes, setLoadingMakes] = useState(true);
  const [loadingModels, setLoadingModels] = useState(false);
  const [loadingVehicle, setLoadingVehicle] = useState(isEditing);
  const [errors, setErrors] = useState<FormErrors>({});
  const [submitError, setSubmitError] = useState<string | null>(null);
  const [submitting, setSubmitting] = useState(false);

  // Holds the model ID to restore after models load in edit mode.
  const pendingModelIdRef = useRef('');

  useEffect(() => {
    getMakes()
      .then(setMakes)
      .catch(() => {})
      .finally(() => setLoadingMakes(false));
  }, []);

  useEffect(() => {
    if (!makeId) {
      setModels([]);
      return;
    }
    setLoadingModels(true);
    getModels(Number(makeId))
      .then((data) => {
        setModels(data);
        if (pendingModelIdRef.current) {
          setModelId(pendingModelIdRef.current);
          pendingModelIdRef.current = '';
        }
      })
      .catch(() => setModels([]))
      .finally(() => setLoadingModels(false));
  }, [makeId]);

  useEffect(() => {
    if (!vehicleId) return;
    getVehicle(vehicleId)
      .then((vehicle) => {
        setMakeId(String(vehicle.make.makeId));
        pendingModelIdRef.current = String(vehicle.model.modelId);
        setYear(String(vehicle.year));
        setVin(vehicle.vin ?? '');
        setLicensePlate(vehicle.licensePlate ?? '');
        setPurchaseDate(vehicle.purchaseDate ?? '');
        setPurchasePrice(vehicle.purchasePrice != null ? String(vehicle.purchasePrice) : '');
        setMileageAtPurchase(vehicle.mileageAtPurchase != null ? String(vehicle.mileageAtPurchase) : '');
        setMileage(vehicle.mileage != null ? String(vehicle.mileage) : '');
      })
      .catch((err: Error) => setSubmitError(err.message))
      .finally(() => setLoadingVehicle(false));
  }, [vehicleId]);

  function validate(): FormErrors {
    const errs: FormErrors = {};
    const currentYear = new Date().getFullYear();

    if (!makeId) errs.makeId = 'Make is required.';
    if (!modelId) errs.modelId = 'Model is required.';

    if (!year) {
      errs.year = 'Year is required.';
    } else {
      const y = Number(year);
      if (!Number.isInteger(y) || isNaN(y)) {
        errs.year = 'Year must be a whole number.';
      } else if (y < 1886 || y > currentYear + 1) {
        errs.year = `Year must be between 1886 and ${currentYear + 1}.`;
      }
    }

    if (purchasePrice !== '') {
      const p = Number(purchasePrice);
      if (isNaN(p) || p < 0) errs.purchasePrice = 'Purchase price must be a non-negative number.';
    }

    if (mileageAtPurchase !== '') {
      const m = Number(mileageAtPurchase);
      if (!Number.isInteger(m) || isNaN(m) || m < 0)
        errs.mileageAtPurchase = 'Mileage at purchase must be a non-negative whole number.';
    }

    if (mileage !== '') {
      const m = Number(mileage);
      if (!Number.isInteger(m) || isNaN(m) || m < 0)
        errs.mileage = 'Current mileage must be a non-negative whole number.';
    }

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

    try {
      const request = {
        makeId: Number(makeId),
        modelId: Number(modelId),
        year: Number(year),
        vin: vin.trim() || null,
        licensePlate: licensePlate.trim() || null,
        purchaseDate: purchaseDate || null,
        purchasePrice: purchasePrice !== '' ? Number(purchasePrice) : null,
        mileageAtPurchase: mileageAtPurchase !== '' ? Number(mileageAtPurchase) : null,
        mileage: mileage !== '' ? Number(mileage) : null,
      };

      const vehicle = isEditing
        ? await updateVehicle(vehicleId!, request)
        : await createVehicle(request);

      navigate(`/vehicles/${vehicle.vehicleId}`);
    } catch (err: unknown) {
      setSubmitError(err instanceof Error ? err.message : 'Something went wrong. Please try again.');
    } finally {
      setSubmitting(false);
    }
  }

  if (loadingVehicle) {
    return (
      <div className={styles.page}>
        <Header />
        <main className={styles.main}>
          <p className={styles.status}>Loading...</p>
        </main>
      </div>
    );
  }

  return (
    <div className={styles.page}>
      <Header />

      <main className={styles.main}>
        <div className={styles.toolbar}>
          <h1>{isEditing ? 'Edit Vehicle' : 'New Vehicle'}</h1>
        </div>

        <form className={styles.form} onSubmit={handleSubmit} noValidate>
          <div className={styles.row}>
            <div className={styles.field}>
              <label htmlFor="makeId">
                Make <span className={styles.required}>*</span>
              </label>
              <select
                id="makeId"
                value={makeId}
                onChange={(e) => {
                  setMakeId(e.target.value);
                  setModelId('');
                  pendingModelIdRef.current = '';
                }}
                disabled={loadingMakes}
              >
                <option value="">{loadingMakes ? 'Loading...' : 'Select a make'}</option>
                {makes.map((m) => (
                  <option key={m.makeId} value={m.makeId}>
                    {m.name}
                  </option>
                ))}
              </select>
              {errors.makeId && <span className={styles.fieldError}>{errors.makeId}</span>}
            </div>

            <div className={styles.field}>
              <label htmlFor="modelId">
                Model <span className={styles.required}>*</span>
              </label>
              <select
                id="modelId"
                value={modelId}
                onChange={(e) => setModelId(e.target.value)}
                disabled={!makeId || loadingModels}
              >
                <option value="">
                  {!makeId ? 'Select a make first' : loadingModels ? 'Loading...' : 'Select a model'}
                </option>
                {models.map((m) => (
                  <option key={m.modelId} value={m.modelId}>
                    {m.name}
                  </option>
                ))}
              </select>
              {errors.modelId && <span className={styles.fieldError}>{errors.modelId}</span>}
            </div>
          </div>

          <div className={styles.row}>
            <div className={styles.field}>
              <label htmlFor="year">
                Year <span className={styles.required}>*</span>
              </label>
              <input
                id="year"
                type="number"
                value={year}
                onChange={(e) => setYear(e.target.value)}
                placeholder="e.g. 2020"
                min="1886"
                max={new Date().getFullYear() + 1}
              />
              {errors.year && <span className={styles.fieldError}>{errors.year}</span>}
            </div>

            <div className={styles.field}>
              <label htmlFor="licensePlate">License Plate</label>
              <input
                id="licensePlate"
                type="text"
                value={licensePlate}
                onChange={(e) => setLicensePlate(e.target.value)}
                placeholder="e.g. ABC-1234"
              />
            </div>
          </div>

          <div className={styles.row}>
            <div className={styles.field}>
              <label htmlFor="vin">VIN</label>
              <input
                id="vin"
                type="text"
                value={vin}
                onChange={(e) => setVin(e.target.value)}
                placeholder="17-character VIN"
              />
            </div>

            <div className={styles.field}>
              <label htmlFor="purchaseDate">Purchase Date</label>
              <input
                id="purchaseDate"
                type="date"
                value={purchaseDate}
                onChange={(e) => setPurchaseDate(e.target.value)}
              />
            </div>
          </div>

          <div className={styles.row}>
            <div className={styles.field}>
              <label htmlFor="purchasePrice">Purchase Price</label>
              <input
                id="purchasePrice"
                type="number"
                value={purchasePrice}
                onChange={(e) => setPurchasePrice(e.target.value)}
                placeholder="0.00"
                min="0"
                step="0.01"
              />
              {errors.purchasePrice && <span className={styles.fieldError}>{errors.purchasePrice}</span>}
            </div>

            <div className={styles.field}>
              <label htmlFor="mileageAtPurchase">Mileage at Purchase</label>
              <input
                id="mileageAtPurchase"
                type="number"
                value={mileageAtPurchase}
                onChange={(e) => setMileageAtPurchase(e.target.value)}
                placeholder="0"
                min="0"
              />
              {errors.mileageAtPurchase && (
                <span className={styles.fieldError}>{errors.mileageAtPurchase}</span>
              )}
            </div>
          </div>

          <div className={styles.row}>
            <div className={styles.field}>
              <label htmlFor="mileage">Current Mileage</label>
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
            <button type="button" className={styles.cancelButton} onClick={() => navigate(-1)}>
              Cancel
            </button>
            <button type="submit" className={styles.submitButton} disabled={submitting}>
              {submitting ? 'Saving...' : isEditing ? 'Save Changes' : 'Create Vehicle'}
            </button>
          </div>
        </form>
      </main>
    </div>
  );
}
