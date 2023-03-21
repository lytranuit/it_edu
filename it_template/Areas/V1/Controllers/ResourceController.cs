
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using it_template.Areas.V1.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using Vue.Data;
using Vue.Models;

namespace it_template.Areas.V1.Controllers
{

	public class ResourceController : BaseController
	{
		private readonly IConfiguration _configuration;
		private readonly EduContext _EduContext;
		private readonly UserManager<UserModel> UserManager;
		public ResourceController(AuthContext context, UserManager<UserModel> UserMgr, EduContext EduContext, IConfiguration configuration) : base(context)
		{
			_configuration = configuration;
			_EduContext = EduContext;
			UserManager = UserMgr;
		}
		public async Task<JsonResult> Groups()
		{
			var GroupModel = _EduContext.GroupModel.Where(d => d.deleted_at == null).Include(d => d.types).ToList();
			return Json(GroupModel);
		}
		public async Task<JsonResult> Topics()
		{
			var TopicModel = _EduContext.TopicModel.Where(d => d.deleted_at == null).ToList();
			return Json(TopicModel);
		}
		public async Task<JsonResult> Exams()
		{
			var ExamModel = _EduContext.ExamModel.Where(d => d.deleted_at == null && d.is_active == true).ToList();
			return Json(ExamModel);
		}
		public async Task<JsonResult> Courses()
		{
			var CourseModel = _EduContext.CourseModel.Where(d => d.deleted_at == null && d.is_active == true).ToList();
			return Json(CourseModel);
		}
		public async Task<JsonResult> GetVideoDuration(string id)
		{
			using (var youtubeService = new YouTubeService(new BaseClientService.Initializer()
			{
				ApiKey = _configuration["Youtube_APIKEY"],
			}))
			{
				// Prepare the request
				var searchRequest = youtubeService.Videos.List("contentDetails");
				searchRequest.Id = id;
				var searchResponse = await searchRequest.ExecuteAsync();

				var youTubeVideo = searchResponse.Items.FirstOrDefault();
				var duration = Convert.ToInt32(System.Xml.XmlConvert.ToTimeSpan(youTubeVideo.ContentDetails.Duration).TotalSeconds);
				return Json(duration);
			}
			return Json(0);
		}
	}
}
