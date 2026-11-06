import { GetService, PostService } from "@/constants/globalItems";

const BASE = "/api/Communication/OnlineMeetingSetup/v3";

/** Provider keys as stored in the type column. */
export type MeetingProvider = "googlemeet" | "zoom" | "jitsi" | "myraabta";

export type OnlineMeetingSetup = {
  type: MeetingProvider;
  entityId: number;
  /** Provider settings. The shape is decided by each provider's screen. */
  configuration: Record<string, any>;
};

export const getOnlineMeetingSetups = async () => GetService(BASE);

export const getOnlineMeetingSetup = async (type: MeetingProvider) =>
  GetService(`${BASE}/${type}`);

export const saveOnlineMeetingSetup = async (
  type: MeetingProvider,
  configuration: Record<string, any>,
) => PostService(BASE, { type, configuration });

/** Checks the saved MyRaabta API key and private key without creating a room. */
export const testMyRaabtaConnection = async () =>
  PostService(`${BASE}/myraabta/test-connection`, {});
