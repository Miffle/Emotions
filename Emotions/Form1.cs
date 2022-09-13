using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Emotions
{
    public partial class Form1 : Form
    {
        private List<string> filenames;
        public Form1()
        {
            InitializeComponent();
        }

        private int min(int f, int s)
        {
            if (f < s)
                return f;
            else
                return s;
        }
        
        public void addImage(string path)
        {
            if (filenames == null)
                filenames = new List<string>();
            filenames.Add(path);
            //Создаем ячейку с картинкой
            Panel new_panel = new Panel();

            new_panel.BackgroundImage = Image.FromFile(path);
            new_panel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            new_panel.Name = "panel";
            new_panel.Name += flowLayoutPanel2.Controls.Count.ToString();
            new_panel.Size = new System.Drawing.Size(230, 220);
            new_panel.TabIndex = flowLayoutPanel2.Controls.Count;
            new_panel.Click += new System.EventHandler(this.image_Click);
            new_panel.DoubleClick += new System.EventHandler(this.openImage);

            //Добавляем кнопку её удаления
            Button new_button = new Button();

            //new_button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            new_button.BackgroundImage = global::Emotions.Properties.Resources.delete;
            new_button.Click += new System.EventHandler(this.close_Image);
            new_button.Name = "del_button" + new_panel.TabIndex.ToString();
            new_button.Location = new System.Drawing.Point(200, 0);

            new_button.BackColor = System.Drawing.Color.Transparent;
            new_button.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            new_button.FlatAppearance.BorderSize = 0;
            new_button.FlatAppearance.MouseDownBackColor = System.Drawing.SystemColors.ActiveBorder;
            new_button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            new_button.Size = new System.Drawing.Size(30, 30);
            new_button.TabIndex = 0;
            new_button.UseVisualStyleBackColor = false;

            new_panel.Controls.Add(new_button);
            //Добавляем кнопку Play
            new_button = new Button();

            //new_button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            new_button.Name = "play_button" + new_panel.TabIndex.ToString();
            new_button.BackgroundImage = global::Emotions.Properties.Resources.play;
            new_button.Location = new System.Drawing.Point(200, 190);
            new_button.Click += new System.EventHandler(this.faceRec);

            new_button.BackColor = System.Drawing.Color.Transparent;
            new_button.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            new_button.FlatAppearance.BorderSize = 0;
            new_button.FlatAppearance.MouseDownBackColor = System.Drawing.SystemColors.ActiveBorder;
            new_button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            new_button.Size = new System.Drawing.Size(30, 30);
            new_button.TabIndex = 1;
            new_button.UseVisualStyleBackColor = false;

            new_panel.Controls.Add(new_button);
            //Добавляем иконку эмоции
            PictureBox new_emo = new PictureBox();

            new_emo.BackColor = Color.Transparent;
            //new_emo.BackgroundImage = global::Emotions.Properties.Resources.unrec;
            new_emo.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            new_emo.Location = new System.Drawing.Point(0, 190);
            new_emo.Name = "emotion" + new_panel.TabIndex.ToString();
            new_emo.Size = new System.Drawing.Size(30, 30);
            new_emo.TabIndex = 2;
            new_emo.TabStop = false;
            new_panel.Controls.Add(new_emo);

            flowLayoutPanel2.Controls.Add(new_panel);
        }

        private void openImage(object sender, EventArgs e)
        {
            Panel cur_panel = (Panel)sender;
            System.Diagnostics.Process.Start(filenames[cur_panel.TabIndex]);
        }

        private void открытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Если не была нажата ОК -> выход
            if (openFileDialog1.ShowDialog() != DialogResult.OK) {
                openFileDialog1.FileName = "";
                return;
            }
            //Путь к файлу в openFileDialog1.FileName
            
            int cnt = openFileDialog1.FileNames.Length;
            for (int i=0;i< cnt; i++)
            {
                addImage(openFileDialog1.FileNames[i]);
            }
            
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            открытьToolStripMenuItem_Click(sender, e);
        }

        private void recAll(object sender, EventArgs e)
        {
            for (int i = 0; i < flowLayoutPanel2.Controls.Count; i++)
            {
                Button curButton = (Button)flowLayoutPanel2.Controls[i].Controls[1];
                faceRec(curButton, e);
            }
        }

        private void captureCamera(object sender, EventArgs e)
        {
            CameraForm cam = new CameraForm();
            cam.Owner = this;
            cam.ShowDialog();
            cam.Dispose();
        }

        private Image setEmoIcon(int n)
        {
             switch (n)
            {
                case 0: //Эмоция не распознана
                    return global::Emotions.Properties.Resources.unrec;                    
                case 1: //Радость
                    return global::Emotions.Properties.Resources.happy;                    
                case 2: //Нейтральный
                    return global::Emotions.Properties.Resources.neutral;                    
                case 3: //Грусть
                    return global::Emotions.Properties.Resources.sad;                    
                default:
                    break;
            }
            return null;
        }
        private void reFace()
        {
            for (int i = 0; i < min(flowLayoutPanel1.Controls.Count, faces[toolStripLabel1.Text].Count); i++)
            {
                KeyValuePair<int, Rectangle> new_face = (faces[toolStripLabel1.Text])[i];
                Panel cur_face = (Panel)flowLayoutPanel1.Controls[i];
                cur_face.BackgroundImage.Dispose();
                cur_face.BackgroundImage = faceImage(toolStripLabel1.Text, new_face.Value);
                PictureBox emo_icon = (PictureBox)cur_face.Controls[0];
                emo_icon.Image.Dispose();
                emo_icon.Image = setEmoIcon(new_face.Key);
            }
        }

        private void newFace()
        {
            for (int i = flowLayoutPanel1.Controls.Count; i < faces[toolStripLabel1.Text].Count; i++)
            {
                KeyValuePair<int, Rectangle> new_fc = (faces[toolStripLabel1.Text])[i];
                Panel new_face = new Panel();
                new_face.Margin = new System.Windows.Forms.Padding(3, 3, 3, 50);
                new_face.Name = "face" + i.ToString();
                new_face.Size = new System.Drawing.Size(150, 150);
                new_face.TabIndex = i;
                new_face.BackgroundImage = faceImage(toolStripLabel1.Text, new_fc.Value);
                new_face.BackgroundImageLayout = ImageLayout.Zoom;
                new_face.DoubleClick += faceProp;

                PictureBox emo_icon = new PictureBox();
                emo_icon.Location = new System.Drawing.Point(0, 120);
                emo_icon.Name = "emo" + i.ToString();
                emo_icon.Size = new System.Drawing.Size(30, 30);
                emo_icon.TabIndex = 0;
                emo_icon.TabStop = false;

                emo_icon.BackColor = Color.Transparent;
                emo_icon.Image = setEmoIcon(new_fc.Key);
                emo_icon.SizeMode = PictureBoxSizeMode.Zoom;
                new_face.Controls.Add(emo_icon);

                flowLayoutPanel1.Controls.Add(new_face);
            }
        }

        private void delFace(int from, int to)
        {
            for (int i = to - 1; i >= from; i--)
            {
                Panel del_face = (Panel)flowLayoutPanel1.Controls[i];
                PictureBox del_emo = (PictureBox)del_face.Controls[0];
                del_emo.Image.Dispose();
                del_emo.Dispose();
                del_face.Controls.Clear();
                del_face.BackgroundImage.Dispose();
                del_face.Dispose();
                flowLayoutPanel1.Controls.Remove(del_face);
            }
        }

        private void image_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < flowLayoutPanel2.Controls.Count; i++)
            {
                Panel t_panel = (Panel)flowLayoutPanel2.Controls[i];
                t_panel.BackColor = System.Drawing.SystemColors.ActiveBorder;
                t_panel.BorderStyle = BorderStyle.None;
            }
            Panel curPanel = (Panel)sender;
            curPanel.BackColor = System.Drawing.SystemColors.ActiveCaption;
            curPanel.BorderStyle = BorderStyle.FixedSingle;
            toolStripLabel1.Text = filenames[curPanel.TabIndex];

            if (faces != null)
            {
                if (faces.Keys.Contains(toolStripLabel1.Text))
                {
                    reFace();
                    if (flowLayoutPanel1.Controls.Count < faces[toolStripLabel1.Text].Count)
                        newFace();
                    else
                        delFace(faces[toolStripLabel1.Text].Count, flowLayoutPanel1.Controls.Count);
                }
                else
                    delFace(0, flowLayoutPanel1.Controls.Count);
            }
        }

        private void close_Image(object sender, EventArgs e)
        {
            Button cur_button = (Button)sender;
            Panel del_panel = (Panel)cur_button.Parent;
            FlowLayoutPanel parent = (FlowLayoutPanel)del_panel.Parent;

            if (del_panel.BackColor == SystemColors.ActiveCaption)
                delFace(0, flowLayoutPanel1.Controls.Count);
            if (faces != null && faces.Keys.Contains(filenames[del_panel.TabIndex]))
            {
                faces[filenames[del_panel.TabIndex]].Clear();
                faces.Remove(filenames[del_panel.TabIndex]);
            }
            for (int i = del_panel.TabIndex+1; i < parent.Controls.Count; i++)
            {
                parent.Controls[i].TabIndex--;
                parent.Controls[i].Controls[0].Name = "del_button" + parent.Controls[i].TabIndex.ToString();
                parent.Controls[i].Controls[1].Name = "play_button" + parent.Controls[i].TabIndex.ToString();
                parent.Controls[i].Controls[2].Name = "emotion" + parent.Controls[i].TabIndex.ToString();
            }
            if (del_panel.Controls[2].BackgroundImage != null)
                del_panel.Controls[2].BackgroundImage.Dispose();
            del_panel.Controls[2].Dispose();
            del_panel.Controls[1].BackgroundImage.Dispose();
            del_panel.Controls[1].Dispose();
            del_panel.Controls[0].BackgroundImage.Dispose();
            del_panel.Controls[0].Dispose();
            del_panel.Controls.Clear();
            del_panel.BackgroundImage.Dispose();
            
            del_panel.Dispose();
            parent.Controls.Remove(del_panel);
            filenames.RemoveRange(del_panel.TabIndex, 1);

            GC.Collect();
        }

        private void flowLayoutPanel2_MouseEnter(object sender, EventArgs e)
        {
            if (ActiveForm == this)
                flowLayoutPanel2.Focus();
            
        }

        private void flowLayoutPanel1_MouseEnter(object sender, EventArgs e)
        {
            if (ActiveForm == this)
                flowLayoutPanel1.Focus();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            delFace(0, flowLayoutPanel1.Controls.Count);
            
            for (int i = 0; i<flowLayoutPanel2.Controls.Count; i++)
                ((Panel)(flowLayoutPanel2.Controls[i])).BackgroundImage.Dispose();

            if (System.IO.Directory.Exists("temp"))
            {
                string[] files = System.IO.Directory.GetFiles("temp");
                for (int i = 0; i < files.Length; i++)
                    System.IO.File.Delete(files[i]);
                System.IO.Directory.Delete("temp");
            }
        }
    }
}
