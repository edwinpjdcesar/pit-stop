import { useEffect, useState } from 'react';
import { Link, useNavigate, useParams } from 'react-router-dom';
import Header from '../components/Header';
import { createPart, deletePart, getPart, updatePart } from '../services/partService';
import styles from './PartFormPage.module.css';

type FormErrors = {
  name?: string;
  description?: string;
};

export default function PartFormPage() {
  const { partId } = useParams<{ partId: string }>();
  const navigate = useNavigate();
  const isEditing = !!partId;

  const [name, setName] = useState('');
  const [modelNumber, setModelNumber] = useState('');
  const [description, setDescription] = useState('');

  const [loading, setLoading] = useState(isEditing);
  const [errors, setErrors] = useState<FormErrors>({});
  const [submitError, setSubmitError] = useState<string | null>(null);
  const [submitting, setSubmitting] = useState(false);
  const [deleting, setDeleting] = useState(false);

  useEffect(() => {
    if (!partId) return;
    getPart(partId)
      .then((part) => {
        setName(part.name);
        setModelNumber(part.modelNumber ?? '');
        setDescription(part.description);
      })
      .catch((err: Error) => setSubmitError(err.message))
      .finally(() => setLoading(false));
  }, [partId]);

  function validate(): FormErrors {
    const errs: FormErrors = {};
    if (!name.trim()) errs.name = 'Name is required.';
    if (!description.trim()) errs.description = 'Description is required.';
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
      name: name.trim(),
      modelNumber: modelNumber.trim() || null,
      description: description.trim(),
    };

    try {
      if (isEditing) {
        await updatePart(partId!, request);
      } else {
        await createPart(request);
      }
      navigate('/parts');
    } catch (err: unknown) {
      setSubmitError(err instanceof Error ? err.message : 'Something went wrong. Please try again.');
    } finally {
      setSubmitting(false);
    }
  }

  async function handleDelete() {
    if (!partId || !window.confirm('Delete this part? This cannot be undone.')) return;
    setDeleting(true);
    setSubmitError(null);
    try {
      await deletePart(partId);
      navigate('/parts');
    } catch (err: unknown) {
      setSubmitError(err instanceof Error ? err.message : 'Failed to delete part.');
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

  return (
    <div className={styles.page}>
      <Header />

      <main className={styles.main}>
        <div className={styles.toolbar}>
          <Link to="/parts" className={styles.back}>← Parts Library</Link>
          <h1>{isEditing ? 'Edit Part' : 'New Part'}</h1>
        </div>

        <form className={styles.form} onSubmit={handleSubmit} noValidate>
          <div className={styles.field}>
            <label htmlFor="name">
              Name <span className={styles.required}>*</span>
            </label>
            <input
              id="name"
              type="text"
              value={name}
              onChange={(e) => setName(e.target.value)}
              placeholder="e.g. Oil Filter"
            />
            {errors.name && <span className={styles.fieldError}>{errors.name}</span>}
          </div>

          <div className={styles.field}>
            <label htmlFor="modelNumber">Model Number</label>
            <input
              id="modelNumber"
              type="text"
              value={modelNumber}
              onChange={(e) => setModelNumber(e.target.value)}
              placeholder="e.g. PH3614"
            />
          </div>

          <div className={styles.field}>
            <label htmlFor="description">
              Description <span className={styles.required}>*</span>
            </label>
            <textarea
              id="description"
              value={description}
              onChange={(e) => setDescription(e.target.value)}
              placeholder="e.g. Standard oil filter for 4-cylinder engines"
              rows={3}
            />
            {errors.description && <span className={styles.fieldError}>{errors.description}</span>}
          </div>

          {submitError && <p className={styles.submitError}>{submitError}</p>}

          <div className={styles.actions}>
            {isEditing && (
              <button
                type="button"
                className={styles.deleteButton}
                onClick={handleDelete}
                disabled={deleting || submitting}
              >
                {deleting ? 'Deleting...' : 'Delete Part'}
              </button>
            )}
            <div className={styles.rightActions}>
              <Link to="/parts" className={styles.cancelButton}>
                Cancel
              </Link>
              <button type="submit" className={styles.submitButton} disabled={submitting || deleting}>
                {submitting ? 'Saving...' : isEditing ? 'Save Changes' : 'Create Part'}
              </button>
            </div>
          </div>
        </form>
      </main>
    </div>
  );
}
