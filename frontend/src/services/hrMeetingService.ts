import {GetService} from '@/constants/globalItems';
export const getOnlineMeetingSetup=(provider:string)=>GetService('/api/HumanResource/Settings/v3/online-meeting/'+encodeURIComponent(provider));
