import { useEffect, useState } from 'react';
import {
  addMaintenancePart,
  removeMaintenancePart,
  updateMaintenancePart,
} from '../services/maintenanceService';
import { getParts } from '../services/partService';
import type { MaintenancePart } from '../types/maintenancePart';
import type { Part } from '../types/part';
import styles from './MaintenancePartsPanel.module.css';

type Props = {
  vehicleId: string;
  maintenanceId: string;
  parts: MaintenancePart[];
  onPartsChanged: (parts: MaintenancePart[]) => void;
};

function formatCurrency(value: number): string {
  return value.toLocaleString(undefined, { style: 'currency', currency: 'USD' });
}

export default function MaintenancePartsPanel({
  vehicleId,
  maintenanceId,
  parts,
  onPartsChanged,
}: Props) {
  const [libraryParts, setLibraryParts] = useState<Part[]>([]);
  const [libraryError, setLibraryError] = useState<string | null>(null);
  const [actionError, setActionError] = useState<string | null>(null);

  // Add-part form state.
  const [selectedPartId, setSelectedPartId] = useState('');
  const [quantity, setQuantity] = useState('1');
  const [unitPrice, setUnitPrice] = useState('');
  const [adding, setAdding] = useState(false);

  // Inline edit state (one part at a time).
  const [editingPartId, setEditingPartId] = useState<string | null>(null);
  const [editQuantity, setEditQuantity] = useState('');
  const [editUnitPrice, setEditUnitPrice] = useState('');
  const [savingEdit, setSavingEdit] = useState(false);

  const [removingPartId, setRemovingPartId] = useState<string | null>(null);

  useEffect(() => {
    getParts()
      .then(setLibraryParts)
      .catch((err: Error) => setLibraryError(err.message));
  }, []);

  // Only offer library parts that are not already linked to this record.
  const linkedPartIds = new Set(parts.map((p) => p.partId));
  const availableParts = libraryParts.filter((p) => !linkedPartIds.has(p.partId));

  function validateQuantityAndPrice(qtyValue: string, priceValue: string): string | null {
    const qty = Number(qtyValue);
    if (!Number.isInteger(qty) || qty < 1) return 'Quantity must be a whole number of at least 1.';
    const price = Number(priceValue);
    if (isNaN(price) || price < 0) return 'Unit price must be zero or greater.';
    return null;
  }

  async function handleAdd(e: React.FormEvent) {
    e.preventDefault();
    setActionError(null);

    if (!selectedPartId) {
      setActionError('Select a part to add.');
      return;
    }
    const validationError = validateQuantityAndPrice(quantity, unitPrice);
    if (validationError) {
      setActionError(validationError);
      return;
    }

    setAdding(true);
    try {
      const added = await addMaintenancePart(vehicleId, maintenanceId, selectedPartId, {
        quantity: Number(quantity),
        unitPrice: Number(unitPrice),
      });
      onPartsChanged([...parts, added]);
      setSelectedPartId('');
      setQuantity('1');
      setUnitPrice('');
    } catch (err: unknown) {
      setActionError(err instanceof Error ? err.message : 'Failed to add part.');
    } finally {
      setAdding(false);
    }
  }

  function startEdit(part: MaintenancePart) {
    setActionError(null);
    setEditingPartId(part.partId);
    setEditQuantity(String(part.quantity));
    setEditUnitPrice(String(part.unitPrice));
  }

  function cancelEdit() {
    setEditingPartId(null);
  }

  async function handleSaveEdit(partId: string) {
    setActionError(null);
    const validationError = validateQuantityAndPrice(editQuantity, editUnitPrice);
    if (validationError) {
      setActionError(validationError);
      return;
    }

    setSavingEdit(true);
    try {
      const updated = await updateMaintenancePart(vehicleId, maintenanceId, partId, {
        quantity: Number(editQuantity),
        unitPrice: Number(editUnitPrice),
      });
      onPartsChanged(parts.map((p) => (p.partId === partId ? updated : p)));
      setEditingPartId(null);
    } catch (err: unknown) {
      setActionError(err instanceof Error ? err.message : 'Failed to update part.');
    } finally {
      setSavingEdit(false);
    }
  }

  async function handleRemove(partId: string) {
    setActionError(null);
    setRemovingPartId(partId);
    try {
      await removeMaintenancePart(vehicleId, maintenanceId, partId);
      onPartsChanged(parts.filter((p) => p.partId !== partId));
    } catch (err: unknown) {
      setActionError(err instanceof Error ? err.message : 'Failed to remove part.');
    } finally {
      setRemovingPartId(null);
    }
  }

  const total = parts.reduce((sum, p) => sum + p.quantity * p.unitPrice, 0);

  return (
    <div className={styles.panel}>
      <h3 className={styles.heading}>Parts</h3>

      {actionError && <p className={styles.error}>{actionError}</p>}
      {libraryError && <p className={styles.error}>{libraryError}</p>}

      {parts.length === 0 ? (
        <p className={styles.empty}>No parts linked to this record.</p>
      ) : (
        <table className={styles.partsTable}>
          <thead>
            <tr>
              <th>Part</th>
              <th>Qty</th>
              <th>Unit Price</th>
              <th>Line Total</th>
              <th></th>
            </tr>
          </thead>
          <tbody>
            {parts.map((part) => {
              const isEditing = editingPartId === part.partId;

              if (isEditing) {
                return (
                  <tr key={part.partId}>
                    <td>{part.partName}</td>
                    <td>
                      <input
                        type="number"
                        className={styles.inlineInput}
                        value={editQuantity}
                        min="1"
                        aria-label="Quantity"
                        onChange={(e) => setEditQuantity(e.target.value)}
                      />
                    </td>
                    <td>
                      <input
                        type="number"
                        className={styles.inlineInput}
                        value={editUnitPrice}
                        min="0"
                        step="0.01"
                        aria-label="Unit price"
                        onChange={(e) => setEditUnitPrice(e.target.value)}
                      />
                    </td>
                    <td>—</td>
                    <td className={styles.rowActions}>
                      <button
                        type="button"
                        className={styles.smallButton}
                        disabled={savingEdit}
                        onClick={() => handleSaveEdit(part.partId)}
                      >
                        {savingEdit ? 'Saving...' : 'Save'}
                      </button>
                      <button type="button" className={styles.smallButton} onClick={cancelEdit}>
                        Cancel
                      </button>
                    </td>
                  </tr>
                );
              }

              return (
                <tr key={part.partId}>
                  <td>{part.partName}</td>
                  <td>{part.quantity}</td>
                  <td>{formatCurrency(part.unitPrice)}</td>
                  <td>{formatCurrency(part.quantity * part.unitPrice)}</td>
                  <td className={styles.rowActions}>
                    <button
                      type="button"
                      className={styles.smallButton}
                      onClick={() => startEdit(part)}
                    >
                      Edit
                    </button>
                    <button
                      type="button"
                      className={styles.smallDanger}
                      disabled={removingPartId === part.partId}
                      onClick={() => handleRemove(part.partId)}
                    >
                      {removingPartId === part.partId ? 'Removing...' : 'Remove'}
                    </button>
                  </td>
                </tr>
              );
            })}
          </tbody>
          <tfoot>
            <tr>
              <td colSpan={3} className={styles.totalLabel}>
                Total
              </td>
              <td className={styles.totalValue}>{formatCurrency(total)}</td>
              <td></td>
            </tr>
          </tfoot>
        </table>
      )}

      <form className={styles.addForm} onSubmit={handleAdd}>
        <select
          className={styles.select}
          value={selectedPartId}
          aria-label="Part"
          onChange={(e) => setSelectedPartId(e.target.value)}
        >
          <option value="">Select a part…</option>
          {availableParts.map((p) => (
            <option key={p.partId} value={p.partId}>
              {p.name}
            </option>
          ))}
        </select>
        <input
          type="number"
          className={styles.qtyInput}
          value={quantity}
          min="1"
          aria-label="Quantity"
          onChange={(e) => setQuantity(e.target.value)}
        />
        <input
          type="number"
          className={styles.priceInput}
          value={unitPrice}
          min="0"
          step="0.01"
          placeholder="Unit price"
          aria-label="Unit price"
          onChange={(e) => setUnitPrice(e.target.value)}
        />
        <button
          type="submit"
          className={styles.addButton}
          disabled={adding || availableParts.length === 0}
        >
          {adding ? 'Adding...' : '+ Add part'}
        </button>
      </form>

      {libraryParts.length > 0 && availableParts.length === 0 && (
        <p className={styles.hint}>All library parts are already linked to this record.</p>
      )}
    </div>
  );
}
