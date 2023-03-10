using Syncfusion.EJ2.FileManager.PhysicalFileProvider;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Syncfusion.EJ2.FileManager.Base;
using System.IO;
using Vue.Models;
using Microsoft.AspNetCore.Identity;

namespace it_template.Areas.FileManager.Controllers
{

	[Route("FileManager/[controller]")]
	[EnableCors("AllowAllOrigins")]
	public class FileManagerController : Controller
	{
		private UserManager<UserModel> UserManager;
		private readonly IConfiguration _configuration;
		public PhysicalFileProvider operation;
		public string basePath;
		string root = "wwwroot\\Files";
		public FileManagerController(IWebHostEnvironment hostingEnvironment, IConfiguration configuration, UserManager<UserModel> UserMgr)
		{
			this.basePath = hostingEnvironment.ContentRootPath;

			_configuration = configuration;
			UserManager = UserMgr;
		}
		[Route("FileOperations")]
		public object FileOperations([FromBody] FileManagerDirectoryContent args)
		{
			this.operation = new PhysicalFileProvider();
			System.Security.Claims.ClaimsPrincipal currentUser = this.User;
			string user_id = UserManager.GetUserId(currentUser); // Get user id:
			string pathroot = _configuration["Source:Path_Private"] + "\\upload\\" + user_id;
			bool exists = System.IO.Directory.Exists(pathroot);

			if (!exists)
				System.IO.Directory.CreateDirectory(pathroot);
			this.operation.RootFolder(pathroot);



			if (args.Action == "delete" || args.Action == "rename")
			{
				if ((args.TargetPath == null) && (args.Path == ""))
				{
					FileManagerResponse response = new FileManagerResponse();
					response.Error = new ErrorDetails { Code = "401", Message = "Restricted to modify the root folder." };
					return this.operation.ToCamelCase(response);
				}
			}
			switch (args.Action)
			{
				case "read":
					// reads the file(s) or folder(s) from the given path.
					return this.operation.ToCamelCase(this.operation.GetFiles(args.Path, args.ShowHiddenItems));
				case "delete":
					// deletes the selected file(s) or folder(s) from the given path.
					return this.operation.ToCamelCase(this.operation.Delete(args.Path, args.Names));
				case "copy":
					// copies the selected file(s) or folder(s) from a path and then pastes them into a given target path.
					return this.operation.ToCamelCase(this.operation.Copy(args.Path, args.TargetPath, args.Names, args.RenameFiles, args.TargetData));
				case "move":
					// cuts the selected file(s) or folder(s) from a path and then pastes them into a given target path.
					return this.operation.ToCamelCase(this.operation.Move(args.Path, args.TargetPath, args.Names, args.RenameFiles, args.TargetData));
				case "details":
					// gets the details of the selected file(s) or folder(s).
					return this.operation.ToCamelCase(this.operation.Details(args.Path, args.Names, args.Data));
				case "create":
					// creates a new folder in a given path.
					return this.operation.ToCamelCase(this.operation.Create(args.Path, args.Name));
				case "search":
					// gets the list of file(s) or folder(s) from a given path based on the searched key string.
					return this.operation.ToCamelCase(this.operation.Search(args.Path, args.SearchString, args.ShowHiddenItems, args.CaseSensitive));
				case "rename":
					// renames a file or folder.
					return this.operation.ToCamelCase(this.operation.Rename(args.Path, args.Name, args.NewName));
			}
			return null;
		}

