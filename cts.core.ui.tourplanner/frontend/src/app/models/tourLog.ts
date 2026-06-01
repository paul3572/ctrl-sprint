export interface TourLog {
  tourLogGuid: string;
  tourGuid: string;
  dateTime: Date;
  comment: string;
  difficulty: number;
  totalDistance: number;
  totalTime: number;
  rating: number;
}
