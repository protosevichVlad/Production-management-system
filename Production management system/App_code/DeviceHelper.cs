using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProductionManagementSystem.Models;

namespace ProductionManagementSystem
{
    public static class DeviceHelper
    {
        public static HtmlString CreateDevice(this IHtmlHelper html, Device device, bool print, int? quantity)
        {
            string result = "<dl class='row'><div class='col-sm-10'>";
            if (print)
            {
                result +=
                    @"<button class='btn btn-link float-right' type='button' onclick='window.print()'>Распечатать(Сохранить)</button>";
            }

            result += @"</div><dt class = 'col-sm-4'>Наименовие</dt><dd class = 'col-sm-10'>" + device.Name + @"</dd>
                      <dt class = 'col-sm-4'>Количество на складе:</dt><dd class = 'col-sm-10'>" + device.Quantity + "</dd>";
            if (!(quantity is null))
            {
                result += "<dt class = 'col-sm-4'>Количество в задаче</dt><dd class = 'col-sm-10'>" + quantity + "</dd>";
            }
            result += @"</dl><div class='row'><div class='col'><h3>Монтаж</h3></div></div><div class='row'>
            <div class='col' id='Components'><table class='table table-sm' style='width:100%;'><tr><td>Наименовие</td>
            <td>Количество в приборе</td>";
            if (!(quantity is null))
            {
                result += "<td>Количество в задаче</td>";
            }
            result += "<td>Количество на складе</td><td width='40%'>Примечание</td></tr>";
            foreach (var comp in device.DeviceComponentsTemplate)
            {
                result += "<tr ";
                if (comp.Component.Quantity < comp.Quantity)
                { 
                    result += "class='table-danger'";
                };
                result += $"><td>{comp.Component.Name}</td>";
                result += $"<td>{comp.Quantity}</td>";
                if (!(quantity is null))
                {
                    result += $"<td>{comp.Quantity * quantity}</td>";
                }
                result += $"<td>{comp.Component.Quantity}</td>";
                result += $"<td>{comp.Description}</td></tr>";
            }

            result += @"</table></div></div><br /><div class='row'><div class='col'><h3>Конструктив</h3>
            </div></div><div class='row'><div class='col' id='Designs'><table  class='table table-sm' style='width:100%;'><tr><td>Наименовие</td>
            <td>Количество в приборе</td>";
            if (!(quantity is null))
            {
                result += "<td>Количество в задаче</td>";
            }
            result += "<td>Количество на складе</td><td width='40%'>Примечание</td></tr>";
            foreach (var des in device.DeviceDesignTemplate)
            {
                result += "<tr "; 
                if (des.Design.Quantity < des.Quantity)
                {
                    result += "class='table-danger'";
                }

                result += $"><td>{des.Design.Name}</td>";
                result += $"<td>{des.Quantity}</td>";
                if (!(quantity is null))
                {
                    result += $"<td>{des.Quantity * quantity}</td>";
                }
                result += $"<td>{des.Design.Quantity}</td>";
                result += $"<td>{des.Description}</td></tr>";
            }
            result += "</table></div></div>";
            
            return new HtmlString(result);
        }
    }
}