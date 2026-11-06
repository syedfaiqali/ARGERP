import React from "react";
import { GenericSearchModal } from "../GenericSearchModal";
import { Column } from "../DashboardTable";

export interface DeviceSearchModalProps {
  open: boolean;
  onClose: () => void;
  data: any[];
  selectedValue?: string;
  onSelect: (row: any) => void;
}

export const DeviceSearchModal: React.FC<DeviceSearchModalProps> = ({
  open,
  onClose,
  data,
  selectedValue,
  onSelect,
}) => {
  const columns: Column[] = [
    { id: "code", label: "Code", minWidth: 100 },
    { id: "shortName", label: "Short Name", minWidth: 150 },
    { id: "description", label: "Description", minWidth: 200 },
  ];

  return (
    <GenericSearchModal
      open={open}
      onClose={onClose}
      title="Device"
      columns={columns}
      data={data}
      onSelect={onSelect}
      selectedValue={selectedValue}
      selectedField="shortName"
    />
  );
};
