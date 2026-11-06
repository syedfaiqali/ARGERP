import React from "react";
import { GenericSearchModal } from "../GenericSearchModal";
import { Column } from "../DashboardTable";

interface PeriodSearchModalProps {
  open: boolean;
  onClose: () => void;
  onSelect: (row: any) => void;
  data: any[];
  selectedValue?: string | number;
}

const columns: Column[] = [
  { id: "year", label: "Year", minWidth: 80 },
  { id: "month", label: "Month", minWidth: 90 },
  { id: "periodText", label: "Period", minWidth: 120 },
  { id: "startDate", label: "Start Date", minWidth: 110 },
  { id: "endDate", label: "End Date", minWidth: 110 },
  { id: "numberOfDays", label: "# of Days", minWidth: 90 },
  { id: "disbursementDate", label: "Disbursement Date", minWidth: 140 },
];

export const PeriodSearchModal: React.FC<PeriodSearchModalProps> = ({
  open,
  onClose,
  onSelect,
  data,
  selectedValue,
}) => {
  return (
    <GenericSearchModal
      title="Period"
      open={open}
      onClose={onClose}
      onSelect={onSelect}
      columns={columns}
      data={data}
      breadcrumbPath="Human Resource / Employee Monthly Attendance / Period"
      selectedValue={selectedValue}
      selectedField="periodText"
    />
  );
};
