
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

	public class CourseController : BaseController
	{
		private readonly IConfiguration _configuration;
		private readonly EduContext _EduContext;
		private readonly UserManager<UserModel> UserManager;
		public CourseController(AuthContext context, UserManager<UserModel> UserMgr, EduContext EduContext, IConfiguration configuration) : base(context)
		{
			_configuration = configuration;
			_EduContext = EduContext;
			UserManager = UserMgr;
		}
		[HttpPost]
		public async Task<JsonResult> SaveCourse(CourseModel CoursesModel, List<int>? remove)
		{
			try
			{
				if (CoursesModel.id > 0)
				{
					CoursesModel.updated_at = DateTime.Now;
					_EduContext.Update(CoursesModel);

					if (remove != null && remove.Count() > 0)
					{
						var chapter_remove = _EduContext.ChapterModel.Where(d => remove.Contains(d.id)).ToList();

						foreach (var chapter in chapter_remove)
						{
							chapter.deleted_at = DateTime.Now;
						}
						_EduContext.UpdateRange(chapter_remove);
					}
				}
				else
				{
					var user_id = UserManager.GetUserId(this.User);
					CoursesModel.author_id = user_id;
					CoursesModel.created_at = DateTime.Now;
					_EduContext.Add(CoursesModel);

				}

				_EduContext.SaveChanges();
				return Json(new { success = true, data = CoursesModel });
			}
			catch (Exception ex)
			{
				return Json(new { success = false, message = ex.Message });
			}

		}

		[HttpPost]
		public async Task<JsonResult> SaveChapter(ChapterModel ChapterModel, List<int>? remove)
		{
			var jsonData = new { success = true, message = "" };
			try
			{
				if (ChapterModel.id > 0)
				{
					ChapterModel.updated_at = DateTime.Now;
					_EduContext.Update(ChapterModel);
					if (remove != null && remove.Count() > 0)
					{
						var lesson_remove = _EduContext.LessonModel.Where(d => remove.Contains(d.id)).ToList();

						foreach (var lesson in lesson_remove)
						{
							lesson.deleted_at = DateTime.Now;
						}
						_EduContext.UpdateRange(lesson_remove);
					}
				}
				else
				{
					ChapterModel.created_at = DateTime.Now;
					_EduContext.Add(ChapterModel);
				}
				_EduContext.SaveChanges();
				return Json(new { success = true, data = ChapterModel });
			}
			catch (Exception ex)
			{
				return Json(new { success = false, message = ex.Message });
			}
		}
		[HttpPost]
		public async Task<JsonResult> SaveLesson(LessonModel LessonModel, List<int>? removeFile, List<int>? removeAttachments)
		{
			var jsonData = new { success = true, message = "" };
			try
			{
				if (LessonModel.id > 0)
				{
					LessonModel.updated_at = DateTime.Now;
					_EduContext.Update(LessonModel);


					if (removeFile != null && removeFile.Count() > 0)
					{
						var file_remove = _EduContext.LessonFileModel.Where(d => removeFile.Contains(d.id)).ToList();

						foreach (var item in file_remove)
						{
							item.deleted_at = DateTime.Now;
						}
						_EduContext.UpdateRange(file_remove);
					}

					if (removeAttachments != null && removeAttachments.Count() > 0)
					{
						var attachments_remove = _EduContext.LessonAttachmentModel.Where(d => removeAttachments.Contains(d.id)).ToList();

						foreach (var item in attachments_remove)
						{
							item.deleted_at = DateTime.Now;
						}
						_EduContext.UpdateRange(attachments_remove);
					}
				}
				else
				{
					LessonModel.created_at = DateTime.Now;
					_EduContext.Add(LessonModel);
					_EduContext.SaveChanges();
				}

				var timeStamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
				var files = Request.Form.Files;
				var list_attachments = new List<IFormFile>();
				var pathroot = _configuration["Source:Path_Private"] + "\\courses\\" + LessonModel.course_id + "\\";
				bool exists = System.IO.Directory.Exists(pathroot);

				if (!exists)
					System.IO.Directory.CreateDirectory(pathroot);
				foreach (var f in files)
				{
					if (f.Name == "attachments[]")
					{
						var name = f.FileName;
						var ext = Path.GetExtension(name);
						var mimeType = f.ContentType;
						var size = f.Length;
						var newName = timeStamp + "-" + name;

						newName = newName.Replace("+", "_");
						newName = newName.Replace("%", "_");
						var filePath = pathroot + newName;
						var url = "/private/courses/" + LessonModel.course_id + "/" + newName;
						var Attachments = new LessonAttachmentModel
						{
							lesson_id = LessonModel.id,
							size = size,
							ext = ext,
							url = url,
							name = name,
							mimeType = mimeType,
							created_at = DateTime.Now
						};

						using (var fileSrteam = new FileStream(filePath, FileMode.Create))
						{
							await f.CopyToAsync(fileSrteam);
						}
						_EduContext.Add(Attachments);
					}
					else if (f.Name == "file")
					{
						var name = f.FileName;
						var ext = Path.GetExtension(name);
						var mimeType = f.ContentType;
						var size = f.Length;


						var newName = timeStamp + "file-" + name;

						newName = newName.Replace("+", "_");
						newName = newName.Replace("%", "_");
						var filePath = pathroot + newName;
						var url = "/private/courses/" + LessonModel.course_id + "/" + newName;
						var File = new LessonFileModel
						{
							lesson_id = LessonModel.id,
							size = size,
							ext = ext,
							url = url,
							name = name,
							mimeType = mimeType,
							created_at = DateTime.Now
						};

						using (var fileSrteam = new FileStream(filePath, FileMode.Create))
						{
							await f.CopyToAsync(fileSrteam);
						}

						_EduContext.Add(File);
					}
				}
				_EduContext.SaveChanges();
				return Json(new { success = true, data = LessonModel });
			}
			catch (Exception ex)
			{
				return Json(new { success = false, message = ex.Message });
			}
		}

		[HttpPost]
		public async Task<JsonResult> CompleteLesson(int id, int percent_pass)
		{
			var jsonData = new { success = true, message = "" };
			try
			{
				var user_id = UserManager.GetUserId(this.User);
				var LessonModel = _EduContext.LessonModel.Where(d => d.id == id).FirstOrDefault();
				if (LessonModel == null)
					return Json(new { success = false, message = "Không tìm thấy Bài học" });

				var LessonPassModel = new LessonPassModel
				{
					lesson_id = id,
					course_id = LessonModel.course_id,
					user_id = user_id,
					date_pass = DateTime.Now,
				};
				_EduContext.Add(LessonPassModel);

				var CoursePassModel = _EduContext.CoursePassModel.Where(d => d.course_id == LessonModel.course_id && d.user_id == user_id).FirstOrDefault();
				if (CoursePassModel == null)
				{
					CoursePassModel = new CoursePassModel
					{
						course_id = LessonModel.course_id,
						user_id = user_id,
						percent_pass = percent_pass
					};
					_EduContext.Add(CoursePassModel);
				}
				else
				{
					CoursePassModel.percent_pass = percent_pass;
					if (percent_pass == 100)
					{
						CoursePassModel.date_pass = DateTime.Now;
					}
					_EduContext.Update(CoursePassModel);
				}
				_EduContext.SaveChanges();
				return Json(new { success = true });
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
				var list = _EduContext.CourseModel.Where(d => item.Contains(d.id)).ToList();
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
			var code = Request.Form["filters[code]"].FirstOrDefault();
			var title = Request.Form["filters[title]"].FirstOrDefault();
			int skip = start != null ? Convert.ToInt32(start) : 0;
			var customerData = _EduContext.CourseModel.Where(d => d.deleted_at == null);
			int recordsTotal = customerData.Count();
			if (code != null && code != "")
			{
				customerData = customerData.Where(d => d.code.Contains(code));
			}

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
			CourseModel CourseModel = _EduContext.CourseModel.Where(d => d.id == id)
				.Include(d => d.chapters.Where(c => c.deleted_at == null))
				.ThenInclude(d => d.lessons.Where(c => c.deleted_at == null))
				.ThenInclude(d => d.file_up)
				.Include(d => d.chapters.Where(c => c.deleted_at == null))
				.ThenInclude(d => d.lessons.Where(c => c.deleted_at == null))
				.ThenInclude(d => d.attachments_up.Where(c => c.deleted_at == null))

				.FirstOrDefault();
			return Json(CourseModel);
		}
		public JsonResult personInfo(int course_id)
		{
			var user_id = UserManager.GetUserId(this.User);
			var CoursePassModel = _EduContext.CoursePassModel.Where(d => d.user_id == user_id && d.course_id == course_id).FirstOrDefault();
			var LessonPassmodel = _EduContext.LessonPassModel.Where(d => d.user_id == user_id && d.course_id == course_id).Select(d => d.lesson_id).Distinct().ToList();
			CoursePassModel.list_lesson_pass = LessonPassmodel;
			return Json(CoursePassModel);
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
