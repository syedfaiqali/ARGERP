import React from "react";
import { GenericSearchModal } from "../GenericSearchModal";
import { Column } from "../DashboardTable";

interface SiteSearchModalProps {
  open: boolean;
  onClose: () => void;
  onSelect: (row: any) => void;
  data: any[];
  selectedValue?: string | number;
}

const columns: Column[] = [
  { id: "code", label: "Code", minWidth: 80 },
  { id: "stxt", label: "Site Name", minWidth: 200 },
  { id: "sitetypename", label: "Site Type", minWidth: 150 },
  { id: "siteforname", label: "Region", minWidth: 150 },
];

export const SiteSearchModal: React.FC<SiteSearchModalProps> = ({
  open,
  onClose,
  onSelect,
  data,
  selectedValue,
}) => {
  return (
    <GenericSearchModal
      title="Site"
      open={open}
      onClose={onClose}
      onSelect={onSelect}
      columns={columns}
      data={data}
      breadcrumbPath="Human Resource / Employee Daily Attendance / Site"
      selectedValue={selectedValue}
      selectedField="stxt"
    />
  );
};
