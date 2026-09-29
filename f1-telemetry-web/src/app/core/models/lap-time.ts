export interface LapTime {
  id: string;
  sessionKey: number;
  driverNumber: number;
  lapNumber: number;
  lapDurationMs: number;
  tireCompound?: string;
  tireAge?: number;
}
