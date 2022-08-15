export const environment = {
  production: true,
  clientId: window['env']['clientId'] || 'manager',
  hubUrl: window['env']['hubUrl'] || 'http://planeschedulemanager:80/devicesHub'
};
