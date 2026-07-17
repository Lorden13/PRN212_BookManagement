using System;

namespace BookManagement.WPF.Services.Transactions;

public sealed class BusinessRuleException : InvalidOperationException
{
    public BusinessRuleException(string message)
        : base(message)
    {
    }
}
