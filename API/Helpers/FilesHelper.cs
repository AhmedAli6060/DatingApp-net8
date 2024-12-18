namespace API.Helpers;

public class FilesHelper
{
    public static void CreateDirectory(string uploadPath)
    {
        string dir = GetDirectory(uploadPath);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
    }

    public static string GetDirectory(string uploadPath)
    {
        return Path.Combine(uploadPath);
    }
    
    public static string SaveFile(IFormFile formfile, string uploadPath)
    {
        string fileName = RenameFiles(formfile.FileName);
        FileStream stream = new FileStream(GetPath(uploadPath, fileName), FileMode.Create);
        formfile.CopyTo(stream);
        stream.Close();
        return fileName;
    }

    public static string GetPath(string uploadPath, string fileName)
    {
        return Path.Combine(uploadPath, fileName);
    }

    public static void RemoveFile(string filePath, string fileName)
    {
        if (fileName != null)
        {
            string FillPath = Path.Combine(filePath, fileName);
            if (System.IO.File.Exists(FillPath))
            {
                System.IO.File.Delete(FillPath);

            }
        }
    }



    public static string RenameFiles(string FileName)
    {
        // Find the index of the last dot
        int lastIndex = FileName.LastIndexOf('.');
        string ext = FileName.Substring(lastIndex + 1);
        string name = DateTime.Now.ToString("yyyyMMddHHmmss") + DateTime.Now.Millisecond + "." + ext;
        return name;
    }
}