		// uploads the file(s) into a specified path
		[Route("Upload")]
		public IActionResult Upload(string path, IList<IFormFile> uploadFiles, string action)
		{
			this.operation = new PhysicalFileProvider();
			System.Security.Claims.ClaimsPrincipal currentUser = this.User;
			string user_id = UserManager.GetUserId(currentUser); // Get user id:
			string pathroot = _configuration["Source:Path_Private"] + "\\upload\\" + user_id;
			bool exists = System.IO.Directory.Exists(pathroot);

			if (!exists)
				System.IO.Directory.CreateDirectory(pathroot);
			this.operation.RootFolder(pathroot);

			FileManagerResponse uploadResponse;
			foreach (var file in uploadFiles)
			{
				var folders = (file.FileName).Split('/');
				// checking the folder upload
				if (folders.Length > 1)
				{
					for (var i = 0; i < folders.Length - 1; i++)
					{
						string newDirectoryPath = Path.Combine(this.basePath + path, folders[i]);
						if (!Directory.Exists(newDirectoryPath))
						{
							this.operation.ToCamelCase(this.operation.Create(path, folders[i]));
						}
						path += folders[i] + "/";
					}
				}
			}
			uploadResponse = operation.Upload(path, uploadFiles, action, null);
			if (uploadResponse.Error != null)
			{
				Response.Clear();
				Response.ContentType = "application/json; charset=utf-8";
				Response.StatusCode = Convert.ToInt32(uploadResponse.Error.Code);
				Response.HttpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase = uploadResponse.Error.Message;
			}
			return Content("");
		}

		// downloads the selected file(s) and folder(s)
		[Route("Download")]
		public IActionResult Download(string downloadInput)
		{
			this.operation = new PhysicalFileProvider();
			System.Security.Claims.ClaimsPrincipal currentUser = this.User;
			string user_id = UserManager.GetUserId(currentUser); // Get user id:
			string pathroot = _configuration["Source:Path_Private"] + "\\upload\\" + user_id;
			bool exists = System.IO.Directory.Exists(pathroot);

			if (!exists)
				System.IO.Directory.CreateDirectory(pathroot);
			this.operation.RootFolder(pathroot);



			FileManagerDirectoryContent args = JsonConvert.DeserializeObject<FileManagerDirectoryContent>(downloadInput);
			return this.operation.Download(args.Path, args.Names, args.Data);
		}

		// gets the image(s) from the given path
		[Route("GetImage")]
		public IActionResult GetImage(FileManagerDirectoryContent args)
		{
			this.operation = new PhysicalFileProvider();
			System.Security.Claims.ClaimsPrincipal currentUser = this.User;
			string user_id = UserManager.GetUserId(currentUser); // Get user id:
			string pathroot = _configuration["Source:Path_Private"] + "\\upload\\" + user_id;
			bool exists = System.IO.Directory.Exists(pathroot);

			if (!exists)
				System.IO.Directory.CreateDirectory(pathroot);
			this.operation.RootFolder(pathroot);



			return this.operation.GetImage(args.Path, args.Id, false, null, null);
		}

		[Route("fileupload")]
		[HttpPost]
		public async Task<JsonResult> fileupload(string id)
		{
			var files = Request.Form.Files;
			//return Json(files);
			if (files != null && files.Count > 0)
			{
				var items = new List<string>();
				var pathroot = _configuration["Source:Path_Private"] + "\\courses\\" + id;
				bool exists = System.IO.Directory.Exists(pathroot);

				if (!exists)
					System.IO.Directory.CreateDirectory(pathroot);
				foreach (var file in files)
				{
					var timeStamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
					string name = file.FileName;
					string ext = Path.GetExtension(name);
					string mimeType = file.ContentType;

					//var fileName = Path.GetFileName(name);
					var newName = timeStamp + " - " + name;
					newName = newName.Replace("+", "_");
					newName = newName.Replace("%", "_");
					var filePath = pathroot + "\\" + newName;
					string url = "/private/courses/" + id + "/" + newName;
					items.Add(url);

					using (var fileSrteam = new FileStream(filePath, FileMode.Create))
					{
						await file.CopyToAsync(fileSrteam);
					}
				}
				//_context.AddRange(items);
				//_context.SaveChanges();
				return Json(new { success = 1, items = items });
			}
			return Json(new { message = "Lỗi" });
		}

	}

}
