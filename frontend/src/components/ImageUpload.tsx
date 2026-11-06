import React, { useEffect, useId, useRef, useState } from "react";
import { Alert, Box, IconButton, Tooltip, Typography } from "@mui/material";
import {
  AddPhotoAlternate as AddPhotoAlternateIcon,
  DeleteOutline as DeleteIcon,
  Edit as EditIcon,
} from "@mui/icons-material";
import { loadImageObjectUrl } from "@/utils/loadImageObjectUrl";
import { ImageEditorDialog } from "@/components/ImageEditorDialog";
import { useOptionalTranslation } from "@/components/AdaptiveTranslationProvider";
import { isEditableImageFile } from "@/utils/imageEditing";

interface ImageUploadProps {
  onImageSelect?: (file: File) => void | Promise<void>;
  onImageRemove?: (fileId?: number | null) => void;
  currentImage?: string;
  disabled?: boolean;
  shape?: "square" | "circle";
  variant?: "default" | "dashed";
  placeholderText?: string;
  imageFit?: "cover" | "contain";
  aspectRatio?: string;
  enableEditing?: boolean;
  sx?: any;
}

export const ImageUpload: React.FC<ImageUploadProps> = ({
  onImageSelect,
  onImageRemove,
  currentImage,
  disabled = false,
  shape = "square",
  variant = "default",
  placeholderText = "Upload Photo",
  imageFit = "cover",
  aspectRatio = "1/1",
  enableEditing = true,
  sx: customSx = {},
}) => {
  const inputId = useId();
  const { translate } = useOptionalTranslation();
  const fileInputRef = useRef<HTMLInputElement>(null);
  const [previewUrl, setPreviewUrl] = useState<string>(currentImage || "");
  const [editor, setEditor] = useState<{ source: string; fileName: string } | null>(null);
  const [imageError, setImageError] = useState("");
  const isDashed = variant === "dashed";

  useEffect(() => {
    let cancelled = false;
    const raw = (currentImage || "").trim();
    if (!raw) {
      setPreviewUrl("");
      return;
    }

    // If it's already a local preview or public URL, just use it.
    if (/^data:/i.test(raw) || /^blob:/i.test(raw) || /^(https?:)?\/\//i.test(raw)) {
      setPreviewUrl(raw);
      return;
    }

    // For protected API images (relative "/api/..."), fetch via axios (adds Authorization) and use blob URL.
    loadImageObjectUrl(raw)
      .then((blobUrl) => {
        if (!cancelled) setPreviewUrl(blobUrl || raw);
      })
      .catch(() => {
        if (!cancelled) setPreviewUrl(raw);
      });

    return () => {
      cancelled = true;
    };
  }, [currentImage]);

  const handleFileSelect = (event: React.ChangeEvent<HTMLInputElement>) => {
    const file = event.target.files?.[0];
    event.target.value = "";
    if (!file) return;

    if (enableEditing) {
      setImageError("");
      if (file.size > 20 * 1024 * 1024) {
        setImageError(translate("Please choose an image smaller than 20 MB."));
        return;
      }
    }

    const reader = new FileReader();
    reader.onloadend = () => {
      if (typeof reader.result !== "string") { setImageError(translate("Unable to open this image.")); return; }
      if (enableEditing && isEditableImageFile(file)) setEditor({ source: reader.result, fileName: file.name });
      else setPreviewUrl(reader.result);
    };
    reader.readAsDataURL(file);

    if ((!enableEditing || !isEditableImageFile(file)) && onImageSelect) {
      void Promise.resolve(onImageSelect(file)).catch(() => setImageError(translate("Unable to save the image. Please try again.")));
    }
  };

  const editCurrentImage = async () => {
    if (disabled) return;
    setImageError("");
    try {
      const source = await loadImageObjectUrl(previewUrl);
      if (!source) throw new Error("No image available");
      setEditor({ source, fileName: "photo.png" });
    } catch { setImageError(translate("Unable to open this image for editing.")); }
  };

  const applyImage = async (file: File) => {
    // Do not replace the existing preview or close the editor when upload fails.
    await onImageSelect?.(file);
    const preview = await new Promise<string>((resolve, reject) => {
      const reader = new FileReader();
      reader.onload = () => resolve(reader.result as string);
      reader.onerror = () => reject(new Error("Unable to read image"));
      reader.readAsDataURL(file);
    });
    setPreviewUrl(preview);
    setEditor(null);
  };

  const handleRemove = () => {
    if (disabled) return;
    setPreviewUrl("");
    setImageError("");
    if (fileInputRef.current) {
      fileInputRef.current.value = "";
    }
    if (onImageRemove) {
      onImageRemove();
    }
  };

  return (
    <Box sx={{ width: "100%", opacity: disabled ? 0.8 : 1, ...customSx }}>
      <input
        ref={fileInputRef}
        id={inputId}
        type="file"
        accept="image/*"
        onChange={handleFileSelect}
        style={{ display: "none" }}
        disabled={disabled}
      />
      {imageError && <Alert severity="error" sx={{ mb: 1 }}>{imageError}</Alert>}
      <Box
        component="label"
        htmlFor={inputId}
        sx={{
          width: "100%",
          aspectRatio: isDashed ? undefined : aspectRatio,
          minHeight: isDashed ? 280 : undefined,
          height: isDashed ? "100%" : undefined,
          border: isDashed ? "2px dashed var(--lms-border)" : "1px solid rgba(0, 0, 0, 0.05)",
          borderRadius: shape === "circle" ? "50%" : isDashed ? "16px" : "12px",
          display: "flex",
          alignItems: "center",
          justifyContent: "center",
          bgcolor: isDashed ? "rgba(0,0,0,0.01)" : "rgba(var(--lms-primary-rgb), 0.04)",
          color: "var(--lms-primary)",
          cursor: disabled ? "default" : "pointer",
          overflow: "hidden",
          position: "relative",
          transition: "all 0.2s ease-in-out",
          boxShadow: isDashed ? "none" : "inset 0 1px 3px rgba(0,0,0,0.02)",
          "&:hover": disabled
            ? undefined
            : {
                borderColor: "var(--lms-primary)",
                bgcolor: "rgba(var(--lms-primary-rgb), 0.05)",
                transform: "translateY(-1px)",
                boxShadow: "0 4px 12px rgba(0,0,0,0.05)",
              },
        }}
      >
        {previewUrl ? (
          <Box
            sx={{
              width: "100%",
              height: "100%",
              p: shape === "circle" || isDashed ? 0 : 1.5,
              display: "flex",
              alignItems: "center",
              justifyContent: "center",
            }}
          >
            <Box
              component="img"
              src={previewUrl}
              alt="Upload"
              sx={{
                width: "100%",
                height: "100%",
                objectFit: imageFit,
                borderRadius: shape === "circle" ? "50%" : isDashed ? "14px" : "8px",
                bgcolor: "#fff",
              }}
            />
            {!disabled && (
              <IconButton
                aria-label={translate("Remove photo")}
                onClick={(e) => {
                  e.preventDefault();
                  e.stopPropagation();
                  handleRemove();
                }}
                sx={{
                  position: "absolute",
                  top: 8,
                  right: 8,
                  width: 34,
                  height: 34,
                  bgcolor: isDashed ? "rgba(255, 255, 255, 0.8)" : "rgba(0, 0, 0, 0.5)",
                  color: isDashed ? "var(--lms-text-main)" : "#fff",
                  boxShadow: "0 2px 8px rgba(0,0,0,0.2)",
                  transition: "all 0.2s",
                  "&:hover": {
                    bgcolor: "rgba(220, 53, 69, 0.9)",
                    color: "#fff",
                    transform: "scale(1.1)",
                  },
                }}
              >
                <DeleteIcon sx={{ fontSize: "1.1rem" }} />
              </IconButton>
            )}
            {!disabled && enableEditing && <Tooltip title={translate("Edit photo")}>
              <IconButton aria-label={translate("Edit photo")} onClick={e => { e.preventDefault(); e.stopPropagation(); void editCurrentImage(); }} sx={{ position: "absolute", top: 8, left: 8, bgcolor: "rgba(255,255,255,0.9)", color: "var(--lms-primary)", "&:hover": { bgcolor: "#fff" } }}><EditIcon fontSize="small" /></IconButton>
            </Tooltip>}
          </Box>
        ) : (
          <Box sx={{ textAlign: "center", px: 2 }}>
            {isDashed ? (
              <AddPhotoAlternateIcon
                sx={{ fontSize: "4rem", color: "var(--lms-primary)", mb: 1, opacity: 0.6 }}
              />
            ) : (
              <Box
                sx={{
                  width: 60,
                  height: 60,
                  bgcolor: "rgba(var(--lms-primary-rgb), 0.1)",
                  borderRadius: "12px",
                  display: "flex",
                  alignItems: "center",
                  justifyContent: "center",
                  margin: "0 auto",
                  mb: 1.5,
                }}
              >
                <AddPhotoAlternateIcon
                  sx={{ fontSize: "2rem", color: "var(--lms-primary)" }}
                />
              </Box>
            )}
            <Typography
              sx={{
                fontWeight: isDashed ? 700 : 800,
                fontSize: isDashed ? "0.85rem" : "0.9rem",
                color: isDashed ? "var(--lms-text-secondary)" : "var(--lms-text-main)",
                letterSpacing: 0,
              }}
            >
              {placeholderText}
            </Typography>
          </Box>
        )}
      </Box>
      {editor && <ImageEditorDialog source={editor.source} fileName={editor.fileName} onCancel={() => setEditor(null)} onApply={applyImage} />}
    </Box>
  );
};
