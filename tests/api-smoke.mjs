import fs from 'node:fs';
const base='http://127.0.0.1:5181';
const credentials=JSON.parse(fs.readFileSync(new URL('../.local/settings.json',import.meta.url),'utf8').replace(/^\uFEFF/,''));
const login=await fetch(base+'/api/auth/login',{method:'POST',headers:{'Content-Type':'application/json'},body:JSON.stringify({username:'admin',password:credentials.adminPassword})});
if(login.status!==200)throw Error('Login failed: '+login.status+' '+await login.text());
const session=await login.json();
const headers={Authorization:'Bearer '+session.accessToken,'X-Client-Id':'1','X-Entity-Id':'1','X-Campus-Id':'1'};
const paths=new Set(['/api/auth/me','/api/auth/Legalentity/v3/Legalentity','/api/Workflow/v3/me','/api/Workflow/v3/actions','/api/Workflow/v3/directory?clientId=1','/api/auth/HR/v3/attrition?from=2026-01-01&to=2026-12-31','/api/HumanResource/EmployeeProfile/v3/defaults','/api/HumanResource/EmployeeProfile/v3/creation-policy','/api/HumanResource/EmployeeProfile/v3/next-employee-code',...['religions','departments','designations','institutions','entities','countries','states','cities','sites'].map(name=>'/api/Lookups/v3/'+name)]);
for(const service of ['HumanResource','Organization']){
const directory=new URL('../src/Services/'+service+'.Service/Controllers/',import.meta.url);
for(const name of fs.readdirSync(directory).filter(n=>n.endsWith('.cs'))){const text=fs.readFileSync(new URL(name,directory),'utf8');if(!/\[HttpGet\]/.test(text))continue;const routes=[...text.matchAll(/\[Route\("([^"{]+)"\)\]/g)].map(m=>m[1]);if(!routes.length)continue;let route=routes[0].replace('[controller]',(text.match(/class\s+(\w+Controller)\b/)?.[1] || name.replace(/\.cs$/, '')).replace(/Controller$/, ''));if(service==='HumanResource'&&!route.startsWith('api/auth/'))route='api/HumanResource/'+route.replace(/^api\/(?:HumanResource\/)?/,'');if(service==='Organization')route='api/organization/'+route.replace(/^api\//,'');paths.add('/'+route+'?pageNumber=1&pageSize=20');}}
for (const kind of ['banks','religions','relations','diseases','employment-types','income-tax','classes','teaching-subjects']) paths.add('/api/HumanResource/Reference/v3/'+kind);
for (const report of ['employees','attendance','payslips']) paths.add('/api/HumanResource/Reports/v3/'+report+'?from=2020-01-01&to=2026-12-31');
paths.add('/api/arg/users');paths.add('/api/arg/users/options');
paths.add('/api/Academics/Subjects/v3?entityId=1');paths.add('/api/Academics/Courses/v3?entityId=1');
paths.add('/api/HumanResource/EmployeeDocuments/v3?employeeId=2');
paths.add('/api/HumanResource/EmployeeDocuments/v3/document-types');
paths.add('/api/HumanResource/EmployeeProfile/v3/2');
const results=[];
for(const path of paths){try{const response=await fetch(base+path,{headers,signal:AbortSignal.timeout(25000)});const body=await response.text();results.push({path,status:response.status,...(response.status>=400?{body:body.slice(0,250)}:{})});}catch(error){results.push({path,status:0,error:error.message});}}
for (const removed of ['LoanType','LoanRequest','LoanApproval','Vacancy','Organogram']) {
 const response=await fetch(base+'/api/HumanResource/'+removed+'/v3',{headers});
 if(response.status!==404) throw Error('Removed module is still reachable: '+removed+' '+response.status);
}
const anonymous=await fetch(base+'/api/HumanResource/EmployeeProfile/v3');if(anonymous.status!==401)throw Error('Anonymous access was not denied.');
const invalid=await fetch(base+'/api/auth/login',{method:'POST',headers:{'Content-Type':'application/json'},body:JSON.stringify({username:'admin',password:'incorrect'})});if(invalid.status!==401)throw Error('Invalid credentials were not denied.');
fs.mkdirSync(new URL('../artifacts/',import.meta.url),{recursive:true});fs.writeFileSync(new URL('../artifacts/api-smoke.json',import.meta.url),JSON.stringify(results,null,2));
const failed=results.filter(r=>r.status>=500||r.status===404||r.status===0||r.status===401||r.status===403);
console.log(JSON.stringify({checked:results.length,passed:results.length-failed.length,failures:failed},null,2));if(failed.length)process.exitCode=1;
