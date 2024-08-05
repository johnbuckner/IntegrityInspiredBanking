using FluentValidation;
using Models;
using Models.IRepositories;
using Models.IServices;
using Models.IValidators;
using Repositories;
using Services;

namespace Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddValidators(this IServiceCollection services)
    {
        services.AddScoped<ICreateValidator<Account>, AccountCreateValidator>();
        services.AddScoped<IUpdateValidator<Account>, AccountUpdateValidator>();
        services.AddScoped<IValidator<AccountBalanceUpdate>, AccountBalanceUpdateValidator>();

        return services;
    }

    public static IServiceCollection AddRepositoriesGroup(this IServiceCollection services)
    {
        services.AddScoped<IAccountRepository, AccountRepository>();

        return services;
    }

    public static IServiceCollection AddServicesGroup(this IServiceCollection services)
    {
        services.AddScoped<IAccountService, AccountService>();

        return services;
    }
}
