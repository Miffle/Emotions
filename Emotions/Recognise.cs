using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Cvb;
using Emgu.CV.Structure;
using Emgu.Util;
using Microsoft.ProjectOxford.Emotion;
using Microsoft.ProjectOxford.Emotion.Contract;

namespace Emotions
{
    using MyList = List<KeyValuePair<int, System.Drawing.Rectangle>>;

    partial class CameraForm
    {
        Capture cap = new Capture();

        private void capture(object sender, EventArgs e)
        {
           if (pictureBox1.Image != null)
            {
                pictureBox1.Image.Dispose();
                System.GC.Collect();
            }
            if (cap == null)
                cap = new Capture();
            pictureBox1.Image = cap.QueryFrame().Bitmap;
        }

        private void take_photo(object sender, EventArgs e)
        {
           button2.Enabled = true;
            button3.Enabled = true;
            Application.Idle -= capture;
        }

        private void retake_photo(object sender, EventArgs e)
        {
            button2.Enabled = false;
            button3.Enabled = false;
            Application.Idle += capture;
        }

        public static string createFileName()
        {
            string fnm = "";
            if (DateTime.Today.Day < 10)
                fnm += "0";
            fnm += DateTime.Today.Day.ToString();
            if (DateTime.Today.Month < 10)
                fnm += "0";
            fnm += DateTime.Today.Month.ToString();
            fnm += DateTime.Today.Year.ToString() + "_";
            int f = 0;
            if (DateTime.UtcNow.Hour < 7)
            {
                fnm += "0";
                f = 3;
            }
            if (DateTime.UtcNow.Hour > 20 && DateTime.UtcNow.Hour < 24)
            {
                fnm += "0";
                f = -21;
            }
            fnm += (DateTime.UtcNow.Hour + f).ToString();
            if (DateTime.UtcNow.Minute < 10)
                fnm += "0";
            fnm += DateTime.UtcNow.Minute.ToString();
            if (DateTime.UtcNow.Second < 10)
                fnm += "0";
            fnm += DateTime.UtcNow.Second.ToString() + ".bmp";
            return fnm;
        }

        private void accept_photo(object sender, EventArgs e)
        {
            System.IO.Directory.CreateDirectory("temp");
            
            string fnm = System.IO.Directory.GetCurrentDirectory() + "\\temp\\";
            fnm += createFileName();
            cap.QueryFrame().Bitmap.Save(fnm);
            Form1 form1 = (Form1)this.Owner;
            form1.addImage(fnm);            
            this.Close();
        }
    }

    partial class Form1
    {
        //List<System.Drawing.Rectangle[]> faces; //Массив лиц
        //System.Collections.Generic.Dictionary<string,System.Drawing.Rectangle> f1;
        
        Dictionary<string, MyList> faces;
        void faceRec(object sender, EventArgs e)
        {
            CascadeClassifier fc = new CascadeClassifier("haarcascade_frontalface_alt.xml");
            Button pushed = (Button)sender;
            if (faces != null && faces.Keys.Contains(filenames[pushed.Parent.TabIndex]))
            {
                faces[filenames[pushed.Parent.TabIndex]].Clear();
                faces.Remove(filenames[pushed.Parent.TabIndex]);
            }

            Image<Bgr, Byte> image = new Image<Bgr, byte>(filenames[pushed.Parent.TabIndex]);
            Image<Gray, Byte> grayImage = image.Convert<Gray, Byte>();
            try {
                var Face = fc.DetectMultiScale(grayImage, 1.05, 3, new System.Drawing.Size(30, 30));
                if(faces == null)
                    faces = new Dictionary<string, MyList>();
                MyList fs = new MyList();
                for (int i = 0; i < Face.Length; i++)
                {
                    int l = emoRec(filenames[pushed.Parent.TabIndex], Face[i]); System.GC.Collect();
                    //int l = 0;
                    fs.Add(new KeyValuePair<int, System.Drawing.Rectangle>(l, Face[i]));
                }
                faces.Add(filenames[pushed.Parent.TabIndex], fs);                               
            }
            catch (Exception ex)
            {
                toolStripLabel1.Text = ex.Message;
            }
            fc.Dispose();
            grayImage.Dispose();
            image.Dispose();
            Panel pan = (Panel)pushed.Parent;
            ((PictureBox)(pan.Controls[2])).BackgroundImage = global::Emotions.Properties.Resources.apply;

        }

