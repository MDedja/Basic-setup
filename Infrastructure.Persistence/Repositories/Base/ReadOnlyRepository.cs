using System.Linq.Expressions;
using AutoMapper;
using Entities.Base;
using Infrastructure.Persistence.Records.Base;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories.Base
{
    public abstract class ReadOnlyRepository<TReadModel, TRecord> : IReadOnlyRepository<TReadModel>
        where TReadModel : class, IReadModel where TRecord : class, IRecord, IMapFromReadModel<TReadModel>
    {
        protected DatabaseContext _context;
        protected readonly IMapper _mapper;

        /// <summary>
        /// Defines the base query for the given entity used by all operations.
        /// Concrete implementations should apply all necessary includes and pre-filters here.
        /// </summary>
        protected abstract IQueryable<TRecord> BaseQuery { get; }

        /// <summary>
        /// Defines the base query for the given entity used by browse operations.
        /// Concrete implementations should apply all necessary includes and pre-filters here.
        /// </summary>
        protected abstract IQueryable<TRecord> BaseBrowseQuery { get; }


        public ReadOnlyRepository(DatabaseContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public virtual async Task<TReadModel> GetByIdAsync(Guid id)
        {
            var record = await BaseQuery.SingleOrDefaultAsync(e => e.Id == id);
            return _mapper.Map<TReadModel>(record);
        }

        public virtual async Task<List<TReadModel>> GetAllAsync(bool readOnly = false)
        {
            var records = await (readOnly ? BaseQuery.AsNoTracking() : BaseQuery).ToListAsync();

            return _mapper.Map<List<TReadModel>>(records);
        }

        public async Task<TReadModel> GetSingleAsync(Expression<Func<TReadModel, bool>> filter, bool readOnly = false)
        {
            var expressionFilter = _mapper.Map<Expression<Func<TRecord, bool>>>(filter);

            var record = await (readOnly
                ? BaseQuery.AsNoTracking().FirstOrDefaultAsync(expressionFilter)
                : BaseQuery.FirstOrDefaultAsync(expressionFilter));

            return record == null ? null : _mapper.Map<TReadModel>(record);
        }

        public async Task<List<TReadModel>> GetFilteredAsync(Expression<Func<TReadModel, bool>> filter,
            bool readOnly = false)
        {
            var expressionFilter = _mapper.Map<Expression<Func<TRecord, bool>>>(filter);

            var records =
                await (readOnly ? BaseQuery.AsNoTracking().Where(expressionFilter) : BaseQuery.Where(expressionFilter))
                    .ToListAsync();

            return _mapper.Map<List<TReadModel>>(records);
        }

        public async Task<bool> ExistFilteredAsync(Expression<Func<TReadModel, bool>> filter)
        {
            var expressionFilter = _mapper.Map<Expression<Func<TRecord, bool>>>(filter);
        
            return await BaseQuery.AsNoTracking().Where(expressionFilter).AnyAsync();
        }

        public virtual bool Exists(Guid id)
        {
            return BaseQuery.Any(e => e.Id == id);
        }

        public bool Any(Expression<Func<TReadModel, bool>> filter = null)
        {
            if (filter == null)
            {
                return BaseQuery.AsNoTracking().Any();
            }

            var expressionFilter = _mapper.Map<Expression<Func<TRecord, bool>>>(filter);

            return BaseQuery.AsNoTracking().Any(expressionFilter);
        }

        //     public virtual async Task<PaginatedListReadModel<TReadModel>> BrowsePaginatedReadModel(PageQuery query)
        //     {
        //         var page = query.Page ?? 1;
        //         var limit = query.Limit ?? 20;
        //         var skip = (page - 1) * limit;
        //
        //         var recordQuery = BaseBrowseQuery
        //             .AsNoTracking()
        //             .QueryById(query.Id);
        //
        //         foreach (var predicate in query.Predicates)
        //         {
        //             var navPropertyParts = predicate.NavigationPropertyName.Split('.').ToList();
        //
        //             var parameter = Expression.Parameter(typeof(TRecord));
        //             var expression = BaseBrowseQueryableExtensions.GetNestedPropertyExpression(parameter, navPropertyParts, out var property);
        //             Expression operatorExpression;
        //             var propertyType = property.PropertyType;
        //
        //             switch (propertyType)
        //             {
        //                 case var x when x == typeof(int) || x == typeof(decimal):
        //                     var predicateValue = predicate.Value;
        //
        //                     if (!predicateValue.Contains("applied"))
        //                     {
        //                         var objectValue = BaseBrowseQueryableExtensions.GetObjectConvertedToNumber(predicateValue, propertyType);
        //                         operatorExpression = BaseBrowseQueryableExtensions.GetNumericExactExpression(expression, objectValue);
        //                     }
        //                     else
        //                     {
        //                         if (predicateValue.Contains("applied: []"))
        //                         {
        //                             return ReturnEmpty(page, limit);
        //                         }
        //                         var obj = JsonConvert.DeserializeObject<JsonUrlObject>(predicateValue);
        //
        //                         operatorExpression = obj.Op switch
        //                         {
        //                             "and" => BaseBrowseQueryableExtensions.GetEveryConstraintForNumericAndExpression(expression, obj, propertyType),
        //                             "or" => BaseBrowseQueryableExtensions.GetEveryConstraintForNumericOrExpression(expression, obj, propertyType),
        //                             _ => BaseBrowseQueryableExtensions.GetEveryConstraintForNumericAndExpression(expression, obj, propertyType)
        //                         };
        //                     }
        //
        //                     break;
        //                 case var x when x == typeof(Guid):
        //                     operatorExpression = BaseBrowseQueryableExtensions.GetGuidExpression(expression, predicate);
        //                     break;
        //                 case var x when x == typeof(Guid?):
        //                     operatorExpression = BaseBrowseQueryableExtensions.GetNullableGuidExpression(expression, predicate);
        //                     break;
        //                 case var x when (x == typeof(DateTime) || x == typeof(DateTime?)):
        //                     var value = predicate.Value;
        //
        //                     if (!value.Contains("applied"))
        //                     {
        //                         operatorExpression = BaseBrowseQueryableExtensions.GetDateExactExpression(expression, value);
        //                     }
        //                     else
        //                     {
        //                         if (value.Contains("applied: []"))
        //                         {
        //                             return ReturnEmpty(page, limit);
        //                         }
        //                         var urlValueDecoded = System.Web.HttpUtility.UrlDecode(value);
        //                         var obj = JsonConvert.DeserializeObject<JsonUrlObject>(urlValueDecoded, new JsonSerializerSettings()
        //                         {
        //                             DateParseHandling = DateParseHandling.None,
        //                             DateFormatHandling = DateFormatHandling.IsoDateFormat
        //                         });
        //
        //                         operatorExpression = obj.Op switch
        //                         {
        //                             "and" => BaseBrowseQueryableExtensions.GetEveryConstraintForDateAndExpression(expression, obj),
        //                             "or" => BaseBrowseQueryableExtensions.GetEveryConstraintForDateOrExpression(expression, obj),
        //                             _ => BaseBrowseQueryableExtensions.GetEveryConstraintForDateAndExpression(expression, obj)
        //                         };
        //                     }
        //
        //                     break;
        //                 case var x when x.IsEnum:
        //                     var enumPredicateValue = predicate.Value;
        //                     if (!enumPredicateValue.Contains("applied"))
        //                     {
        //                         operatorExpression = BaseBrowseQueryableExtensions.GetEnumAndExpression(expression, propertyType, predicate);
        //                     }
        //                     else
        //                     {
        //                         if (enumPredicateValue.Contains("applied: []"))
        //                         {
        //                             return ReturnEmpty(page, limit);
        //                         }
        //                         var obj = JsonConvert.DeserializeObject<JsonUrlObject>(enumPredicateValue);
        //
        //                         operatorExpression = obj.Op switch
        //                         {
        //                             "or" => BaseBrowseQueryableExtensions.GetEveryConstraintForEnumOrExpression(expression, obj),
        //                             _ => expression
        //                         };
        //                     }
        //
        //                     break;
        //                 case var x when Nullable.GetUnderlyingType(x) != null && Nullable.GetUnderlyingType(x)!.IsEnum:
        //                     var enumNullablePredicateValue = predicate.Value;
        //                     if (!enumNullablePredicateValue.Contains("applied"))
        //                     {
        //                         operatorExpression = BaseBrowseQueryableExtensions.GetEnumAndExpression(expression, Nullable.GetUnderlyingType(propertyType),
        //                             predicate);
        //                     }
        //                     else
        //                     {
        //                         if (enumNullablePredicateValue.Contains("applied: []"))
        //                         {
        //                             return ReturnEmpty(page, limit);
        //                         }
        //                         var obj = JsonConvert.DeserializeObject<JsonUrlObject>(enumNullablePredicateValue);
        //
        //                         operatorExpression = obj.Op switch
        //                         {
        //                             "or" => BaseBrowseQueryableExtensions.GetEveryConstraintForEnumOrExpression(expression, obj),
        //                             _ => expression
        //                         };
        //                     }
        //                     break;
        //                 case var x when x == typeof(Boolean):
        //                     operatorExpression = BaseBrowseQueryableExtensions.GetBoolExpression(expression, predicate);
        //                     break;
        //                 default:
        //                     var stringPredicateValue = predicate.Value;
        //
        //                     if (!stringPredicateValue.Contains("applied"))
        //                     {
        //                         operatorExpression = BaseBrowseQueryableExtensions.GetStringExpression(expression, predicate);
        //                     }
        //                     else
        //                     {
        //                         if (stringPredicateValue.Contains("applied: []"))
        //                         {
        //                             return ReturnEmpty(page, limit);
        //                         }
        //                         var obj = JsonConvert.DeserializeObject<JsonUrlObject>(stringPredicateValue);
        //
        //                         operatorExpression = obj.Op switch
        //                         {
        //                             "or" => BaseBrowseQueryableExtensions.GetEveryConstraintForStringOrExpression(expression, obj),
        //                             _ => expression
        //                         };
        //                     }
        //
        //                     break;
        //             }
        //
        //             if (operatorExpression == null) continue;
        //
        //             var lambda = Expression.Lambda<Func<TRecord, bool>>(operatorExpression, parameter);
        //
        //             recordQuery = recordQuery.Where(lambda);
        //         }
        //
        //         if (query.IsSorted)
        //         {
        //             var sortNavPropertyParts = query.NavigationSortProperty.Split(".").ToList();
        //             var parameter = Expression.Parameter(typeof(TRecord));
        //             var expression = BaseBrowseQueryableExtensions.GetNestedPropertyExpression(parameter, sortNavPropertyParts, out _);
        //             var objectExpression = Expression.Convert(expression, typeof(object));
        //             var lambda = Expression.Lambda<Func<TRecord, dynamic>>(objectExpression, parameter);
        //
        //             recordQuery = query.Order switch
        //             {
        //                 "ASC" => recordQuery.OrderBy(lambda),
        //                 "DESC" => recordQuery.OrderByDescending(lambda),
        //                 _ => recordQuery
        //             };
        //         }
        //
        //         var totalCount = recordQuery.Count();
        //
        //         var recordsPage = await recordQuery.Skip(skip)
        //             .Take(limit)
        //             .ToListAsync();
        //
        //         var readModels = _mapper.Map<List<TReadModel>>(recordsPage.ToList());
        //
        //         return new PaginatedListReadModel<TReadModel>(
        //             readModels,
        //             page,
        //             limit,
        //             totalCount);
        //     }
        //

        //
        //     private static PaginatedListReadModel<TReadModel> ReturnEmpty(int page, int limit)
        //     {
        //         return new PaginatedListReadModel<TReadModel>(
        //             new List<TReadModel>(),
        //             page,
        //             limit,
        //             0);
        //     }
        //     
        // }
    }
}