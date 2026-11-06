import React, { forwardRef, useEffect, useRef, useState } from 'react';
import { Alert, Snackbar } from '@mui/material';
import { ImageEditorDialog } from '@/components/ImageEditorDialog';
import { useOptionalTranslation } from '@/components/AdaptiveTranslationProvider';
import { isEditableImageFile } from '@/utils/imageEditing';

// A file input adapter for existing forms. Non-image attachments keep their
// existing handler; images reach that handler only after the user applies edits.
type Props = Omit<React.InputHTMLAttributes<HTMLInputElement>, 'onChange'> & {
  onChange?: (event: React.ChangeEvent<HTMLInputElement>) => void | Promise<void>;
};
export const EditableImageInput = forwardRef<HTMLInputElement, Props>(({ onChange, disabled, ...props }, forwardedRef) => {
  const { translate } = useOptionalTranslation();
  const inputRef = useRef<HTMLInputElement | null>(null);
  const [pending, setPending] = useState<{ files: File[]; index: number; event: React.ChangeEvent<HTMLInputElement> } | null>(null);
  const [source, setSource] = useState('');
  const [error, setError] = useState('');

  useEffect(() => {
    setSource('');
    if (!pending) return;
    const url = URL.createObjectURL(pending.files[pending.index]);
    setSource(url);
    return () => URL.revokeObjectURL(url);
  }, [pending?.index, pending?.event]);

  const select = async (event: React.ChangeEvent<HTMLInputElement>) => {
    if (disabled) return;
    const files = Array.from(event.target.files || []);
    const index = files.findIndex(isEditableImageFile);
    if (index < 0) { await onChange?.(event); return; }
    if (files.some(file => isEditableImageFile(file) && file.size > 20 * 1024 * 1024)) {
      event.target.value = '';
      setError(translate('Please choose an image smaller than 20 MB.'));
      return;
    }
    // React clears currentTarget after dispatch. Preserve the actual input for
    // existing handlers that inspect currentTarget or reset its value.
    const snapshot = Object.assign(Object.create(event), { target: event.target, currentTarget: event.target }) as React.ChangeEvent<HTMLInputElement>;
    event.target.value = '';
    setPending({ files, index, event: snapshot });
  };
  const apply = async (file: File) => {
    if (!pending || !inputRef.current) return;
    const files = pending.files.map((original, index) => index === pending.index ? file : original);
    const next = files.findIndex((original, index) => index > pending.index && isEditableImageFile(original));
    if (next >= 0) { setPending({ ...pending, files, index: next }); return; }
    const transfer = new DataTransfer();
    files.forEach(item => transfer.items.add(item));
    inputRef.current.files = transfer.files;
    await onChange?.(pending.event);
    inputRef.current.value = '';
    setPending(null);
  };
  return <>
    <input {...props} type="file" disabled={disabled} ref={node => {
      inputRef.current = node;
      if (typeof forwardedRef === 'function') forwardedRef(node);
      else if (forwardedRef) forwardedRef.current = node;
    }} onChange={event => { void select(event).catch(() => setError(translate('Unable to select this file.'))); }} />
    {pending && source && <ImageEditorDialog key={pending.index} source={source} fileName={pending.files[pending.index].name} onCancel={() => setPending(null)} onApply={apply} />}
    <Snackbar open={Boolean(error)} autoHideDuration={6000} onClose={() => setError('')}><Alert severity="error" onClose={() => setError('')}>{error}</Alert></Snackbar>
  </>;
});
EditableImageInput.displayName = 'EditableImageInput';
