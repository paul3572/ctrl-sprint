import { Transport } from './transport';
import { TourLog } from './tourLog';
import { RouteGeometry } from '../contracts/RouteGeometry';

export interface Tour {
  tourGuid: string;
  userGuid: string;
  name: string;
  description: string;
  from: string;
  to: string;
  transportType: Transport;
  tourDistance: number;
  estimatedTimeMinutes: number;
  rating: number;
  tourLogs: TourLog[];
  routeGeometry: RouteGeometry;
}
