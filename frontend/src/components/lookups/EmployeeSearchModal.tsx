import { GenericSearchModal } from "../GenericSearchModal";
import { Column } from "../DashboardTable";

interface EmployeeSearchModalProps {
  open: boolean;
  onClose: () => void;
  onSelect: (row: any) => void;
  data: any[];
  selectedValue?: string | number;
  title?: string;
  breadcrumbPath?: string;
  columns?: Column[];
  selectedField?: string;
}

const defaultColumns: Column[] = [
  { id: "employeeCode", label: "Code", minWidth: 120 },
  { id: "employeeName", label: "Full Name", minWidth: 200 },
  { id: "department", label: "Department", minWidth: 150 },
  { id: "designation", label: "Designation", minWidth: 150 },
  { id: "machineCode", label: "Machine Code", minWidth: 140 },
  { id: "shift", label: "Shift", minWidth: 140 },
];

export const EmployeeSearchModal: React.FC<EmployeeSearchModalProps> = ({
  open,
  onClose,
  onSelect,
  data,
  selectedValue,
  title = "Employee",
  breadcrumbPath = "Human Resource / Employee Attendance / Employee",
  columns,
  selectedField = "employeeCode",
}) => {
  return (
    <GenericSearchModal
      title={title}
      open={open}
      onClose={onClose}
      onSelect={onSelect}
      columns={columns ?? defaultColumns}
      data={data}
      breadcrumbPath={breadcrumbPath}
      selectedValue={selectedValue}
      selectedField={selectedField}
    />
  );
};
