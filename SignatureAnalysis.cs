using System;
using System.IO;
using System.Text;

namespace Signature_Analysis
{
    class Program
    {
		/// <summary>
		/// Main class takes command line input and processes it depending on desired traversal
		/// </summary>
		/// <remarks> please note program deficiency in ProcessMultipleDirectories() method description </remarks>
        static void Main(string[] args)
        {
			bool flag = false;
			if (args.Length == 0)
				Console.WriteLine("No args");
			else if (args.Length == 1)
				Console.WriteLine("Please provide an output path");
			else if (args.Length == 3)
				flag = true;
			
			var inDirectory = args[0];
			var outPath = args[1];
			try
			{
				DirectoryInfo di = new DirectoryInfo(inDirectory);
				if (flag)
				{
					ProcessFiles(di, outPath);
					ProcessMultipleDirectories(di.GetDirectories(), outPath);
				}
				else
					ProcessFiles(di, outPath);
				Console.WriteLine("Complete");
			}catch(Exception){}
        }

		/// <summary>
		/// Processes files from the provided DirectoryInfo. Each file is processed, then 
		/// is added to the file specified by the user if the file's extension is correct.
		/// </summary>
		public static void ProcessFiles(DirectoryInfo di, string outPath)
		{
			try{
			if (!di.Exists)
			{
				Console.Write("Directory Does not exist");
				return;
			}
			FileInfo[] files = di.GetFiles("*.*");
			for (int i = 0; i < files.Length; i++)
			{
				if(CheckPDF(files[i]) || CheckJPG(files[i]))
					AddToCSV(files[i], outPath);
			}	
			}catch(Exception){}
		}
		
		/// <summary>
		/// If the user specified that sub-directories are to be checked, recursively traverses 
		/// sub-directories.
		/// </summary>
		/// <remarks> My attempt was a recursive one. The program completes as expected unless 
		///			  a file doesn't have proper access authorization, at which point the exception
		///			  is consumed to continue, but the recursion is thrown off. </remarks>
		public static void ProcessMultipleDirectories(DirectoryInfo[] directories, string outPath)
		{
			try
			{
				foreach(DirectoryInfo dir in directories)
					ProcessFiles(dir, outPath);
				if(directories != null)
				{
					foreach(DirectoryInfo dir in directories)
					{
						DirectoryInfo[] subDir = dir.GetDirectories();
						ProcessMultipleDirectories(subDir, outPath);
					}
				}
			}catch(UnauthorizedAccessException){}
		}

		/// <summary>
		/// Checks if the provided file is a PDF using the file's header.
		/// </summary>
		public static bool CheckPDF(FileInfo file)
		{
			try
			{
				using(FileStream fs = file.Open(FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
				{
					byte[] fileBytes = new byte[4];
					int n = 0;
					while(n < 4)
					{
						fileBytes[n] = (byte)fs.ReadByte();
						n++;
					}
					if (fileBytes[0] == 37 && fileBytes[1] == 80 && 
					fileBytes[2] == 68 && fileBytes[3] == 70)
						return true;
					return false;
				}
			}catch(UnauthorizedAccessException){return false;}
		}

		/// <summary>
		/// Checks if the provided file is a JPG using the file's header.
		/// </summary>
		public static bool CheckJPG(FileInfo file)
		{
			try
			{
				using(FileStream fs = file.Open(FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
				{
					byte[] fileBytes = new byte[2];
					int n = 0;
					while(n < 2)
					{
						fileBytes[n] = (byte)fs.ReadByte();
						n++;
					}
			
					if (fileBytes[0] == 255 && fileBytes[1] == 216)
						return true;
					return false;
				}
			}catch(UnauthorizedAccessException){return false;}
		}
		
		/// <summary>
		/// Adds file information to the designated output path.
		/// </summary>
		public static void AddToCSV(FileInfo file, string outPath)
		{
			try
			{
				if(!File.Exists(outPath))
					using(File.Create(outPath)){}
				File.AppendAllText(outPath, file.FullName.Replace("," , ""));
				File.AppendAllText(outPath, ",");
				if(CheckPDF(file))
					File.AppendAllText(outPath, "PDF,");
				else
					File.AppendAllText(outPath, "JPG,");
				
				File.AppendAllText(outPath, CreateMD5(file));
			}catch(Exception){}
		}
		
		/// <summary>
		///	Hashes contents of the given file using MD5.
		/// </summary>
		public static string CreateMD5(FileInfo file)
		{
			try
			{
				System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();
				using(FileStream fs = file.Open(FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
				{
					StringBuilder result = new StringBuilder();
					byte[] fileContents = new byte[file.Length];
					int c = 0;
					for(int i = 0; i < file.Length; i++)
					{
						if(c == -1)
						break;
						fileContents[i] = (byte)fs.ReadByte();
					}
					
					byte[] hashedContents = md5.ComputeHash(fileContents);
					result.Append(BitConverter.ToString(hashedContents));
					result.Append("\n");
					return result.ToString();
				}
			}catch(Exception){return "";}
		}
    }
}