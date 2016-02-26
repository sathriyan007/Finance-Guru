using System;
using System.Linq;
using System.Web.Mvc;
using MVCGrid.Models;
using MVCGrid.Web;
using TrustMoi.Services.Interfaces;
using TrustMoi.ViewModels;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(TrustMoi.MvcGridConfig), "RegisterGrids")]

namespace TrustMoi
{
    public static class MvcGridConfig
    {
        public static void RegisterGrids()
        {
            MVCGridDefinitionTable.Add("ManageUsersGrid", new MVCGridBuilder<ManagePersonVm>()
                .WithAuthorizationType(AuthorizationType.Authorized)
                .WithPaging(true, 15, true, 50)
                .AddColumns(cols =>
                {
                    cols.Add("Key").WithValueExpression(p => p.Key).WithVisibility(false).WithFiltering(true);
                    cols.Add("FullName").WithHeaderText("Name").WithValueExpression(p => p.FullName).WithFiltering(true);
                    cols.Add("Address")
                        .WithHeaderText("Address")
                        .WithValueExpression(p => p.Address)
                        .WithFiltering(true);
                    cols.Add("Phone").WithHeaderText("Phone").WithValueExpression(p => p.Phone).WithFiltering(true);
                    cols.Add("PhoneConfirmed")
                        .WithHeaderText("Phone Confirmed")
                        .WithValueExpression(p => p.PhoneConfirmed ? "Yes" : "No")
                        .WithCellCssClassExpression(p => p.PhoneConfirmed ? "success" : "warning");
                    cols.Add("Email").WithHeaderText("Email").WithValueExpression(p => p.Email).WithFiltering(true);
                    cols.Add("EmailConfirmed")
                        .WithHeaderText("Email Confirmed")
                        .WithValueExpression(p => p.EmailConfirmed ? "Yes" : "No")
                        .WithCellCssClassExpression(p => p.EmailConfirmed ? "success" : "warning");
                    cols.Add("Gender").WithHeaderText("Gender").WithValueExpression(p => p.Gender);
                })
                .WithFiltering(true)
                .WithRetrieveDataMethod(context =>
                {
                    var service = DependencyResolver.Current.GetService<IUserService>();
                    var options = context.QueryOptions;
                    var result = new QueryResult<ManagePersonVm>();

                    if (!options.GetLimitOffset().HasValue) return result;

                    var query = service.GetFilteredUsers().AsQueryable();

                    var limitOffset = options.GetLimitOffset();
                    var limitRowcount = options.GetLimitRowcount();

                    if (!string.IsNullOrWhiteSpace(options.GetFilterString("Key")))
                        query =
                            query.Where(
                                p =>
                                    $"{p.FullName ?? string.Empty} {p.Address ?? string.Empty} {p.Email ?? string.Empty} {p.Phone ?? string.Empty}"
                                        .IndexOf(options.GetFilterString("Key").ToString(),
                                            StringComparison.InvariantCultureIgnoreCase) >= 0);

                    result.TotalRecords = query.Count();

                    if (limitOffset != null && limitRowcount != null)
                        query = query.Skip(limitOffset.Value).Take(limitRowcount.Value);

                    result.Items = query.ToList();

                    return result;
                }
                ));
        }
    }
}