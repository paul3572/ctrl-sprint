export interface TourLog {
  tourLogGuid: string;
  tourGuid: string;
  timestamp: Date;
  comment: string;
  difficulty: number;
  totalDistanceInMeters: number;
  totalTimeMin: number;
  rating: number;
}
