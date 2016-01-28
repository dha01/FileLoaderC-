using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace FileLoader.Models.Service.File
{
	public class FileService
	{

		public List<string> GetFileList(string location)
		{
			var dir = new DirectoryInfo("F://Files//");
			FileInfo[] file_names = dir.GetFiles("*.*");
			List<string> list = new List<string>();

			foreach (var file in file_names)
			{
				list.Add(file.Name);
			}
			return list;
		}

		public string SaveFile(HttpPostedFileBase file, string location)
		{
			if (file.ContentLength == 0)
			{
				throw new Exception("Данные отсутствуют.");
			}

			var new_name = Guid.NewGuid().ToString();
			var path = Path.Combine(location, new_name);
			file.SaveAs(path);
			return new_name;
		}
	}
}