using Abp.Configuration;
using Abp.Dependency;
using Abp.UI;
using Eaf.Middleware.Configuration;
using Eaf.Middleware.Core.Editions;
using Eaf.Middleware.Payments.Dto;
using Stripe;
using Stripe.Checkout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Eaf.Middleware.Payments.Gateways
{
    /// <summary>
    /// Gateway de pagamento Stripe para assinaturas (one-shot e recorrente).
    /// </summary>
    public class StripePaymentGateway : IPaymentGateway, ISubscriptionPaymentGateway, ITransientDependency
    {
        private readonly ISettingManager _settingManager;

        /// <summary>
        /// StripePaymentGateway.
        /// </summary>
        /// <param name="settingManager">Gerenciador de configurações.</param>
        public StripePaymentGateway(ISettingManager settingManager)
        {
            _settingManager = settingManager;
        }

        /// <summary>
        /// Cria um PaymentIntent ou uma sessão de checkout recorrente no Stripe.
        /// </summary>
        public async Task<PaymentRequestDto> CreatePaymentAsync(CreatePaymentRequestInput input)
        {
            var secretKey = await GetSecretKeyAsync();
            var client = new StripeClient(secretKey);

            if (input.IsRecurring)
            {
                return await CreateCheckoutSessionAsync(client, input);
            }

            var service = new PaymentIntentService(client);

            var options = new PaymentIntentCreateOptions
            {
                Amount = (long)(input.Amount * 100),
                Currency = "brl",
                Description = input.Description,
                Metadata = new Dictionary<string, string>
                {
                    { "editionId", input.EditionId.ToString() },
                    { "paymentPeriodType", input.PaymentPeriodType.ToString() }
                }
            };

            var intent = await service.CreateAsync(options);

            return new PaymentRequestDto
            {
                PaymentId = intent.Id,
                GatewayPaymentId = null,
                Gateway = "Stripe",
                IsSuccess = !string.IsNullOrEmpty(intent.Id),
                CheckoutUrl = null
            };
        }

        /// <summary>
        /// Recupera o PaymentIntent/Session e verifica se foi confirmado.
        /// </summary>
        public async Task<PaymentResultDto> ProcessPaymentAsync(ProcessPaymentInput input)
        {
            var secretKey = await GetSecretKeyAsync();
            var client = new StripeClient(secretKey);

            if (input.ExternalPaymentId.StartsWith("pi_", StringComparison.OrdinalIgnoreCase))
            {
                var service = new PaymentIntentService(client);
                var intent = await service.GetAsync(input.ExternalPaymentId);

                return new PaymentResultDto
                {
                    ExternalPaymentId = input.ExternalPaymentId,
                    Gateway = "Stripe",
                    IsSuccess = intent.Status == "succeeded"
                };
            }

            if (input.ExternalPaymentId.StartsWith("cs_", StringComparison.OrdinalIgnoreCase))
            {
                var sessionService = new SessionService(client);
                var session = await sessionService.GetAsync(input.ExternalPaymentId);

                DateTime? subscriptionEndDate = null;
                if (!string.IsNullOrWhiteSpace(session.SubscriptionId))
                {
                    var subscriptionService = new SubscriptionService(client);
                    var subscription = await subscriptionService.GetAsync(session.SubscriptionId);
                    subscriptionEndDate = subscription.Items?.Data?.FirstOrDefault()?.CurrentPeriodEnd;
                }

                return new PaymentResultDto
                {
                    ExternalPaymentId = input.ExternalPaymentId,
                    GatewaySubscriptionId = session.SubscriptionId,
                    SubscriptionEndDate = subscriptionEndDate,
                    Gateway = "Stripe",
                    IsSuccess = session.PaymentStatus == "paid"
                };
            }

            return new PaymentResultDto
            {
                ExternalPaymentId = input.ExternalPaymentId,
                Gateway = "Stripe",
                IsSuccess = false
            };
        }

        /// <summary>
        /// Cancela uma assinatura recorrente no Stripe.
        /// </summary>
        public async Task<PaymentResultDto> CancelSubscriptionAsync(string gatewaySubscriptionId)
        {
            var secretKey = await GetSecretKeyAsync();
            var client = new StripeClient(secretKey);
            var service = new SubscriptionService(client);

            var subscription = await service.CancelAsync(gatewaySubscriptionId);

            return new PaymentResultDto
            {
                GatewaySubscriptionId = subscription.Id,
                Gateway = "Stripe",
                IsSuccess = subscription.Status == "canceled"
            };
        }

        /// <summary>
        /// Obtém o status de uma assinatura recorrente.
        /// </summary>
        public async Task<SubscriptionStatusResult> GetSubscriptionStatusAsync(string gatewaySubscriptionId)
        {
            var secretKey = await GetSecretKeyAsync();
            var client = new StripeClient(secretKey);
            var service = new SubscriptionService(client);

            var subscription = await service.GetAsync(gatewaySubscriptionId);

            return new SubscriptionStatusResult
            {
                IsActive = subscription.Status == "active",
                Status = subscription.Status,
                CurrentPeriodEnd = subscription.Items?.Data?.FirstOrDefault()?.CurrentPeriodEnd
            };
        }

        /// <summary>
        /// Processa um webhook do Stripe.
        /// </summary>
        public async Task<PaymentResultDto> ProcessWebhookAsync(string eventName, string json, string signature)
        {
            var webhookSecret = await _settingManager.GetSettingValueForApplicationAsync(AppSettings.Payment.Stripe.WebhookSecret);
            var stripeEvent = string.IsNullOrWhiteSpace(webhookSecret)
                ? EventUtility.ParseEvent(json)
                : EventUtility.ConstructEvent(json, signature, webhookSecret, 300, false);

            return stripeEvent.Type switch
            {
                "invoice.paid" => await HandleInvoicePaidAsync(stripeEvent),
                "invoice.payment_failed" => await HandleInvoiceFailedAsync(stripeEvent),
                "checkout.session.completed" => await HandleCheckoutSessionCompletedAsync(stripeEvent),
                _ => new PaymentResultDto { Gateway = "Stripe", IsSuccess = false }
            };
        }

        private async Task<PaymentRequestDto> CreateCheckoutSessionAsync(StripeClient client, CreatePaymentRequestInput input)
        {
            var service = new SessionService(client);

            var lineItems = new List<SessionLineItemOptions>();
            if (input.Products != null && input.Products.Any())
            {
                lineItems.AddRange(input.Products.Select(p => new SessionLineItemOptions
                {
                    Quantity = p.Count,
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(p.Amount * 100),
                        Currency = "brl",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = p.Description ?? "Subscription"
                        },
                        Recurring = MapRecurring(input.PaymentPeriodType)
                    }
                }));
            }
            else
            {
                lineItems.Add(new SessionLineItemOptions
                {
                    Quantity = 1,
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(input.Amount * 100),
                        Currency = "brl",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = input.Description ?? "Subscription"
                        },
                        Recurring = MapRecurring(input.PaymentPeriodType)
                    }
                });
            }

            var options = new SessionCreateOptions
            {
                Mode = "subscription",
                CustomerCreation = "always",
                LineItems = lineItems,
                SuccessUrl = GetSuccessUrl(input.SuccessUrl),
                CancelUrl = input.ErrorUrl ?? "https://example.com/payment/error",
                Metadata = new Dictionary<string, string>
                {
                    { "editionId", input.EditionId.ToString() },
                    { "paymentPeriodType", input.PaymentPeriodType.ToString() }
                }
            };

            var session = await service.CreateAsync(options);

            return new PaymentRequestDto
            {
                PaymentId = session.Id,
                GatewayPaymentId = session.SubscriptionId,
                Gateway = "Stripe",
                IsSuccess = !string.IsNullOrEmpty(session.Id),
                CheckoutUrl = session.Url
            };
        }

        private async Task<PaymentResultDto> HandleInvoicePaidAsync(Event stripeEvent)
        {
            var invoice = stripeEvent.Data.Object as Invoice;
            var subscriptionId = invoice?.Parent?.SubscriptionDetails?.SubscriptionId;
            var paymentIntentId = invoice?.Payments?.Data?.FirstOrDefault()?.Payment?.PaymentIntentId;

            if (string.IsNullOrWhiteSpace(subscriptionId))
            {
                return new PaymentResultDto { Gateway = "Stripe", IsSuccess = false };
            }

            var secretKey = await GetSecretKeyAsync();
            var client = new StripeClient(secretKey);
            var service = new SubscriptionService(client);
            var subscription = await service.GetAsync(subscriptionId);

            return new PaymentResultDto
            {
                GatewaySubscriptionId = subscription.Id,
                InvoiceNo = invoice?.Number,
                ExternalPaymentId = paymentIntentId,
                SubscriptionEndDate = invoice?.PeriodEnd,
                Gateway = "Stripe",
                IsSuccess = true
            };
        }

        private Task<PaymentResultDto> HandleInvoiceFailedAsync(Event stripeEvent)
        {
            var invoice = stripeEvent.Data.Object as Invoice;
            return Task.FromResult(new PaymentResultDto
            {
                GatewaySubscriptionId = invoice?.Parent?.SubscriptionDetails?.SubscriptionId,
                ExternalPaymentId = invoice?.Payments?.Data?.FirstOrDefault()?.Payment?.PaymentIntentId,
                Gateway = "Stripe",
                IsSuccess = false
            });
        }

        private async Task<PaymentResultDto> HandleCheckoutSessionCompletedAsync(Event stripeEvent)
        {
            var session = stripeEvent.Data.Object as Session;
            if (session?.SubscriptionId == null)
            {
                return new PaymentResultDto { Gateway = "Stripe", IsSuccess = false };
            }

            var secretKey = await GetSecretKeyAsync();
            var client = new StripeClient(secretKey);
            var service = new SubscriptionService(client);
            var subscription = await service.GetAsync(session.SubscriptionId);

            return new PaymentResultDto
            {
                GatewaySubscriptionId = subscription.Id,
                ExternalPaymentId = session.PaymentIntentId,
                SubscriptionEndDate = subscription.Items?.Data?.FirstOrDefault()?.CurrentPeriodEnd,
                Gateway = "Stripe",
                IsSuccess = session.PaymentStatus == "paid"
            };
        }

        private static string GetSuccessUrl(string inputSuccessUrl)
        {
            if (string.IsNullOrWhiteSpace(inputSuccessUrl))
            {
                return "https://example.com/payment/success?session_id={CHECKOUT_SESSION_ID}";
            }

            return inputSuccessUrl.Contains("{CHECKOUT_SESSION_ID}", StringComparison.OrdinalIgnoreCase)
                ? inputSuccessUrl
                : $"{inputSuccessUrl.TrimEnd('/', '?')}?session_id={{CHECKOUT_SESSION_ID}}";
        }

        private SessionLineItemPriceDataRecurringOptions MapRecurring(PaymentPeriodType period)
        {
            return period switch
            {
                PaymentPeriodType.Daily => new SessionLineItemPriceDataRecurringOptions { Interval = "day", IntervalCount = 1 },
                PaymentPeriodType.Weekly => new SessionLineItemPriceDataRecurringOptions { Interval = "week", IntervalCount = 1 },
                PaymentPeriodType.Monthly => new SessionLineItemPriceDataRecurringOptions { Interval = "month", IntervalCount = 1 },
                PaymentPeriodType.Quarterly => new SessionLineItemPriceDataRecurringOptions { Interval = "month", IntervalCount = 3 },
                PaymentPeriodType.Biannual => new SessionLineItemPriceDataRecurringOptions { Interval = "month", IntervalCount = 6 },
                PaymentPeriodType.Annual => new SessionLineItemPriceDataRecurringOptions { Interval = "year", IntervalCount = 1 },
                _ => new SessionLineItemPriceDataRecurringOptions { Interval = "month", IntervalCount = 1 }
            };
        }

        private async Task<string> GetSecretKeyAsync()
        {
            var secretKey = await _settingManager.GetSettingValueForApplicationAsync(AppSettings.Payment.Stripe.SecretKey);
            if (string.IsNullOrWhiteSpace(secretKey))
            {
                throw new UserFriendlyException("Stripe secret key is not configured.");
            }

            return secretKey;
        }
    }
}
