import { GetService } from '@/constants/globalItems';
import { workflowMutation } from '@/workflow/api';
const documents='/api/HumanResource/HRDocumentType/v3';
const attachments='/api/HumanResource/Settings/v3/attachments';
export const humanResourceAdministrationService = {
 createDocumentType: (payload:any) => workflowMutation('post',documents,payload),
 updateDocumentType: (id:number|string,payload:any) => workflowMutation('put',documents+'/'+id,payload),
 deleteDocumentType: (id:number|string) => workflowMutation('delete',documents+'/'+id),
 getSystemPolicyAttachmentPolicies: async ():Promise<any[]> => { const r:any=await GetService(attachments); return r?.data?.data??r?.data??[] },
 getSystemPolicyAttachmentLookups: async ():Promise<any> => { const r:any=await GetService(attachments+'/lookups'); return r?.data?.data??r?.data??{} },
};
