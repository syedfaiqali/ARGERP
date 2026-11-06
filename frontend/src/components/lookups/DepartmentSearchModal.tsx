import React from "react";
import { GenericSearchModal } from "../GenericSearchModal";
import { Column } from "../DashboardTable";

interface DepartmentSearchModalProps {
  open: boolean;
  onClose: () => void;
  onSelect: (row: any) => void;
  data: any[];
  selectedValue?: string | number;
}

const columns: Column[] = [
  { id: "code", label: "Code", minWidth: 100 },
  { id: "name", label: "Department", minWidth: 200 },
  { id: "description", label: "Description", minWidth: 200 },
];

export const DepartmentSearchModal: React.FC<DepartmentSearchModalProps> = ({
  open,
  onClose,
  onSelect,
  data,
  selectedValue,
}) => {
  return (
    <GenericSearchModal
      title="Department"
      open={open}
      onClose={onClose}
      onSelect={onSelect}
      columns={columns}
      data={data}
      breadcrumbPath="Human Resource / Employee Monthly Attendance / Department"
      selectedValue={selectedValue}
      selectedField="name"
    />
  );
};
