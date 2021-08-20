
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestCore.Helper
{
    public enum ButtonType
    {
        Cancel,
        General
    }
    public class UIHelper
    {
        public static IHtmlContent GetButton(IHtmlHelper helper, string text, string action, string controller, ButtonType type )
        {
            object htmlAttributes;

            switch (type)
            {
                case ButtonType.Cancel:
                    htmlAttributes = new { @class = "btn btn-danger" };
                    break;
                case ButtonType.General:
                    htmlAttributes = new { @class = "btn btn-warning" };
                    break;
                default:
                    htmlAttributes = new { @class = "btn btn-warning" };
                    break;
            }

            return helper.ActionLink(text, action, controller, null, htmlAttributes);
            
        }
    }
}
