using System;
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;

namespace screenSavingCreator
{
    public partial class ScreensaverForm : Form
    {
        private Image img;
        private float zoom = 0.95f;        // initial zoom (95%)
        private float zoomSpeed = 0.0003f; // per tick
        private float driftX = 0.2f;       // pixels per tick
        private float driftY = 0.15f;
        private float offsetX = 0;
        private float offsetY = 0;

        private Timer timer;

        public ScreensaverForm()
        {
            InitializeComponent();
            LoadImage();
            SetupForm();
            SetupTimer();
        }

        public ScreensaverForm(IntPtr previewHandle)
        {
            InitializeComponent();
            LoadImage();

            // Use the small preview window instead of full screen
            this.FormBorderStyle = FormBorderStyle.None;
            this.TopMost = false;

            // Attach to preview panel
            NativeMethods.SetParent(this.Handle, previewHandle);

            // Make it fill the preview window
            Rectangle parentRect;
            NativeMethods.GetClientRect(previewHandle, out parentRect);
            this.Size = new Size(parentRect.Width, parentRect.Height);

            SetupPreviewTimer();

        }




        private void LoadImage()
        {
            // Load embedded resource image
            var assembly = Assembly.GetExecutingAssembly();

            var names = assembly.GetManifestResourceNames();
            MessageBox.Show(string.Join("\n", names));

            img = Image.FromStream(
                assembly.GetManifestResourceStream("screenSavingCreator.background.jpg")
            );
        }

        private void SetupForm()
        {
            Cursor.Hide();
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;
            this.TopMost = true;

            // Exit events
            this.KeyDown += (s, e) => Close();
            this.MouseMove += (s, e) => Close();
            this.MouseClick += (s, e) => Close();
        }

        private void SetupTimer()
        {
            timer = new Timer();
            timer.Interval = 16; // ~60 FPS
            timer.Tick += (s, e) =>
            {
                zoom += zoomSpeed;
                offsetX += driftX;
                offsetY += driftY;
                Invalidate();
            };
            timer.Start();
        }

        private void SetupPreviewTimer()
        {
            timer = new Timer();
            timer.Interval = 33; // 30fps is enough
            timer.Tick += (s, e) =>
            {
                zoom += zoomSpeed;
                offsetX += driftX;
                offsetY += driftY;
                Invalidate();
            };
            timer.Start();
        }

        internal static class NativeMethods
        {
            [System.Runtime.InteropServices.DllImport("user32.dll")]
            public static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

            [System.Runtime.InteropServices.DllImport("user32.dll")]
            public static extern bool GetClientRect(IntPtr hWnd, out Rectangle lpRect);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            var g = e.Graphics;
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

            float screenW = this.Width;
            float screenH = this.Height;
            float imgW = img.Width * zoom;
            float imgH = img.Height * zoom;

            float scale = Math.Min(screenW / imgW, screenH / imgH);
            float drawW = imgW * scale;
            float drawH = imgH * scale;

            float x = (screenW - drawW) / 2 + offsetX;
            float y = (screenH - drawH) / 2 + offsetY;

            g.DrawImage(img, x, y, drawW, drawH);
        }
    }
}
