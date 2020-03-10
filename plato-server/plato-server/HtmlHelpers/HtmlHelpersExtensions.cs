using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace plato.server.HtmlHelpers
{
    public static class HtmlHelpersExtensions
    {
        #region Private fields

        private static string style =
           @"<style>
            ul.platoFooter {
              list-style-type: none;
              margin: 0;
              padding: 0;
              overflow: hidden;
              background-color: #6c757d;
              position: fixed;
                    bottom: 0;
              width: 100%;
            }

                li.platoFooter {
              float: left;
            }

            li.platoFooter a.platoFooter
            {
                display: block;
                color: white;
                text-align: center;
                padding: 10px 16px;
                text-decoration: none;
            }

            li.platoFooter a:hover:not(.active).platoFooter {
              background-color: #111;
            }

            </style>";

        private static string menu =
            "<ul class=\"platoFooter\">" +
            "<li class=\"platoFooter\"><a class=\"platoFooter\" href=\"/Termo\"><img src=\"\\images\\thermSetting.png\" width=\"16\" height=\"16\"></a></li>" +
            "<li class=\"platoFooter\"><a class=\"platoFooter\" href=\"/SystemStat/SystemStat\"><img src=\"\\images\\system.png\" width=\"16\" height=\"16\"></a></li>" +
            "</ul>";

        #endregion

        public static IHtmlContent CreateSettingNavigationMenu(this IHtmlHelper html)
        {
            var footer = style + "" + menu;
            return new HtmlString(footer);
        }

        public static IHtmlContent CreateDashboardCard(this IHtmlHelper html, string iconMain, string iconExtra, string value, string refValue, string link, WarningLevel level)
        {
            var card =
                $" <div class=\"card {ConvertWarningLevel(level)}\" style=\"width: 15rem; \">                                 " +
                 " 	<div class=\"card-body\">                                                            " +
                 " 		<table>                                                                          " +
                 " 			<tr>                                                                         " +
                $" 				<td><img src=\"{iconMain}\" width=\"64\" height=\"64\"></td>             " +
                $" 				<td><div class=\"display-3\">{value}</div></td>                          " +
                 " 			</tr>                                                                        " +
                 " 			<tr>                                                                         " +
                $" 				<td><div class=\"display-4\">{refValue}</p></td>                                               " +
                $" 				<td><img src=\"{iconExtra}\" width =\"24\" height=\"24\"></td>           " +
                 " 			</tr>                                                                        " +
                 " 		</table>                                                                         " +
                $" 		<a href=\"{link}\" class=\"stretched-link\">Go</a>                               " +
                 " 	</div>                                                                               " +
                 " </div>                                                                                " ;
            return new HtmlString(card);
        }

       

        private static string ConvertWarningLevel(WarningLevel level)
        {
            switch(level)
            {
                case WarningLevel.bg_info:
                    return "bg-info";
                case WarningLevel.bg_light:
                    return "bg-light";
                case WarningLevel.bg_warning:
                    return "bg-warning";
                case WarningLevel.bg_danger:
                    return "bg-danger";
                default:
                    return "bg-info";
            }
                
        }
    }
}
