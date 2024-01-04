using System.Reflection;
using Application.Commons.DataAccess;
using Application.Commons.DataAccess.ReadOnly;
using AutoMapper.Extensions.ExpressionMapping;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Infrastructure.Persistence.Repositories;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddAutoMapper(cfg => { cfg.AddExpressionMapping(); }, Assembly.GetExecutingAssembly());
    
       
        // services.TryAddTransient<ICategoryReadOnlyRepository, CategoryReadOnlyRepository>();
        // services.TryAddTransient<ICategoryRepository, CategoryRepository>();
        //
        // services.TryAddTransient<IUnitOfWork, UnitOfWork>();
        // services.TryAddScoped<IUnitOfWork, UnitOfWork>();
        services.AddTransient<ICategoryReadOnlyRepository, CategoryReadOnlyRepository>();
        services.AddTransient<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}