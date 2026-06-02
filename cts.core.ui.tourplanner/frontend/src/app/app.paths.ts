export const AppPaths = {
  home: '/',
  login: '/login',
  register: '/register',
  tour: (tourGuid: string) => `/tour/${tourGuid}`,
} as const;
