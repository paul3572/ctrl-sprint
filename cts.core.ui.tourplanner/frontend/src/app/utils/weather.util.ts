export function getWeatherEmoji(description: string): string {
  const d = description.toLowerCase();

  if (d.includes('clear')) return '☀️';
  if (d.includes('partly')) return '⛅';
  if (d.includes('cloud')) return '☁️';
  if (d.includes('fog')) return '🌫️';
  if (d.includes('drizzle')) return '🌦️';
  if (d.includes('rain')) return '🌧️';
  if (d.includes('snow')) return '❄️';
  if (d.includes('thunder')) return '⛈️';

  return '❓';
}
