import { Transport } from './transport';
import { TourLog } from './tourLog';

export interface Tour {
  tourGuid: string;
  name: string;
  description: string;
  from: string;
  to: string;
  transportType: Transport;
  tourDistance: number;
  estimatedTimeMinutes: number;
  rating: number;
  tourLogs: TourLog[];
}
