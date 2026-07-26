import { normalizeEafError } from './eaf-contracts';

describe('EAF consumer contracts', () => {
  it('normaliza um erro público preservando correlation ID', () => {
    const result = normalizeEafError({ code: 'rate_limited', message: 'Aguarde.', retryable: true }, 429, 'corr-1');

    expect(result).toEqual({
      code: 'rate_limited',
      message: 'Aguarde.',
      retryable: true,
      correlationId: 'corr-1',
    });
  });

  it('não marca validação como retryable', () => {
    const result = normalizeEafError({ code: 'validation_failed', message: 'Inválido.', retryable: false }, 400, 'corr-2');

    expect(result.retryable).toBeFalse();
  });

  it('usa códigos públicos estáveis para autenticação', () => {
    expect(normalizeEafError(undefined, 401).code).toBe('not_authenticated');
    expect(normalizeEafError(undefined, 403).code).toBe('not_authorized');
  });

  it('marca falhas transitórias como retryable', () => {
    expect(normalizeEafError(undefined, 503).retryable).toBeTrue();
    expect(normalizeEafError(undefined, 404).retryable).toBeFalse();
  });
});
