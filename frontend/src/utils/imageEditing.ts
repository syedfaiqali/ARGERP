export interface ImageEditOptions {
  rotation: number;
  flipX: boolean;
  flipY: boolean;
  cropRatio: number | null;
  positionX: number;
  positionY: number;
  width: number;
  height: number;
}

export const MAX_IMAGE_DIMENSION = 4096;

export const isEditableImageFile = (file: File) =>
  /^image\/(png|jpe?g|webp|bmp|x-ms-bmp)$/i.test(file.type) ||
  (!file.type && /\.(png|jpe?g|webp|bmp)$/i.test(file.name));

export function imageEditBounds(width: number, height: number, rotation: number, cropRatio: number | null) {
  const quarterTurn = Math.abs(rotation % 180) === 90;
  const rotatedWidth = quarterTurn ? height : width;
  const rotatedHeight = quarterTurn ? width : height;
  const ratio = cropRatio || rotatedWidth / rotatedHeight;
  const cropWidth = Math.min(rotatedWidth, rotatedHeight * ratio);
  const cropHeight = Math.min(rotatedHeight, rotatedWidth / ratio);
  return { rotatedWidth, rotatedHeight, cropWidth, cropHeight };
}

export function fitImageSize(width: number, height: number, maxSize = 1200) {
  const scale = Math.min(1, maxSize / Math.max(width, height));
  return { width: Math.max(1, Math.round(width * scale)), height: Math.max(1, Math.round(height * scale)) };
}

export function drawEditedImage(image: HTMLImageElement, canvas: HTMLCanvasElement, options: ImageEditOptions, preview = false) {
  if (![options.width, options.height].every(n => Number.isInteger(n) && n > 0 && n <= MAX_IMAGE_DIMENSION))
    throw new Error('Enter image dimensions between 1 and 4096 pixels.');
  const { rotatedWidth, rotatedHeight, cropWidth, cropHeight } = imageEditBounds(image.naturalWidth, image.naturalHeight, options.rotation, options.cropRatio);
  const size = preview ? fitImageSize(options.width, options.height, 720) : options;
  canvas.width = size.width;
  canvas.height = size.height;
  const context = canvas.getContext('2d');
  if (!context) throw new Error('Image editing is unavailable in this browser.');
  context.imageSmoothingEnabled = true;
  context.imageSmoothingQuality = 'high';
  context.scale(size.width / cropWidth, size.height / cropHeight);
  context.translate(-(rotatedWidth - cropWidth) * options.positionX / 100, -(rotatedHeight - cropHeight) * options.positionY / 100);
  context.translate(rotatedWidth / 2, rotatedHeight / 2);
  // Flips follow the displayed axes, even after a quarter turn.
  context.scale(options.flipX ? -1 : 1, options.flipY ? -1 : 1);
  context.rotate(options.rotation * Math.PI / 180);
  context.drawImage(image, -image.naturalWidth / 2, -image.naturalHeight / 2);
}

export async function exportEditedImage(image: HTMLImageElement, options: ImageEditOptions, name: string, mime: 'image/png' | 'image/jpeg', quality: number) {
  const canvas = document.createElement('canvas');
  drawEditedImage(image, canvas, options);
  if (mime === 'image/jpeg') {
    const context = canvas.getContext('2d')!;
    context.setTransform(1, 0, 0, 1, 0, 0);
    context.globalCompositeOperation = 'destination-over';
    context.fillStyle = '#ffffff';
    context.fillRect(0, 0, canvas.width, canvas.height);
  }
  const blob = await new Promise<Blob>((resolve, reject) => canvas.toBlob(value => value ? resolve(value) : reject(new Error('Unable to export this image.')), mime, quality));
  const baseName = name.replace(/\.[^.]+$/, '') || 'photo';
  return new File([blob], `${baseName}.${mime === 'image/png' ? 'png' : 'jpg'}`, { type: mime });
}
