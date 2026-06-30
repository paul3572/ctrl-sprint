export interface TourLogDto {
  tourLogGuid: string;
  tourGuid: string;
  timestamp: string;
  comment: string;
  difficulty: number;
  totalDistanceInMeters: number;
  totalTimeMin: number;
  rating: number;
}
