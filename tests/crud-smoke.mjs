import fs from 'node:fs';import assert from 'node:assert/strict';import {randomUUID} from 'node:crypto';
const settings=JSON.parse(fs.readFileSync(new URL('../.local/settings.json',import.meta.url),'utf8').replace(/^\uFEFF/,''));
const base=(process.env.ARG_API_BASE || 'http://127.0.0.1:5181'),login=await fetch(base+'/api/auth/login',{method:'POST',headers:{'Content-Type':'application/json'},body:JSON.stringify({username:'admin',password:settings.adminPassword})}).then(r=>r.json());
const headers={Authorization:'Bearer '+login.accessToken,'Content-Type':'application/json','X-Client-Id':'1','X-Entity-Id':'1','X-Campus-Id':'1'};
async function request(method,url,body){const response=await fetch(base+url,{method,headers:{...headers,'Idempotency-Key':randomUUID()},body:body?JSON.stringify(body):undefined});const text=await response.text();if(!response.ok)throw Error(method+' '+url+' returned '+response.status+' '+text);return text?JSON.parse(text):null;}
const suffix=Date.now().toString().slice(-7),results=[];
const cases=[{name:'Designation',path:'/api/organization/Designation/v3',body:{designationName:'Smoke '+suffix,description:'Disposable standalone integration test',inactive:false},key:'designationName'},{name:'Department',path:'/api/organization/Department/v3',body:{departmentName:'Smoke '+suffix,description:'Disposable standalone integration test'},key:'departmentName'},{name:'Qualification Type',path:'/api/HumanResource/QualificationType/v3',body:{code:'Q'+suffix,name:'Smoke '+suffix,description:'Disposable test',isActive:true},key:'name'},{name:'Skill Type',path:'/api/HumanResource/SkillType/v3',body:{code:'S'+suffix,name:'Smoke '+suffix,description:'Disposable test',isActive:true},key:'name'}];
for(const test of cases){let id;try{const saved=await request('POST',test.path,test.body);assert.notEqual(saved.disposition,'PendingApproval','Fresh installation must use direct save until an approval policy is configured.');id=saved.id;assert.ok(id);const loaded=await request('GET',test.path+'/'+id);assert.equal(loaded[test.key],test.body[test.key]);const next={...test.body,...saved,id,[test.key]:test.body[test.key]+' Updated'};await request('PUT',test.path+'/'+id,next);const updated=await request('GET',test.path+'/'+id);assert.equal(updated[test.key],next[test.key]);results.push({name:test.name,create:true,read:true,update:true});}finally{if(id)await request('DELETE',test.path+'/'+id);}}
for(const kind of ['banks','religions','relations','diseases','employment-types','income-tax','classes','teaching-subjects']) {
 const path='/api/HumanResource/Reference/v3/'+kind;let id;
 try {
  const body={code:'T'+suffix,name:'Smoke '+suffix,description:'Disposable integration test',inactive:false};
  await request('POST',path,body);let rows=await request('GET',path);id=rows.find(r=>r.code===body.code)?.id;assert.ok(id,kind+' create/read failed');
  await request('PUT',path+'/'+id,{...body,name:body.name+' Updated'});rows=await request('GET',path);assert.equal(rows.find(r=>r.id===id).name,body.name+' Updated');
  results.push({name:kind,create:true,read:true,update:true});
 } finally { if(id)await request('DELETE',path+'/'+id); }
}
fs.writeFileSync(new URL('../artifacts/crud-smoke.json',import.meta.url),JSON.stringify(results,null,2));console.log('Create, read, update and delete passed for '+results.length+' HR and organization setups.');
