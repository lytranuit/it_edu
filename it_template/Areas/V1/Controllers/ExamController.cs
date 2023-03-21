
using elFinder.NetCore.Models;
using elFinder.NetCore;
using it_template.Areas.V1.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging.Signing;
using System;
using System.Collections;
using System.Net.Mail;
using System.Security.Policy;
using Vue.Data;
using Vue.Models;

namespace it_template.Areas.V1.Controllers
{

	public class ExamController : BaseController
	{
		private readonly IConfiguration _configuration;
		private readonly EduContext _EduContext;
		private readonly UserManager<UserModel> UserManager;
		public ExamController(AuthContext context, UserManager<UserModel> UserMgr, EduContext EduContext, IConfiguration configuration) : base(context)
		{
			_configuration = configuration;
			_EduContext = EduContext;
			UserManager = UserMgr;
		}
		[HttpPost]
		public async Task<JsonResult> Save(ExamModel ExamModel)
		{
			try
			{
				if (ExamModel.id > 0)
				{
					ExamModel.updated_at = DateTime.Now;
					_EduContext.Update(ExamModel);
				}
				else
				{
					var user_id = UserManager.GetUserId(this.User);
					ExamModel.created_at = DateTime.Now;
					_EduContext.Add(ExamModel);
				}

				_EduContext.SaveChanges();
				return Json(new { success = true, data = ExamModel });
			}
			catch (Exception ex)
			{
				return Json(new { success = false, message = ex.Message });
			}

		}

		[HttpPost]
		public async Task<JsonResult> Remove(List<int> item)
		{
			var jsonData = new { success = true, message = "" };
			try
			{
				var list = _EduContext.ExamModel.Where(d => item.Contains(d.id)).ToList();
				foreach (var course in list)
				{
					course.deleted_at = DateTime.Now;
				}
				_EduContext.UpdateRange(list);
				_EduContext.SaveChanges();
			}
			catch (Exception ex)
			{
				jsonData = new { success = false, message = ex.Message };
			}


			return Json(jsonData);
		}
		[HttpPost]
		public async Task<JsonResult> Table()
		{
			var draw = Request.Form["draw"].FirstOrDefault();
			var start = Request.Form["start"].FirstOrDefault();
			var length = Request.Form["length"].FirstOrDefault();
			int pageSize = length != null ? Convert.ToInt32(length) : 0;
			var title = Request.Form["filters[title]"].FirstOrDefault();
			int skip = start != null ? Convert.ToInt32(start) : 0;
			var customerData = _EduContext.ExamModel.Where(d => d.deleted_at == null);
			int recordsTotal = customerData.Count();


			if (title != null && title != "")
			{
				customerData = customerData.Where(d => d.title.Contains(title));
			}

			int recordsFiltered = customerData.Count();
			var datapost = customerData.OrderByDescending(d => d.id).Skip(skip).Take(pageSize).ToList();
			//var data = new ArrayList();
			//foreach (var record in datapost)
			//{
			//	var data1 = new
			//	{
			//		MaNSX = record.MaNSX,
			//		TenNSX = record.TenNSX,
			//		TenNSX_VN = record.TenNSX_VN
			//	};
			//	data.Add(data1);
			//}
			var jsonData = new { draw = draw, recordsFiltered = recordsFiltered, recordsTotal = recordsTotal, data = datapost };
			return Json(jsonData);
		}
		public async Task<JsonResult> Get(int id)
		{
			ExamModel ExamModel = _EduContext.ExamModel.Where(d => d.id == id)
				.FirstOrDefault();
			return Json(ExamModel);
		}
		public void CopyValues<T>(T target, T source)
		{
			Type t = typeof(T);

			var properties = t.GetProperties().Where(prop => prop.CanRead && prop.CanWrite);

			foreach (var prop in properties)
			{
				var value = prop.GetValue(source, null);
				//if (value != null)
				prop.SetValue(target, value, null);
			}
		}
	}
}
