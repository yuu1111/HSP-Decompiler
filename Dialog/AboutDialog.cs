using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace KttK.HspDecompiler
{
    /// <summary>
    /// AboutDialog の概要の説明です。.
    /// </summary>
    internal sealed class AboutDialog : Form
    {
        private PictureBox pictureBox1;
        private Label lBTitle;
        private Label lBCopyright;

        /// <summary>
        /// 必要なデザイナ変数です。.
        /// </summary>
        private System.ComponentModel.Container components = null;

        internal AboutDialog()
        {
            // Windows フォーム デザイナ サポートに必要です。
            this.InitializeComponent();
            Assembly mainAssembly = Assembly.GetEntryAssembly();
            string appCopyright = "-";
            object[] copyrightArray =
                mainAssembly.GetCustomAttributes(
                    typeof(AssemblyCopyrightAttribute), false);
            if ((copyrightArray != null) && (copyrightArray.Length > 0))
            {
                appCopyright =
                    ((AssemblyCopyrightAttribute)copyrightArray[0]).Copyright;
            }

            this.lBCopyright.Text = appCopyright;

            // TODO: InitializeComponent 呼び出しの後に、コンストラクタ コードを追加してください。
        }

        /// <summary>
        /// 使用されているリソースに後処理を実行します。.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.components != null)
                {
                    this.components.Dispose();
                }
            }

            base.Dispose(disposing);
        }
        /// <summary>
        /// デザイナ サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディタで変更しないでください。.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutDialog));
            this.pictureBox1 = new PictureBox();
            this.lBTitle = new Label();
            this.lBCopyright = new Label();
            ((System.ComponentModel.ISupportInitialize)this.pictureBox1).BeginInit();
            this.SuspendLayout();

            // pictureBox1
            this.pictureBox1.Image =  (Image)resources.GetObject("pictureBox1.Image");
            this.pictureBox1.Location = new Point(12, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new Size(32, 32);
            this.pictureBox1.TabIndex = 4;
            this.pictureBox1.TabStop = false;

            // LB_Title
            this.lBTitle.Font = new Font("MS UI Gothic", 9F, FontStyle.Regular, GraphicsUnit.Point,  (byte)128);
            this.lBTitle.Location = new Point(50, 12);
            this.lBTitle.Name = "LB_Title";
            this.lBTitle.Size = new Size(172, 16);
            this.lBTitle.TabIndex = 5;
            this.lBTitle.Text = "フリー HSP逆コンパイラ　Ver 1.20";

            // LB_Copyright
            this.lBCopyright.AutoSize = true;
            this.lBCopyright.Location = new Point(66, 37);
            this.lBCopyright.Name = "LB_Copyright";
            this.lBCopyright.Size = new Size(11, 12);
            this.lBCopyright.TabIndex = 6;
            this.lBCopyright.Text = "-";

            // AboutDialog
            this.AutoScaleBaseSize = new Size(5, 12);
            this.ClientSize = new Size(240, 63);
            this.Controls.Add(this.lBCopyright);
            this.Controls.Add(this.lBTitle);
            this.Controls.Add(this.pictureBox1);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.ImeMode = ImeMode.Off;
            this.MaximizeBox = false;
            this.MaximumSize = new Size(246, 90);
            this.MinimizeBox = false;
            this.MinimumSize = new Size(246, 90);
            this.Name = "AboutDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "バージョン情報";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.RegistDialog_Load);
            ((System.ComponentModel.ISupportInitialize)this.pictureBox1).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private void ButtonOK_Click(object sender, System.EventArgs e)
        {
        }

        private void ButtonCancel_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }

        private void RegistDialog_Load(object sender, System.EventArgs e)
        {
            this.Location = this.initialLocation;
        }

        private Point initialLocation;

        internal void SetInitialLocation(Form frm)
        {
            this.initialLocation = frm.Location;
            this.initialLocation.Offset(frm.Width / 2, frm.Height / 2);
            this.initialLocation.Offset(-this.Width / 2, -this.Height / 2);
        }
    }
}
