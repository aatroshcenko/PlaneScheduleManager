import { DeviceStatus } from "./device-status";

export interface DeviceHeartbeat {
  deviceId: string;
  timestamp: number;
  status: DeviceStatus;
}