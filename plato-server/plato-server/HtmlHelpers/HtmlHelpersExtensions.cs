using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace plato.server.HtmlHelpers
{
    public static class HtmlHelpersExtensions
    {
        public static IHtmlContent CreateSettingNavigationMenu(this IHtmlHelper html)
        {
            var footer = style + "" + menu;
            

            return new HtmlString(footer);
        }

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
    }
}
