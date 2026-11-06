import { GenericSearchModal } from "../GenericSearchModal";
import { Column } from "../DashboardTable";

interface ActivitySearchModalProps {
  open: boolean;
  onClose: () => void;
  onSelect: (row: any) => void;
  data: any[];
  selectedValue?: string | number;
}

const columns: Column[] = [
  { id: "id", label: "Code", minWidth: 100 },
  { id: "activity", label: "Activity", minWidth: 220 },
];

export const ActivitySearchModal: React.FC<ActivitySearchModalProps> = ({
  open,
  onClose,
  onSelect,
  data,
  selectedValue,
}) => {
  return (
    <GenericSearchModal
      title="Activity"
      open={open}
      onClose={onClose}
      onSelect={onSelect}
      columns={columns}
      data={data}
      breadcrumbPath="Human Resource / Employee Attendance / Activity"
      selectedValue={selectedValue}
      selectedField="id"
    />
  );
};
