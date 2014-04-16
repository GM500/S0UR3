using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace S0URC3
{
    public partial class Form1 : Form
    {
        string MKBFilename = "";
        List<string> MKBFilenamesList = new List<string>();

        public void ListFiles(string directoryPath)
        {
            if (directoryPath.Contains("build_")) return;

            DirectoryInfo directoryInfo= new DirectoryInfo(directoryPath);
           
            if(directoryInfo.Exists)
            {
                FileInfo[] headerFiles = directoryInfo.GetFiles("*.h");
                FileInfo[] sourceFiles = directoryInfo.GetFiles("*.cpp");

                if (headerFiles.Length > 0 || sourceFiles.Length > 0)
                {
                    FileInfo MKBFileInfo = new FileInfo(MKBFilename);
                    string MKBPath = directoryPath;
                    MKBPath = MKBPath.Replace(MKBFileInfo.DirectoryName, "");
                    MKBPath = MKBPath.Replace("\\", "/");
                    if (MKBPath.Length > 0) MKBPath = MKBPath.Remove(0, 1);
                    string MKBFilter = MKBPath;
                    MKBFilter = MKBFilter.Insert(0, "[\"");
                    MKBFilter += "\"]";
                    MKBFilter = MKBFilter.Insert(0, "\t");
                    MKBFilenamesList.Add(MKBFilter);
                    MKBPath = MKBPath.Insert(0, "(\"");
                    MKBPath += "\")";
                    MKBPath = MKBPath.Insert(0, "\t");
                    MKBFilenamesList.Add(MKBPath);

                    foreach (FileInfo headerFile in headerFiles)
                    {
                        MKBFilenamesList.Add("\t" + "\"" + headerFile.Name + "\"");
                    }

                    foreach (FileInfo sourceFile in sourceFiles)
                    {
                        MKBFilenamesList.Add("\t" + "\"" + sourceFile.Name + "\"");
                    }

                    MKBFilenamesList.Add("\n");

                }

                DirectoryInfo[] subdirectoryInfo = directoryInfo.GetDirectories();
 
                foreach(DirectoryInfo subDirectory in subdirectoryInfo)
                {
                    ListFiles(subDirectory.FullName);
                }
           
            }
        
        }

        private void Parse_MKB_Root()
        {
            // relies on having and MKB to populate
            MKBFilenamesList.Clear();
            FileInfo MKBFileInfo = new FileInfo(MKBFilename);
            ListFiles(MKBFileInfo.DirectoryName);

        }

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            // accept file names
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.All;

        }

        private void Insert_Filenames_to_MKB()
        {
            // seek files then closing brace
            if (MKBFilename.Length > 0)
            {
                int fileInsertLine = -1;
                int fileClosingBrace = -1;
                string[] lines = File.ReadAllLines(MKBFilename);
                for (int i = 0; i < lines.Length; i++)
                {
                    if (fileInsertLine >= 0)
                    {
                        if (lines[i].Equals("{", StringComparison.OrdinalIgnoreCase))
                        {
                            fileInsertLine = i;
                        }
                        else if (lines[i].Equals("}", StringComparison.OrdinalIgnoreCase))
                        {
                            fileClosingBrace = i;
                            break;
                        }

                    }
                    else if (lines[i].Equals("files", StringComparison.OrdinalIgnoreCase)) fileInsertLine = i;

                }

                // insert filename before closing brace
                if (fileInsertLine > 0)
                {
                    string[] topHalfLines = new string[fileInsertLine + 1];
                    for (int i = 0; i < fileInsertLine + 1; i++) topHalfLines[i] = lines[i];

                    string[] endHalfLines = new string[lines.Length - fileClosingBrace];
                    for (int i = 0; i < lines.Length - fileClosingBrace; i++) endHalfLines[i] = lines[i + fileClosingBrace];

                    //int EmptyLines = 0;
                    //foreach (string Line in Filenames) if (Line == null || Line.Length <= 0) EmptyLines++;

                    //string[] TrimmedFilenames = new string[Filenames.Length - EmptyLines];
                    //for (int i = 0; i < Filenames.Length - EmptyLines; i++) if (Filenames[i] != null && Filenames[i].Length > 0) TrimmedFilenames[i] = Filenames[i];

                    File.WriteAllLines(MKBFilename, topHalfLines);
                    if (MKBFilenamesList.Count > 0) File.AppendAllLines(MKBFilename, MKBFilenamesList);
                    File.AppendAllLines(MKBFilename, endHalfLines);

                }
            }
        }

        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            // load images from file names
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (string file in files) Console.WriteLine("File " + file + " dropped.");
            string[] MKBFriendlyFilenames = new string[files.Length];
            for (int i = 0; i < files.Length; i++)
                if (files[i].EndsWith(".mkb", StringComparison.OrdinalIgnoreCase))
                {
                    MKBFilename = files[i];
                    Text = MKBFilename;
                    

                }
                else
                {
                    MKBFriendlyFilenames[i] = Path.GetFileName(files[i]);
                    Console.WriteLine(files[i] + " loaded.");

                }

            

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        public void Create_Empty_Class(string Classname)
        {
            if (MKBFilename.Length > 0 && Classname.Length > 0)
            {
                // .h
                List<string> HeaderContents = new List<string>();
                HeaderContents.AddRange(File.ReadAllLines("ClassName.h"));
                for (int i = 0; i < HeaderContents.Count; i++)
                {
                    HeaderContents[i] = HeaderContents[i].Replace("CLASS_NAME", Classname.Replace(" ", "_").ToUpper());
                    HeaderContents[i] = HeaderContents[i].Replace("ClassName", Classname.Replace(" ", ""));
                }
                string HeaderFilename = Path.GetDirectoryName(MKBFilename) + "\\" + Classname.Replace(" ", "").ToLower() + ".h";

                //insert image pointers
                int IMAGE_POINTERS = -1;
                for (int i = 0; i < HeaderContents.Count; i++)
                {
                    if (HeaderContents[i].Contains("IMAGE_POINTERS"))
                    {
                        IMAGE_POINTERS = i;
                    }

                }

                File.WriteAllLines(HeaderFilename, HeaderContents);

                // .cpp
                List<string> SourceContents = new List<string>();
                SourceContents.AddRange(File.ReadAllLines("ClassName.cpp"));
                for (int i = 0; i < SourceContents.Count; i++) SourceContents[i] = SourceContents[i].Replace("ClassName", Classname.Replace(" ", ""));
                string SourceFilename = Path.GetDirectoryName(MKBFilename) + "\\" + Classname.Replace(" ", "").ToLower() + ".cpp";

                //insert
                int LOAD_FROM_FILE = -1;
                int DELETE_ALL = -1;
                for (int i = 0; i < SourceContents.Count; i++)
                {
                    if (SourceContents[i].Contains("LOAD_FROM_FILE"))
                    {
                        LOAD_FROM_FILE = i;
                    }

                    if (SourceContents[i].Contains("DELETE_ALL"))
                    {
                        DELETE_ALL = i;
                    }

                }

                File.WriteAllLines(SourceFilename, SourceContents);

                // .mkb
                string[] MKBFriendlyFilenames = { Path.GetFileName(HeaderFilename), Path.GetFileName(SourceFilename) };
                Insert_Filenames_to_MKB();

            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form2 form2 = new Form2();
            form2.form1 = this;
            form2.Show();

        }

        List<string> GroupFilenamesList = new List<string>();

        public void ListResourceFiles(string directoryPath)
        {
            if (directoryPath.Contains("build_")) return;

            DirectoryInfo directoryInfo = new DirectoryInfo(directoryPath);

            if (directoryInfo.Exists)
            {
                FileInfo[] pngFiles = directoryInfo.GetFiles("*.png");
                FileInfo[] tgaFiles = directoryInfo.GetFiles("*.tga");
                FileInfo[] gxfontFiles = directoryInfo.GetFiles("*.gxfont");

                if (pngFiles.Length > 0 || (tgaFiles.Length > 0 && gxfontFiles.Length > 0))
                {
                    GroupFilenamesList.Clear();
                    GroupFilenamesList.Add("CIwResGroup");
                    GroupFilenamesList.Add("{");

                    FileInfo MKBFileInfo = new FileInfo(MKBFilename);
                    string GroupPath = directoryPath;
                    GroupPath = GroupPath.Replace(MKBFileInfo.DirectoryName, "");
                    GroupPath = GroupPath.Replace("\\data\\", "");

                    GroupFilenamesList.Add("\t" + "name " + GroupPath);

                    foreach (FileInfo pngFile in pngFiles)
                    {
                        GroupFilenamesList.Add("\t" + "\"" + pngFile.Name + "\"");
                    }

                    foreach (FileInfo tgaFile in tgaFiles)
                    {
                        GroupFilenamesList.Add("\t" + "\"" + tgaFile.Name + "\"");
                    }

                    foreach (FileInfo gxfontFile in gxfontFiles)
                    {
                        GroupFilenamesList.Add("\t" + "\"" + gxfontFile.Name + "\"");
                    }

                    GroupFilenamesList.Add("\n");
                    GroupFilenamesList.Add("}");

                    File.WriteAllLines(directoryPath + "\\" + GroupPath + ".group", GroupFilenamesList);

                }

                DirectoryInfo[] subdirectoryInfo = directoryInfo.GetDirectories();

                foreach (DirectoryInfo subDirectory in subdirectoryInfo)
                {
                    ListResourceFiles(subDirectory.FullName);
                }

            }

        }

        private void Parse_Data_Folder()
        {
            GroupFilenamesList.Clear();
            FileInfo MKBFileInfo = new FileInfo(MKBFilename);
            ListResourceFiles(MKBFileInfo.DirectoryName + "\\data");

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Parse_MKB_Root();
            Insert_Filenames_to_MKB();
            Parse_Data_Folder();

        }

        private void generate_resource_cpp_h(string GroupName)
        {
            if (MKBFilename.Length > 0 && GroupName.Length > 0)
            {
                // .h
                string[] HeaderContents = File.ReadAllLines("ResourceCollection.h");
                for (int i = 0; i < HeaderContents.Length; i++)
                {
                    HeaderContents[i] = HeaderContents[i].Replace("HEADER_GAURD", GroupName.Replace(" ", "_").ToUpper());
                    HeaderContents[i] = HeaderContents[i].Replace("CLASS_NAME", GroupName.Replace(" ", ""));
                }
                string HeaderFilename = Path.GetDirectoryName(MKBFilename) + "\\src\\resources\\" + GroupName.Replace(" ", "").ToLower() + ".h";
                File.WriteAllLines(HeaderFilename, HeaderContents);

                // .cpp
                string[] SourceContents = File.ReadAllLines("ResourceCollection.cpp");
                for (int i = 0; i < SourceContents.Length; i++) SourceContents[i] = SourceContents[i].Replace("CLASS_NAME", GroupName.Replace(" ", ""));
                string SourceFilename = Path.GetDirectoryName(MKBFilename) + "\\src\\resources\\" + GroupName.Replace(" ", "").ToLower() + ".cpp";
                File.WriteAllLines(SourceFilename, SourceContents);

                // .mkb
                string[] MKBFriendlyFilenames = { Path.GetFileName(HeaderFilename), Path.GetFileName(SourceFilename) };
                Insert_Filenames_to_MKB();

            }
        }
    }
}
