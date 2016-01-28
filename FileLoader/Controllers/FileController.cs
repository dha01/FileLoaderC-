using System.Diagnostics;
using System.Web;
using System.Web.Mvc;
using System.IO;
using FileLoader.Models.Service.File;

namespace FileLoader.Controllers
{
	public class FileController : Controller
	{
		private FileService _fielService;

		private string _path = "F://Files//";

		public FileController()
		{
			_fielService = new FileService();
		}

		[AllowAnonymous]
		public ActionResult Index()
		{
			return View(_fielService.GetFileList(_path));
		}

		/// <summary>
		/// Возвращает файл с сервера.
		/// </summary>
		/// <param name="FileName">Имя файла.</param>
		/// <returns></returns>
		[AllowAnonymous]
		public FileResult Download(string FileName)
		{
			var result = File(_path + FileName, System.Net.Mime.MediaTypeNames.Application.Octet, FileName);
			return result;
		}

		/// <summary>
		/// Загружает файл на сервер..
		/// </summary>
		/// <param name="file">Файл.</param>
		/// <returns></returns>
		[HttpPost]
		public ActionResult Upload(HttpPostedFileBase file)
		{
			var new_name = _fielService.SaveFile(file, _path);
			return Json(new { new_name }, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Загрузка файла, выполнение подпрограммы и возврат резульитата.
		/// </summary>
		/// <param name="file">Файл.</param>
		/// <param name="operation_name">Название файла подпрограммы.</param>
		/// <returns>Файл с результатом.</returns>
		[HttpPost]
		public FileResult UploadAndRunOperation(HttpPostedFileBase file, string operation_name)
		{
			// Получаем имя файла.
			var file_name = Path.GetFileName(file.FileName);

			// Сохраняем файл на диск.
			var new_name = _fielService.SaveFile(file, _path);

			// Имя файла с результатом.
			var result_file_name = string.Format("OutFile_{0}", new_name);
			
			// Создаем новый процесс.
			Process proc = new Process
			{
				StartInfo =
				{
					// Имя запускаемой подпрограммы.
					FileName = string.Format(@"{0}\{1}", _path, operation_name),
					// Аргументы для запуска.
					Arguments = string.Format(@"{0}{1} {0}{2}", _path, new_name, result_file_name)
				}
			};

			// Запускаем процесс.
			proc.Start();

			// Ожидаем завершения.
			proc.WaitForExit();

			// Возвращаем файл.
			return File(_path + result_file_name, System.Net.Mime.MediaTypeNames.Application.Octet, string.Format(@"Result_{0}_{1}",new_name, file_name));
		}

		/// <summary>
		/// Удаляет файл
		/// </summary>
		/// <param name="FileName">Имя файла.</param>
		/// <returns></returns>
		public ActionResult Remove(string FileName)
		{
			System.IO.File.Delete(_path + FileName);
			return Redirect(HttpContext.Request.UrlReferrer.AbsoluteUri);
		}
	}
}
