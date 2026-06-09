import { provideKeycloak } from 'keycloak-angular';

export const provideKeycloakAngular = () =>
  provideKeycloak({
    config: {
      url: 'http://localhost:8080',
      realm: 'bytes-rewards',
      clientId: 'bytes-rewards-ui'
    },

    initOptions: {
      onLoad: 'login-required',
      checkLoginIframe: false
    }
  });