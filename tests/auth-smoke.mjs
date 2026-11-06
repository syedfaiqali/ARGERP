import fs from 'node:fs';import assert from 'node:assert/strict';import {randomUUID} from 'node:crypto';
const config=JSON.parse(fs.readFileSync(new URL('../.local/settings.json',import.meta.url),'utf8').replace(/^\uFEFF/,''));
const base='http://127.0.0.1:5181',username='smoke-auth-'+randomUUID(),password='Arg!'+randomUUID(),nextPassword='Next!'+randomUUID();let userId;
async function call(path,method='GET',body,token){const r=await fetch(base+path,{method,headers:{'Content-Type':'application/json',...(token?{Authorization:'Bearer '+token}:{})},body:body?JSON.stringify(body):undefined});const text=await r.text();return{status:r.status,data:text?JSON.parse(text):null};}
const admin=await call('/api/auth/login','POST',{username:'admin',password:config.adminPassword});assert.equal(admin.status,200);const token=admin.data.accessToken;
try{
 const created=await call('/api/arg/users','POST',{username,fullName:'Disposable auth integration test',password,entityId:1},token);assert.equal(created.status,200);userId=created.data.id;
 fs.writeFileSync(new URL('../artifacts/auth-test-user.json',import.meta.url),JSON.stringify({id:userId,username}));
 const login=await call('/api/auth/login','POST',{username,password});assert.equal(login.status,200);
 assert.equal((await call('/api/arg/users/options','GET',undefined,token)).status,200);
 const update={fullName:'Auth test updated',entityId:1,employeeId:null,blocked:true};
 assert.equal((await call('/api/arg/users/'+userId,'PUT',update,token)).status,204);
 assert.equal((await call('/api/auth/me','GET',undefined,login.data.accessToken)).status,401,'Blocking must revoke existing sessions');
 assert.equal((await call('/api/auth/login','POST',{username,password})).status,401);
 assert.equal((await call('/api/arg/users/'+userId,'PUT',{...update,blocked:false},token)).status,204);
 const resumed=await call('/api/auth/login','POST',{username,password});assert.equal(resumed.status,200);
 assert.equal((await call('/api/auth/change-password','POST',{currentPassword:password,newPassword:nextPassword},resumed.data.accessToken)).status,204);
 assert.equal((await call('/api/HumanResource/EmployeeProfile/v3','GET',undefined,resumed.data.accessToken)).status,401,'Password change must revoke sessions across services');
 assert.equal((await call('/api/auth/login','POST',{username,password})).status,401);
 assert.equal((await call('/api/auth/login','POST',{username,password:nextPassword})).status,200);
 console.log('PASS Auth: user creation/update, login, account blocking, password change and cross-service session revocation.');
}finally{if(userId)await call('/api/arg/users/'+userId,'PUT',{fullName:'Disposable auth integration test',entityId:1,employeeId:null,blocked:true},token);}
