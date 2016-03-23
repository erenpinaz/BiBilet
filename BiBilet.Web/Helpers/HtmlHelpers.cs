using System;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace BiBilet.Web.Helpers
{
    public static class HtmlHelpers
    {
        /// <summary>
        /// Gets the display placeholder for model
        /// </summary>
        public static MvcHtmlString DisplayPlaceholderFor<TModel, TValue>(this HtmlHelper<TModel> html,
            Expression<Func<TModel, TValue>> expression)
        {
            var result = html.DisplayNameFor(expression).ToHtmlString();
            return MvcHtmlString.Create(HttpUtility.HtmlDecode(result));
        }
    }
}