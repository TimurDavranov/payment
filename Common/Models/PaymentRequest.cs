using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Common.Enums;
using Common.Extentions;

namespace Common.Models;

public class PaymentRequest
{
    [LanguageCodeValidation]
    public string LanguageCode { get; init; }
    [MinLength(2)]
    public required string CurrencyCode { get; init; }
    [MinLength(1000)]
    public required decimal Amount { get; init; }
    [MinLength(3)]
    public required string UniqueId { get; init; }
    [Phone]
    public required string Phone { get; init; } 
    [Url]
    public required string ReturlUrl { get; init; }
    [Url]
    public required string CallbackUrl { get; init; }
    public EpsSystem System { get; init; }
    public required Guid ClientId { get; init; }
}