        int emoRec(string path, System.Drawing.Rectangle row)
        {            
            int ret = 2;
            bool f = false;
            System.Drawing.Rectangle m_row = new System.Drawing.Rectangle(row.X, row.Y+(2*row.Height/3), row.Width, row.Height/3);

            Image<Bgr, Byte> image = new Image<Bgr, byte>(path);
            image.ROI = m_row;
            Image<Gray, Byte> grayImage = image.Convert<Gray, Byte>();
            
            CascadeClassifier cas_smile = new CascadeClassifier("haarcascade_smile.xml");
            CascadeClassifier cas_mouth = new CascadeClassifier("haarcascade_mcs_mouth.xml");

            var mouth = cas_mouth.DetectMultiScale(grayImage);

            if (mouth.Length == 0)
            {
                f = true;
                if (cas_smile.DetectMultiScale(grayImage).Length != 0)
                    ret = 1;
                else ret = 0;// return 0;
            }
            
            if (!f)
            {
                int ems = 0;
                for (int i = 0; i < mouth.Length; i++)
                {
                    grayImage.ROI = System.Drawing.Rectangle.Empty;
                    grayImage.ROI = mouth[i];
                    if (cas_smile.DetectMultiScale(grayImage).Length != 0)
                        ems++;
                }

                if (ems > (mouth.Length / 2))
                {
                    f = true;
                    ret = 1;
                }
            }

            if (!f)
            {
                image = image.Flip(FlipType.Vertical);
                grayImage.Dispose();
                grayImage = image.Convert<Gray, Byte>();

                mouth = cas_mouth.DetectMultiScale(grayImage);
                for (int i = 0; i < mouth.Length && !f; i++)
                {
                    grayImage.ROI = System.Drawing.Rectangle.Empty;
                    grayImage.ROI = mouth[i];
                    if (cas_smile.DetectMultiScale(grayImage).Length != 0)
                    {
                        ret = 3;
                        f = true;
                    }
                }
            }
            
            cas_mouth.Dispose();
            cas_smile.Dispose();
            grayImage.Dispose();
            image.Dispose();
            return ret;
        }

        System.Drawing.Image faceImage(string filename, System.Drawing.Rectangle rect)
        {
            Image<Bgr, Byte> image = new Image<Bgr, byte>(filename);
            image.ROI = rect;
            image.Resize(150, 150, Inter.Linear);
            System.Drawing.Image face = image.ToBitmap();
            image.Dispose();
            return face;
        }


    }

    partial class FaceForm
    {
        private async void emoInfo(object sender, EventArgs e)
        {
            Emotion[] emotionResult;
            EmotionServiceClient emoServ = new EmotionServiceClient("0c7929e79d4d4897bdf64a1b9d9c4a17");
            System.IO.MemoryStream im = new System.IO.MemoryStream();
            panel1.BackgroundImage.Save(im, System.Drawing.Imaging.ImageFormat.Bmp);
            im.Seek(0, System.IO.SeekOrigin.Begin);
            
            try {
                emotionResult = await emoServ.RecognizeAsync(im);
                progressBar1.Value = (int)(100*(emotionResult[0].Scores.Anger));
                progressBar2.Value = (int)(100 * (emotionResult[0].Scores.Contempt));
                progressBar3.Value = (int)(100 * (emotionResult[0].Scores.Disgust));
                progressBar4.Value = (int)(100 * (emotionResult[0].Scores.Fear));
                progressBar5.Value = (int)(100 * (emotionResult[0].Scores.Happiness));
                progressBar6.Value = (int)(100 * (emotionResult[0].Scores.Neutral));
                progressBar7.Value = (int)(100 * (emotionResult[0].Scores.Sadness));
                progressBar8.Value = (int)(100 * (emotionResult[0].Scores.Surprise));
            }
            catch (Exception ex){
                errorLabel.Text = ex.Message;
            }
        }
    }
}
