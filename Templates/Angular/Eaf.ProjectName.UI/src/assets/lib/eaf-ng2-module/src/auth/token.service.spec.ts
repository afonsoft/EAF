import { TestBed } from '@angular/core/testing';
import { StorageService } from '@eaf/utils/storage.service';
import { TokenService, TokenPayload } from './token.service';
import { setupEafGlobals } from '../../../../../test-helpers/mock-services';

function encodeBase64(value: string): string {
  return btoa(value).replace(/=/g, '');
}

function buildToken(payload: TokenPayload): string {
  const header = encodeBase64(JSON.stringify({ alg: 'none' }));
  const body = encodeBase64(
    JSON.stringify(payload).replace(/[\u007F-\uFFFF]/g, char => `\\u${('0000' + char.charCodeAt(0).toString(16)).slice(-4)}`),
  );
  return `${header}.${body}.signature`;
}

describe('TokenService', () => {
  let service: TokenService;
  let storageService: jasmine.SpyObj<StorageService>;

  beforeEach(() => {
    setupEafGlobals();
    (window as any).eaf.auth.clearToken = () => {};

    storageService = jasmine.createSpyObj('StorageService', ['getCookieValue', 'setCookieValue', 'deleteCookie']);

    TestBed.configureTestingModule({
      providers: [{ provide: StorageService, useValue: storageService }],
    });

    service = TestBed.inject(TokenService);
  });

  it('deve decodificar payload de token JWT', () => {
    const payload: TokenPayload = { sub: '42', unique_name: 'test', tenantid: '7' };
    storageService.getCookieValue.and.returnValue(buildToken(payload));

    expect(service.getPayload()).toEqual(jasmine.objectContaining({
      sub: '42',
      unique_name: 'test',
      tenantid: '7',
    }));
  });

  it('deve retornar null quando token é inválido', () => {
    storageService.getCookieValue.and.returnValue('invalid-token');

    expect(service.getPayload()).toBeNull();
  });

  it('deve retornar userId quando sub é um número', () => {
    const payload: TokenPayload = { sub: '42' };
    storageService.getCookieValue.and.returnValue(buildToken(payload));

    expect(service.getUserId()).toBe(42);
  });

  it('deve retornar tenantId quando claim tenantid existe', () => {
    const payload: TokenPayload = { tenantid: '7' };
    storageService.getCookieValue.and.returnValue(buildToken(payload));

    expect(service.getTenantId()).toBe(7);
  });

  it('deve retornar userName de unique_name', () => {
    const payload: TokenPayload = { unique_name: 'john.doe' };
    storageService.getCookieValue.and.returnValue(buildToken(payload));

    expect(service.getUserName()).toBe('john.doe');
  });

  it('deve retornar roles como array mesmo quando for string', () => {
    const payload: TokenPayload = { role: 'Admin' };
    storageService.getCookieValue.and.returnValue(buildToken(payload));

    expect(service.getRoles()).toEqual(['Admin']);
  });

  it('deve retornar roles quando forem múltiplas', () => {
    const payload: TokenPayload = { role: ['Admin', 'User'] };
    storageService.getCookieValue.and.returnValue(buildToken(payload));

    expect(service.getRoles()).toEqual(['Admin', 'User']);
  });

  it('isInRole deve ser case-insensitive', () => {
    const payload: TokenPayload = { role: ['Admin'] };
    storageService.getCookieValue.and.returnValue(buildToken(payload));

    expect(service.isInRole('admin')).toBeTrue();
    expect(service.isInRole('guest')).toBeFalse();
  });

  it('isValid deve retornar true quando token não expirou', () => {
    const payload: TokenPayload = { exp: Math.floor(Date.now() / 1000) + 3600 };
    storageService.getCookieValue.and.returnValue(buildToken(payload));

    expect(service.isValid()).toBeTrue();
  });

  it('isValid deve retornar false quando token expirou', () => {
    const payload: TokenPayload = { exp: Math.floor(Date.now() / 1000) - 1 };
    storageService.getCookieValue.and.returnValue(buildToken(payload));

    expect(service.isValid()).toBeFalse();
  });
});
