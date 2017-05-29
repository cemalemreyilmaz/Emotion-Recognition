using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Luxand;
using System.IO;
using FacialFeatures_VS2010s;

namespace FacialFeatures
{
    public partial class Form1 : Form
    {
        // WinAPI procedure to release HBITMAP handles returned by FSDKCam.GrabFrame
        [DllImport("gdi32.dll")]
        static extern bool DeleteObject(IntPtr hObject);

        public Form1()
        {
            InitializeComponent();
        }
        double[] dizi2 = new double[132];
        List<string> list = new List<string>();

        private void Form1_Load(object sender, EventArgs e)
        {
            if (FSDK.FSDKE_OK != FSDK.ActivateLibrary("bq39OVODkxU5WnoCPMjrCrklbBDY7dFbEZF+KwLbS7nVsp7DMRQbpXYM/+DiANnEOu8FdWaTe2sSUhknwohMI5WOpKSUrXu1v/COtp30YSUdisb6SUKUIg6lI8SvNeG071dS75EoeTbLGx13QOSdG/pifUqTU1u59iSGcy4oP3Y="))
            {
                MessageBox.Show("Please run the License Key Wizard (Start - Luxand - FaceSDK - License Key Wizard)", "Error activating FaceSDK", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }

            FSDK.InitializeLibrary();
            FSDK.SetFaceDetectionParameters(true, true, 384);
        }
        List<double> noktaList;
        private void btnOpenPhoto_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    FSDK.CImage image = new FSDK.CImage(openFileDialog1.FileName);

                    // pencere genişliğine uyacak şekilde yeniden boyutlandırma
                    double ratio = System.Math.Min((pictureBox1.Width + 0.4) / image.Width,
                        (pictureBox1.Height + 0.4) / image.Height);
                    image = image.Resize(ratio);

                    Image frameImage = image.ToCLRImage();
                    Graphics gr = Graphics.FromImage(frameImage);

                    FSDK.TFacePosition facePosition = image.DetectFace();
                    if (0 == facePosition.w)
                        MessageBox.Show("No faces detected", "Face Detection");
                    else
                    {
                        int left = facePosition.xc - (int)(facePosition.w * 0.6f);
                        int top = facePosition.yc - (int)(facePosition.w * 0.5f);
                        gr.DrawRectangle(Pens.LightGreen, left, top, (int)(facePosition.w * 1.2), (int)(facePosition.w * 1.2));

                        FSDK.TPoint[] facialFeatures = image.DetectFacialFeaturesInRegion(ref facePosition);
                        int i = 0, j = 0;
                        for (int k = 0; k < 132; k = k + 2)
                        {
                            dizi2[k] = facialFeatures[j].x;
                            j++;
                        }
                        j = 0;
                        for (int t = 1; t < 132; t = t + 2)
                        {
                            dizi2[t] = facialFeatures[j].y;
                            j++;
                        }
                        noktaList = new List<double>();
                        foreach (FSDK.TPoint point in facialFeatures)
                        {
                            
                            gr.DrawEllipse((++i > 2) ? Pens.LightGreen : Pens.Blue, point.x, point.y, 3, 3);
                            pictureBox1.Image = frameImage;

                            noktaList.Add(point.x);
                            noktaList.Add(point.y);

                            listBox1.Items.Add(point.x);
                            listBox2.Items.Add(point.y);
                            list.Add(point.x.ToString() + "," + point.y.ToString());
                        }

                        gr.Flush();
                        // display image

                        pictureBox1.Refresh();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Exception");
                }
            }
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                string name = tbPersonName.Text;
                string klasor = cbSelect.Text.ToString();
                string path = @"C:\Users\Emre YILMAZ\Documents\Mimik Algılama\" + klasor + "\\" + name + ".txt";

                if (File.Exists(path))
                {
                    MessageBox.Show("Kaydedilmek istenen dosya bulunmakta");
                }
                else
                {
                    
                    using (System.IO.StreamWriter file =
    new System.IO.StreamWriter(path))
                    {
                        foreach (string line in list)
                        {

                            file.WriteLine(line);

                        }
                        MessageBox.Show("Resim Başarıyla Eklendi");
                        list.Clear();
                        tbPersonName.Clear();
                        listBox1.Items.Clear();
                        listBox2.Items.Clear();
                        pictureBox1.Image = null;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Exception");
            }
        }

        enum sonuc { igrenme, korku, mutlu, uzgun, kizgin, saskin, notr }

     //   double[] dizi;

        private void btnTest_Click(object sender, EventArgs e)
        {
           MessageBox.Show(((sonuc)j48.classify(noktaList.ToArray())).ToString());
        }
    }
}
