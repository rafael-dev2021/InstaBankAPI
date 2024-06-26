﻿using BankingServiceAPI.Dto.Request;
using BankingServiceAPI.FluentValidations.Dto.Request;
using BankingServiceAPI.FluentValidations.Models;
using BankingServiceAPI.Models;
using FluentValidation;
using FluentValidation.AspNetCore;

namespace BankingServiceAPI.Extensions;

public static class FluentValidationDependencyInjection
{
    public static void AddFluentValidationDependencyInjection(this IServiceCollection service)
    {
        service.AddValidatorsFromAssemblyContaining<BankAccountValidator>();
        service.AddTransient<IValidator<BankAccount>, BankAccountValidator>();

        service.AddValidatorsFromAssemblyContaining<BankAccountDtoRequestValidator>();
        service.AddTransient<IValidator<BankAccountDtoRequest>, BankAccountDtoRequestValidator>();

        service.AddValidatorsFromAssemblyContaining<TransferDtoRequestByAccountValidator>();
        service.AddTransient<IValidator<TransferDtoRequestByAccount>, TransferDtoRequestByAccountValidator>();
        
        service.AddValidatorsFromAssemblyContaining<TransferDtoRequestByCpfValidator>();
        service.AddTransient<IValidator<TransferDtoRequestByCpf>, TransferDtoRequestByCpfValidator>();

        service.AddValidatorsFromAssemblyContaining<DepositDtoRequestValidator>();
        service.AddTransient<IValidator<DepositDtoRequest>, DepositDtoRequestValidator>();
        
        service.AddValidatorsFromAssemblyContaining<WithdrawDtoRequestValidator>();
        service.AddTransient<IValidator<WithdrawDtoRequest>, WithdrawDtoRequestValidator>();
        
        service.AddFluentValidationAutoValidation();
        service.AddFluentValidationClientsideAdapters();
    }
}