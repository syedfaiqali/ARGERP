import api from './api';

const sources: Record<string,string> = {
  AttendancePolicySummary:'AttendancePolicy', EarningTypeSummary:'EarningType', EmployeeAttendanceSummary:'EmployeeAttendance',
  LeaveRequestSummary:'LeaveRequest', LeaveTypeSummary:'LeaveType', LeaveTransactionSummary:'LeaveTransaction',
  ScheduleSummary:'Schedule', ShiftSummary:'Shift', PayrollPeriodSummary:'PayrollPeriod', PayrollPeriodDetail:'PayrollPeriod',
  EmployeePerformanceEvaluation:'ERSEmployeesubjectskill',
};

// Reports read the same scoped HR APIs as their screens; no separate reporting service.
export async function hrReportBlob(name:string, parameters:Record<string,any>, format='pdf'):Promise<Blob> {
  const source=sources[name];
  if(!source) throw new Error('Unknown HR report.');
  const params=Object.fromEntries(Object.entries(parameters).map(([k,v])=>[k.charAt(0).toLowerCase()+k.slice(1),v]));
  const rows:Record<string,any>[]=[];
  for(let page=1;page<=1000;page++) {
    const response=await api.get(`/api/HumanResource/${source}/v3`,{params:{...params,pageNumber:page,pageSize:1000}});
    const body=response.data?.data??response.data;
    const items=Array.isArray(body)?body:body?.items??body?.Items??[];
    rows.push(...items);
    const total=body?.totalCount??body?.TotalCount;
    if(!items.length||Array.isArray(body)||(total!=null?rows.length>=total:items.length<1000))break;
    if(page===1000)throw new Error('Please narrow the report filters.');
  }
  if(!rows.length)throw new Error('No Data To Print');
  const keys=Object.keys(rows[0]).filter(k=>!/^ut|^clientId$|^entityId$/i.test(k)&&rows.some(r=>r[k]!=null&&typeof r[k]!=='object'));
  const labels=keys.map(k=>k.replace(/([A-Z])/g,' $1').replace(/^./,c=>c.toUpperCase()));
  const values=rows.map(r=>keys.map(k=>r[k]==null?'':String(r[k])));
  if(format==='xlsx') {
    const {default:ExcelJS}=await import('exceljs');const workbook=new ExcelJS.Workbook();const sheet=workbook.addWorksheet('HR Report');
    sheet.addRow(labels);values.forEach(row=>sheet.addRow(row));
    return new Blob([await workbook.xlsx.writeBuffer()],{type:'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'});
  }
  const {jsPDF}=await import('jspdf');const {default:autoTable}=await import('jspdf-autotable');
  const doc=new jsPDF({orientation:'landscape'});doc.setFontSize(15);doc.text('Al Rahim Group',14,14);
  doc.setFontSize(11);doc.text(name.replace(/([A-Z])/g,' $1').trim(),14,22);
  autoTable(doc,{head:[labels],body:values,startY:28,styles:{fontSize:7,overflow:'linebreak'},headStyles:{fillColor:[109,148,66]}});
  return doc.output('blob');
}
