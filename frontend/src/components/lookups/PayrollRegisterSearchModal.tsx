import React from "react";
import { GenericSearchModal } from "../GenericSearchModal";
import { Column } from "../DashboardTable";

interface PayrollRegisterSearchModalProps {
  open: boolean;
  onClose: () => void;
  onSelect: (row: any) => void;
  data: any[];
  selectedValue?: string | number;
}

const columns: Column[] = [
  { id: "code", label: "Code", minWidth: 100 },
  { id: "payrollSetup", label: "Payroll Setup", minWidth: 200 },
  { id: "description", label: "Description", minWidth: 200 },
];

export const PayrollRegisterSearchModal: React.FC<PayrollRegisterSearchModalProps> = ({
  open,
  onClose,
  onSelect,
  data,
  selectedValue,
}) => {
  return (
    <GenericSearchModal
      title="Payroll Register"
      open={open}
      onClose={onClose}
      onSelect={onSelect}
      columns={columns}
      data={data}
      breadcrumbPath="Human Resource / Employee Monthly Attendance / Payroll Register"
      selectedValue={selectedValue}
      selectedField="payrollSetup"
    />
  );
};
