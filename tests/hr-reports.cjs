const path=require('path'),fs=require('fs');
const root=path.resolve(__dirname,'../frontend');
const esbuild=require(path.join(root,'node_modules/esbuild'));
const entry=`
const assert=require('node:assert/strict');
global.localStorage={getItem:()=>null};
const api=require('./src/services/api').default;
const {hrReportBlob}=require('./src/services/hrReportService');
const requests=[];
api.defaults.adapter=async config=>{requests.push(config);return {status:200,statusText:'OK',headers:{},config,data:{items:[{id:1,code:'TEST',name:'HR report fixture'}],totalCount:1}}};
(async()=>{
const pdf=await hrReportBlob('ShiftSummary',{},'pdf');
assert.equal((await pdf.text()).slice(0,5),'%PDF-');
const xlsx=await hrReportBlob('EmployeePerformanceEvaluation',{employeeId:2},'xlsx');
assert.deepEqual(Array.from(new Uint8Array(await xlsx.arrayBuffer()).slice(0,2)),[80,75]);
assert(requests.every(r=>r.url.startsWith('/api/HumanResource/')));
assert.equal(requests[1].params.employeeId,2);
await assert.rejects(()=>hrReportBlob('UnknownReport',{}),/Unknown HR report/);
api.defaults.adapter=async config=>({status:200,statusText:'OK',headers:{},config,data:{items:[],totalCount:0}});
await assert.rejects(()=>hrReportBlob('ShiftSummary',{}),/No Data To Print/);
console.log('PASS HR reports: PDF, XLSX, HR-owned API paths, employee scope and empty results.');
})().catch(error=>{console.error(error);process.exitCode=1});`;
(async()=>{const outfile=path.join(root,'node_modules/.tmp/hr-reports.cjs');await esbuild.build({stdin:{contents:entry,resolveDir:root},bundle:true,platform:'node',format:'cjs',packages:'external',outfile,alias:{'@':path.join(root,'src')},define:{'import.meta.env':'{}'}});const result=require('child_process').spawnSync(process.execPath,[outfile],{encoding:'utf8'});process.stdout.write(result.stdout);process.stderr.write(result.stderr);process.exitCode=result.status||0})().catch(error=>{console.error(error);process.exitCode=1});
