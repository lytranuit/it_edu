
using it_template.Areas.V1.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using Vue.Data;
using Vue.Models;

namespace it_template.Areas.V1.Controllers
{

	public class GroupController : BaseController
	{
		private readonly IConfiguration _configuration;
		private readonly EduContext _EduContext;
		private readonly UserManager<UserModel> UserManager;
		public GroupController(AuthContext context, UserManager<UserModel> UserMgr, EduContext EduContext, IConfiguration configuration) : base(context)
		{
			_configuration = configuration;
			_EduContext = EduContext;
			UserManager = UserMgr;
		}
		[HttpPost]
		public async Task<JsonResult> Save(GroupModel GroupModel, int? old_key)
		{
			var jsonData = new { success = true, message = "" };
			try
			{
				if (old_key == null)
				{
					_EduContext.Add(GroupModel);
					_EduContext.SaveChanges();
				}
				else
				{
					var GroupModel_old = _EduContext.GroupModel.Where(d => d.id == old_key).FirstOrDefault();
					CopyValues<GroupModel>(GroupModel_old, GroupModel);
					_EduContext.Update(GroupModel_old);
					_EduContext.SaveChanges();
				}
			}
			catch (Exception ex)
			{
				jsonData = new { success = false, message = ex.Message };
			}


			return Json(jsonData);
		}

		[HttpPost]
		public async Task<JsonResult> Remove(List<int> item)
		{
			var jsonData = new { success = true, message = "" };
			try
			{
				var list = _EduContext.GroupModel.Where(d => item.Contains(d.id)).ToList();
				_EduContext.RemoveRange(list);
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
			var name = Request.Form["filters[name]"].FirstOrDefault();
			var id_string = Request.Form["filters[id]"].FirstOrDefault();
			var id = id_string != null ? Convert.ToInt32(id_string) : 0;
			int skip = start != null ? Convert.ToInt32(start) : 0;
			var customerData = _EduContext.GroupModel.Where(d => d.deleted_at == null);
			int recordsTotal = customerData.Count();
			if (name != null && name != "")
			{
				customerData = customerData.Where(d => d.name.Contains(name));
			}
			if (id > 0)
			{
				customerData = customerData.Where(d => d.id == id);
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
