using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.UI;
using System.Web.Mvc.Html;
using System.Web;
using System.IO;

namespace System.Web.Mvc.Html
{
    public enum RepeatDirections
    {
        // 摘要: 
        //     清單的項目會由左而右，然後由上而下，以水平成列顯示，直到所有項目皆呈現。
        Horizontal = 0,
        //
        // 摘要: 
        //     清單的項目會由上而下，然後由左而右，以垂直成行顯示，直到所有項目皆呈現。
        Vertical = 1,
    }

    public static class RadioButtonListHelper
    {
        #region Private Method
        private static string BuildColumnNameFromMemberExpression(MemberExpression memberExpr)
        {
            var sb = new StringBuilder();
            Expression expr = memberExpr;
            while (true)
            {
                string piece = GetExpressionMemberName(expr, ref expr);
                if (string.IsNullOrEmpty(piece)) break;
                if (sb.Length > 0)
                    sb.Insert(0, ".");
                sb.Insert(0, piece);
            }
            return sb.ToString();
        }

        private static string BuildColumnNameFromModel<TModel, TProperty>(Expression<Func<TModel, TProperty>> expression)
        {
            var expr = expression.Body as MemberExpression;
            if (expr == null)
                throw new ArgumentException(
                    string.Format("Expression '{0}' must be a member expression", expression),
                    "expression");
            return BuildColumnNameFromMemberExpression(expr);
        }

        private static string GetExpressionMemberName(Expression expr, ref Expression nextExpr)
        {
            if (expr is MemberExpression)
            {
                var memberExpr = (MemberExpression)expr;
                nextExpr = memberExpr.Expression;
                return memberExpr.Member.Name;
            }
            if (expr is BinaryExpression && expr.NodeType == ExpressionType.ArrayIndex)
            {
                var binaryExpr = (BinaryExpression)expr;
                string memberName = GetExpressionMemberName(binaryExpr.Left, ref nextExpr);
                if (string.IsNullOrEmpty(memberName))
                    throw new InvalidDataException("Cannot parse your column expression");
                return string.Format("{0}[{1}]", memberName, binaryExpr.Right);
            }
            return string.Empty;
        }
        private static string Eval(object container, string expression)
        {
            object value = container;
            if (!String.IsNullOrEmpty(expression))
            {
                value = DataBinder.Eval(container, expression);
            }
            return Convert.ToString(value, CultureInfo.CurrentCulture);
        }
        #endregion

        /// <summary>
        /// RadioButtonList for 資料集
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="helper"></param>
        /// <param name="items">資料集</param>
        /// <param name="dataValueField">值欄位</param>
        /// <param name="dataTextField">顯示欄位</param>
        /// <param name="expression"></param>
        /// <param name="htmlAttributes"></param>
        /// <param name="appendOptionLabel">是否增加預設選項 例:請選擇</param>
        /// <param name="optionLabel">預設選項文字</param>
        /// <param name="direction">顯示方向:垂直、水平</param>
        /// <returns></returns>
        public static MvcHtmlString RadioButtonListForList<T, TProperty>(this HtmlHelper<T> helper
            , IEnumerable items, string dataValueField, string dataTextField, Expression<Func<T, TProperty>> expression
            , object htmlAttributes = null, bool appendOptionLabel = false, string optionLabel = null
            , RepeatDirections direction = RepeatDirections.Horizontal)
        {
            string name = BuildColumnNameFromModel(expression);
            var optionData = from object o in items
                             select new KeyValuePair<string, string>(Eval(o, dataTextField), Eval(o, dataValueField));
            string defaultSelectValue = helper.ViewData.Eval(name).ToString();
            return BuildRadioButtonList(helper, expression, name, defaultSelectValue, optionData, htmlAttributes, appendOptionLabel, optionLabel, direction);
        }

        /// <summary>
        /// RadioButtonList for 列舉
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="helper"></param>
        /// <param name="expression"></param>
        /// <param name="htmlAttributes"></param>
        /// <param name="appendOptionLabel">是否增加預設選項 例:請選擇</param>
        /// <param name="optionLabel">預設選項文字</param>
        /// <param name="direction">顯示方向:垂直、水平</param>
        /// <returns></returns>
        public static MvcHtmlString RadioButtonListForEnum<T, TProperty>(this HtmlHelper<T> helper
   , Expression<Func<T, TProperty>> expression
, object htmlAttributes = null, bool appendOptionLabel = false, string optionLabel = null
 , RepeatDirections direction = RepeatDirections.Horizontal)
        {
            string name = BuildColumnNameFromModel(expression);
            Enum defaultSelectValue = helper.ViewData.Eval(name) as Enum;
            IList<SelectListItem> selectList = EnumHelper.GetSelectList(typeof(TProperty), defaultSelectValue);
            List<KeyValuePair<string, string>> optionData = new List<KeyValuePair<string, string>>();
            foreach (var item in selectList)
            {
                optionData.Add(new KeyValuePair<string, string>(item.Text, item.Value));
            }
            return BuildRadioButtonList(helper, expression, name, Convert.ToInt32(defaultSelectValue).ToString(), optionData, htmlAttributes, appendOptionLabel, optionLabel, direction);
        }




        private static MvcHtmlString BuildRadioButtonList<T, TProperty>(HtmlHelper<T> helper, Expression<Func<T, TProperty>> expression, string name, string defaultSelectValue, IEnumerable<KeyValuePair<string, string>> optionData
, object htmlAttributes, bool appendOptionLabel, string optionLabel, RepeatDirections direction = RepeatDirections.Horizontal)
        {

            StringBuilder renderHtmlTag = new StringBuilder();
            IDictionary<string, string> newOptionData = new Dictionary<string, string>();
            if (appendOptionLabel)
                newOptionData.Add(new KeyValuePair<string, string>(optionLabel ?? "請選擇", ""));

            foreach (var item in optionData)
                newOptionData.Add(item);
            int i = 0;
            foreach (var option in newOptionData)
            {
                string id = string.Format("{0}_{1}", name.Replace(".", "_"), i++);

                TagBuilder optionTag = new TagBuilder("input");
                optionTag.Attributes.Add("type", "radio");
                optionTag.Attributes.Add("name", name);
                optionTag.Attributes.Add("id", id);
                optionTag.Attributes.Add("value", option.Value);
                if (option.Value == defaultSelectValue)
                    optionTag.Attributes.Add("checked", "checked");
                RouteValueDictionary attribute = null;
                if (htmlAttributes != null)
                    attribute = new RouteValueDictionary(htmlAttributes);
                else
                    attribute = new RouteValueDictionary();
                optionTag.MergeAttributes(attribute);
                ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);
                var unobtrusive = helper.GetUnobtrusiveValidationAttributes(name, metadata);
                optionTag.MergeAttributes(unobtrusive);

                var radio = optionTag.ToString();
                renderHtmlTag.AppendFormat("<div class='{2}'><label>{0}{1}</label></div>", radio, option.Key, (direction == RepeatDirections.Horizontal ? "radio-inline" : "radio"));
            }
            return new MvcHtmlString(renderHtmlTag.ToString());
        }



    }
}