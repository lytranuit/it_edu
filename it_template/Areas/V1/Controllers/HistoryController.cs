
using it_template.Areas.V1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System.Collections;
using Vue.Data;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace it_template.Areas.V1.Controllers
{

	public class HistoryController : BaseController
	{
		private readonly IConfiguration _configuration;
		private readonly EduContext _EduContext;
		public HistoryController(AuthContext context, EduContext EduContext, IConfiguration configuration) : base(context)
		{
			_configuration = configuration;
			_EduContext = EduContext;
		}

		[HttpPost]
		public async Task<JsonResult> Table()
		{
			var draw = Request.Form["draw"].FirstOrDefault();
			var start = Request.Form["start"].FirstOrDefault();
			var length = Request.Form["length"].FirstOrDefault();
			var searchValue = Request.Form["search[value]"].FirstOrDefault();
			var search_date_range = Request.Form["search_date"].FirstOrDefault();
			int pageSize = length != null ? Convert.ToInt32(length) : 0;
			int skip = start != null ? Convert.ToInt32(start) : 0;
			var customerData = _context.AuditTrailsModel.Where(d => 1 == 1);
			if (!string.IsNullOrEmpty(search_date_range))
			{
				var explode = search_date_range.Split(" - ");
				if (explode.Length > 1)
				{
					DateTime start_date = DateTime.ParseExact(explode[0].ToString(), "dd/MM/yyyy",
									   System.Globalization.CultureInfo.InvariantCulture);
					DateTime end_date = DateTime.ParseExact(explode[1].ToString(), "dd/MM/yyyy",
									   System.Globalization.CultureInfo.InvariantCulture);

					customerData = customerData.Where(m => m.DateTime.Date >= start_date.Date && m.DateTime.Date <= end_date.Date);
				}
			}
			int recordsTotal = customerData.Count();
			int recordsFiltered = customerData.Count();
			var datapost = customerData.Include(d => d.user).OrderByDescending(d => d.Id).Skip(skip).Take(pageSize).ToList();
			var data = new ArrayList();
			foreach (var record in datapost)
			{
				var user = record.user;
				var user_name = "";
				if (user != null)
				{
					user_name = user.FullName;

				}
				var data1 = new
				{
					id = record.Id,
					user = user_name,
					datetime = record.DateTime.ToString("yyyy/MM/dd HH:mm:ss"),
					type = record.Type,
					tableName = record.TableName,
					description = record.description,
					oldValues = $"<div style='white-space: pre-wrap;'>{record.OldValues}</div>",
					newValues = $"<div style='white-space: pre-wrap;'>{record.NewValues}</div>",
					primaryKey = record.PrimaryKey,
				};
				data.Add(data1);
			}
			var jsonData = new { draw = draw, recordsFiltered = recordsFiltered, recordsTotal = recordsTotal, data = data };
			return Json(jsonData);
		}


	}
}
