import { TourLogDto } from './TourLogDto';

export interface TourDto {
  tourGuid: string;
  userGuid: string;
  name: string;
  description: string;
  from: string;
  to: string;
  transportName: string;
  tourDistanceInMeters: number;
  estimatedTimeMinutes: number;
  rating: number;
  tourLogs: TourLogDto[];
}
