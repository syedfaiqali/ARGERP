import TextField from "@/components/ClearableTextField";
import React, { useEffect, useRef, useState } from 'react';
import { Alert, Box, Button, Checkbox, CircularProgress, Dialog, DialogActions, DialogContent, DialogTitle, FormControlLabel, MenuItem, Slider, Stack, Typography } from "@mui/material";
import { Flip, RestartAlt, RotateLeft, RotateRight } from '@mui/icons-material';
import { useOptionalTranslation } from '@/components/AdaptiveTranslationProvider';
import { drawEditedImage, exportEditedImage, fitImageSize, imageEditBounds, MAX_IMAGE_DIMENSION } from '@/utils/imageEditing';

interface Props {
  source: string;
  fileName: string;
  onCancel: () => void;
  onApply: (file: File) => void | Promise<void>;
}

export const ImageEditorDialog: React.FC<Props> = ({ source, fileName, onCancel, onApply }) => {
  const { translate: t } = useOptionalTranslation();
  const canvasRef = useRef<HTMLCanvasElement>(null);
  const [image, setImage] = useState<HTMLImageElement | null>(null);
  const [rotation, setRotation] = useState(0);
  const [flipX, setFlipX] = useState(false);
  const [flipY, setFlipY] = useState(false);
  const [crop, setCrop] = useState('original');
  const [positionX, setPositionX] = useState(50);
  const [positionY, setPositionY] = useState(50);
  const [width, setWidth] = useState('');
  const [height, setHeight] = useState('');
  const [locked, setLocked] = useState(true);
  const initialMime = /\.jpe?g$/i.test(fileName) ? 'image/jpeg' : 'image/png';
  const [mime, setMime] = useState<'image/png' | 'image/jpeg'>(initialMime);
  const [quality, setQuality] = useState(90);
  const [busy, setBusy] = useState(false);
  const [error, setError] = useState('');
  const cropRatio = crop === 'original' ? null : Number(crop);
  const options = { rotation, flipX, flipY, cropRatio, positionX, positionY, width: Number(width), height: Number(height) };
  const valid = [options.width, options.height].every(n => Number.isInteger(n) && n >= 1 && n <= MAX_IMAGE_DIMENSION);

  useEffect(() => {
    let cancelled = false;
    const next = new Image();
    next.crossOrigin = 'anonymous';
    next.onload = () => {
      if (cancelled) return;
      if (next.naturalWidth * next.naturalHeight > 40000000) { setError(t('Please choose an image smaller than 40 megapixels.')); return; }
      const size = fitImageSize(next.naturalWidth, next.naturalHeight);
      setWidth(String(size.width)); setHeight(String(size.height)); setImage(next);
    };
    next.onerror = () => { if (!cancelled) setError(t('Unable to open this image. Please use a PNG, JPEG or WebP image.')); };
    next.src = source;
    return () => { cancelled = true; };
  }, [source]);

  useEffect(() => {
    if (!image || !canvasRef.current || !valid) return;
    try { drawEditedImage(image, canvasRef.current, options, true); }
    catch { setError(t('Unable to preview this image.')); }
  }, [image, rotation, flipX, flipY, crop, positionX, positionY, width, height]);

  const resize = (value: string, axis: 'width' | 'height', keepRatio = locked) => {
    if (!/^\d*$/.test(value)) return;
    (axis === 'width' ? setWidth : setHeight)(value);
    if (keepRatio && image && Number(value) > 0) {
      const bounds = imageEditBounds(image.naturalWidth, image.naturalHeight, rotation, cropRatio);
      const ratio = bounds.cropWidth / bounds.cropHeight;
      (axis === 'width' ? setHeight : setWidth)(String(Math.max(1, Math.round(axis === 'width' ? Number(value) / ratio : Number(value) * ratio))));
    }
  };
  const changeCrop = (value: string) => {
    if (!image) return;
    const bounds = imageEditBounds(image.naturalWidth, image.naturalHeight, rotation, value === 'original' ? null : Number(value));
    const size = fitImageSize(bounds.cropWidth, bounds.cropHeight);
    setCrop(value); setPositionX(50); setPositionY(50); setWidth(String(size.width)); setHeight(String(size.height));
  };
  const rotate = (delta: number) => {
    const next = (rotation + delta + 360) % 360;
    setRotation(next);
    if (crop === 'original') { setWidth(height); setHeight(width); }
  };
  const reset = () => {
    if (!image) return;
    const size = fitImageSize(image.naturalWidth, image.naturalHeight);
    setRotation(0); setFlipX(false); setFlipY(false); setCrop('original'); setPositionX(50); setPositionY(50);
    setWidth(String(size.width)); setHeight(String(size.height)); setLocked(true); setMime(initialMime); setQuality(90); setError('');
  };
  const apply = async () => {
    if (!image || !valid) return;
    setBusy(true); setError('');
    try { await onApply(await exportEditedImage(image, options, fileName, mime, quality / 100)); }
    catch { setError(t('Unable to save the image. Please try again.')); }
    finally { setBusy(false); }
  };

  return <Dialog open fullWidth maxWidth="md" onClick={event => event.stopPropagation()} onClose={() => { if (!busy) onCancel(); }} aria-labelledby="image-editor-title">
    <DialogTitle id="image-editor-title">{t('Edit photo')}</DialogTitle>
    <DialogContent dividers>
      {error && <Alert severity="error" sx={{ mb: 2 }}>{error}</Alert>}
      {!image ? (!error && <Box sx={{ p: 6, textAlign: 'center' }}><CircularProgress aria-label={t('Loading image')} /></Box>) :
        <Stack direction={{ xs: 'column', md: 'row' }} spacing={3}>
          <Box sx={{ flex: 1, minWidth: 0, minHeight: 240, display: 'flex', alignItems: 'center', justifyContent: 'center', bgcolor: '#f1f3f5', p: 2, borderRadius: 2 }}>
            <canvas ref={canvasRef} aria-label={t('Edited image preview')} style={{ display: 'block', maxWidth: '100%', maxHeight: 400, objectFit: 'contain' }} />
          </Box>
          <Stack spacing={2} sx={{ width: { xs: '100%', md: 300 } }}>
            <Typography variant="body2">{t('Original size')}: {image.naturalWidth} × {image.naturalHeight} px</Typography>
            <Box component="fieldset" disabled={busy} sx={{ m: 0, p: 0, border: 0, minWidth: 0 }}>
              <Stack spacing={2}>
                <Stack direction="row" spacing={1}><Button size="small" startIcon={<RotateLeft />} onClick={() => rotate(-90)}>{t('Rotate left')}</Button><Button size="small" startIcon={<RotateRight />} onClick={() => rotate(90)}>{t('Rotate right')}</Button></Stack>
                <Stack direction="row" spacing={1}><Button size="small" variant={flipX ? 'contained' : 'outlined'} startIcon={<Flip />} onClick={() => setFlipX(!flipX)}>{t('Flip horizontal')}</Button><Button size="small" variant={flipY ? 'contained' : 'outlined'} onClick={() => setFlipY(!flipY)}>{t('Flip vertical')}</Button></Stack>
                <TextField select size="small" label={t('Crop')} value={crop} onChange={e => changeCrop(e.target.value)}>
                  <MenuItem value="original">{t('Full image')}</MenuItem><MenuItem value="1">{t('Square (1:1)')}</MenuItem><MenuItem value="0.75">{t('Portrait (3:4)')}</MenuItem><MenuItem value={String(4 / 3)}>{t('Landscape (4:3)')}</MenuItem>
                </TextField>
                {crop !== 'original' && <><Typography variant="caption">{t('Horizontal crop position')}</Typography><Slider aria-label={t('Horizontal crop position')} value={positionX} onChange={(_,v) => setPositionX(v as number)} /><Typography variant="caption">{t('Vertical crop position')}</Typography><Slider aria-label={t('Vertical crop position')} value={positionY} onChange={(_,v) => setPositionY(v as number)} /></>}
                <Stack direction="row" spacing={1}>
                  <TextField size="small" label={t('Width (px)')} value={width} onChange={e => resize(e.target.value,'width')} inputProps={{ inputMode: 'numeric' }} error={!valid} />
                  <TextField size="small" label={t('Height (px)')} value={height} onChange={e => resize(e.target.value,'height')} inputProps={{ inputMode: 'numeric' }} error={!valid} />
                </Stack>
                {!valid && <Typography color="error" variant="caption">{t('Enter image dimensions between 1 and 4096 pixels.')}</Typography>}
                <FormControlLabel control={<Checkbox checked={locked} onChange={e => { setLocked(e.target.checked); if (e.target.checked) resize(width,'width',true); }} />} label={t('Keep proportions')} />
                <TextField select size="small" label={t('Image format')} value={mime} onChange={e => setMime(e.target.value as typeof mime)}><MenuItem value="image/png">PNG</MenuItem><MenuItem value="image/jpeg">JPEG</MenuItem></TextField>
                {mime === 'image/jpeg' && <><Typography variant="caption">{t('Quality')}: {quality}%</Typography><Slider min={30} max={100} value={quality} aria-label={t('Image quality')} onChange={(_,v) => setQuality(v as number)} /></>}
                <Button startIcon={<RestartAlt />} onClick={reset}>{t('Reset')}</Button>
              </Stack>
            </Box>
          </Stack>
        </Stack>}
    </DialogContent>
    <DialogActions sx={{ p: 2 }}><Button disabled={busy} onClick={onCancel}>{t('Cancel')}</Button><Button variant="contained" disabled={!image || !valid || busy} onClick={() => void apply()}>{busy ? t('Saving…') : t('Apply photo')}</Button></DialogActions>
  </Dialog>;
};
