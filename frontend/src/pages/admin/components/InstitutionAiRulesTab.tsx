import React from "react";
import { Box, TextField, Typography } from "@mui/material";
import { T, useTranslation } from "@/components/AdaptiveTranslationProvider";

/** Rules the user writes for each AI feature, as bullet-point text, by OrgAiRule feature code. */
export type AiRules = Record<string, string>;

const BULLET = "• ";

// One heading per AI feature, keyed by its OrgAiRule.featurecodeid. Terms are
// written for schools and translated for other institution types.
const AI_RULE_SECTIONS = [
  {
    key: "TTG",
    title: "Timetable",
    description: "AI Suggestions and AI Auto-fill on Schedule Timetable follow these rules after the built-in scheduling rules.",
    placeholder: "• No Teacher should teach more than 3 periods in a row\n• Keep Friday after 12:00 PM free for every Class",
  },
  {
    key: "CBG",
    title: "AI Chatbot",
    description: "The AI Chatbot follows these rules when it answers.",
    placeholder: "• Reply in English unless the user writes in Urdu\n• Do not share fee details of other Students",
  },
];

interface InstitutionAiRulesTabProps {
  rules: AiRules;
  onChange: (rules: AiRules) => void;
  disabled?: boolean;
  /** Shown above the headings when the rules can't be edited, saying why. */
  notice?: string;
}

export const InstitutionAiRulesTab: React.FC<InstitutionAiRulesTabProps> = ({ rules, onChange, disabled = false, notice }) => {
  const { translate } = useTranslation();
  const setRules = (key: string, text: string) => onChange({ ...rules, [key]: text });

  // Each new line starts with a bullet, so rules stay one point each.
  const onKeyDown = (key: string) => (e: React.KeyboardEvent<HTMLDivElement>) => {
    if (e.key !== "Enter" || e.shiftKey || !(e.target instanceof HTMLTextAreaElement)) return;
    e.preventDefault();
    const input = e.target;
    const text = rules[key] ?? "";
    const start = input.selectionStart ?? text.length;
    const end = input.selectionEnd ?? text.length;
    const next = `${text.slice(0, start)}\n${BULLET}${text.slice(end)}`;
    setRules(key, next);
    requestAnimationFrame(() => input.setSelectionRange(start + 1 + BULLET.length, start + 1 + BULLET.length));
  };

  return (
    <Box sx={{ display: "flex", flexDirection: "column", gap: 3 }}>
      {notice && (
        <Typography variant="body2" sx={{ p: 1.5, borderRadius: 1.5, bgcolor: "#fff8e1", border: "1px solid #ffe082", color: "#7a5b00" }}>
          {notice}
        </Typography>
      )}
      {AI_RULE_SECTIONS.map((section) => (
        <Box key={section.key}>
          <Box sx={{ display: "flex", alignItems: "center", gap: 1.5, mb: 0.5 }}>
            <Box sx={{ width: 4, height: 18, bgcolor: "var(--lms-primary)", borderRadius: 1 }} />
            <Typography sx={{ fontWeight: 800, fontSize: "1rem", color: "#333" }}>
              <T>{section.title}</T>
            </Typography>
          </Box>
          <Typography variant="body2" sx={{ mb: 1.25, color: "var(--lms-muted, #6b7280)" }}>
            {translate(section.description)}
          </Typography>
          <TextField
            fullWidth
            multiline
            minRows={5}
            disabled={disabled}
            value={rules[section.key] ?? ""}
            placeholder={translate(section.placeholder)}
            onFocus={() => { if (!rules[section.key]) setRules(section.key, BULLET); }}
            onBlur={() => { if ((rules[section.key] ?? "").trim() === BULLET.trim()) setRules(section.key, ""); }}
            onKeyDown={onKeyDown(section.key)}
            onChange={(e) => setRules(section.key, e.target.value)}
            sx={{ "& .MuiInputBase-root": { bgcolor: "#fff", fontSize: "0.9rem", lineHeight: 1.7 } }}
          />
        </Box>
      ))}
    </Box>
  );
};
