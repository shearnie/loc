using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace loc
{
    public partial class Form1 : Form
    {
        private const string PATH = " your source path here ";
        private static readonly List<string> DirIgnores = new List<string>()
        {
            "bin",
            "obj"
        };
        private static readonly List<string> PathIgnores = new List<string>()
        {
            PATH + "\\packages",
            PATH + "\\TestResults",
        };

        private const int PAD = 200;
        private static readonly List<string> Ignores = new List<string>()
        {
            ".dll", ".exe", ".obj", ".pdb", 
            ".png", ".jpg", ".gif", "svg", ".psd", 
            ".min.js", ".map",
            ".eot", ".otf", ".ttf", ".woff", ".tt",
            ".cache", ".resx", ".sln", ".suo", ".csproj", ".diagram", 
            ".user", ".edmx", ".targets", ".pubxml", ".designer.cs", ".d.ts"
        };
        private int totalLines = 0;
        private Dictionary<string, int> files = new Dictionary<string, int>();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            // intro
            this.txtOut.AppendText("FILENAME".PadRight(PAD) + "LINE COUNT   \r\n");
            this.txtOut.AppendText("-".PadRight(PAD, Convert.ToChar("-")) + "-------------\r\n");

            // files
            this.Calc(PATH);

            // summary
            this.txtOut.AppendText("\r\n");
            this.txtOut.AppendText("EXTENSION SUMMARY".PadRight(PAD) + "LINE COUNT   \r\n");
            this.txtOut.AppendText("-".PadRight(PAD, Convert.ToChar("-")) + "-------------\r\n");
            foreach (var ext in this.files.OrderByDescending(f => f.Value))
            {
                this.txtOut.AppendText(ext.Key.PadRight(PAD) + ext.Value.ToString("###,###,###,###") + "\r\n");
            }

            // total
            this.txtOut.AppendText("\r\n");
            this.txtOut.AppendText("TOTAL".PadRight(PAD) + "LINE COUNT   \r\n");
            this.txtOut.AppendText("=".PadRight(PAD, Convert.ToChar("=")) + "=============\r\n");
            this.txtOut.AppendText(" ".PadRight(PAD) + this.totalLines.ToString("###,###,###,###") + "\r\n");
        }

        private void Calc(string path)
        {
            var d = new DirectoryInfo(path);
            foreach (var file in d.GetDirectories())
            {
                if (!PathIgnores.Any(ignore => ignore.ToLower().Contains(file.FullName.ToLower())) && 
                    !DirIgnores.Contains(file.Name.ToLower()))
                    this.Calc(file.FullName);
            }
            foreach (var file in d.GetFiles())
            {
                if (Ignores.Any(ignore => file.Name.ToLower().EndsWith(ignore))) continue;

                var lines = File.ReadLines(file.FullName).Count();
                this.totalLines += lines;
                
                if (this.files.ContainsKey(file.Extension.ToLower()))
                    this.files[file.Extension.ToLower()] += lines;
                else
                    this.files.Add(file.Extension.ToLower(), lines);
                    
                this.txtOut.AppendText(file.FullName.PadRight(PAD) + lines.ToString("###,###,###,###") + "\r\n");
                Application.DoEvents();
            }
        }
    }
}
