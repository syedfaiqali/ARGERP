import React from "react";
import { GenericSearchModal } from "../GenericSearchModal";
import { Column } from "../DashboardTable";

interface DesignationSearchModalProps {
  open: boolean;
  onClose: () => void;
  onSelect: (row: any) => void;
  data: any[];
  selectedValue?: string | number;
}

const columns: Column[] = [
  { id: "code", label: "Designation Code", minWidth: 130 },
  { id: "name", label: "Designation", minWidth: 180 },
  { id: "description", label: "Description", minWidth: 200 },
  { id: "account", label: "Account", minWidth: 150 },
];

export const DesignationSearchModal: React.FC<DesignationSearchModalProps> = ({
  open,
  onClose,
  onSelect,
  data,
  selectedValue,
}) => {
  return (
    <GenericSearchModal
      title="Designation"
      open={open}
      onClose={onClose}
      onSelect={onSelect}
      columns={columns}
      data={data}
      breadcrumbPath="Human Resource / Employee Monthly Attendance / Designation"
      selectedValue={selectedValue}
      selectedField="name"
    />
  );
};
