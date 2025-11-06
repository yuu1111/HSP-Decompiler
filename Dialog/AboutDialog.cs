using System.Drawing;
using System.Windows.Forms;
using System.Reflection;

namespace KttK.HspDecompiler
{
    /// <summary>
    /// AboutDialog の概要の説明です。
    /// </summary>
    internal sealed class AboutDialog : Form
    {
        private PictureBox pictureBox1;
        private Label LB_Title;
        private Label LB_Copyright;
        /// <summary>
        /// 必要なデザイナ変数です。
        /// </summary>
        private System.ComponentModel.Container components = null;

        internal AboutDialog()
        {
            //
            // Windows フォーム デザイナ サポートに必要です。
            //
            InitializeComponent();
            Assembly mainAssembly = Assembly.GetEntryAssembly();
            string appCopyright = "-";
            object[] CopyrightArray =
                mainAssembly.GetCustomAttributes(
                    typeof(AssemblyCopyrightAttribute), false);
            if ((CopyrightArray != null) && (CopyrightArray.Length > 0))
            {
                appCopyright =
                    ((AssemblyCopyrightAttribute)CopyrightArray[0]).Copyright;
            }

            LB_Copyright.Text = appCopyright;
            //
            // TODO: InitializeComponent 呼び出しの後に、コンストラクタ コードを追加してください。
            //
        }

        /// <summary>
        /// 使用されているリソースに後処理を実行します。
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }

            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナで生成されたコード
        /// <summary>
        /// デザイナ サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディタで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutDialog));
            pictureBox1 = new PictureBox();
            LB_Title = new Label();
            LB_Copyright = new Label();
            ((System.ComponentModel.ISupportInitialize)(pictureBox1)).BeginInit();
            SuspendLayout();
            // 
            // pictureBox1
            // 
            pictureBox1.Image = ((Image)(resources.GetObject("pictureBox1.Image")));
            pictureBox1.Location = new Point(12, 12);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(32, 32);
            pictureBox1.TabIndex = 4;
            pictureBox1.TabStop = false;
            // 
            // LB_Title
            // 
            LB_Title.Font = new Font("MS UI Gothic", 9F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(128)));
            LB_Title.Location = new Point(50, 12);
            LB_Title.Name = "LB_Title";
            LB_Title.Size = new Size(172, 16);
            LB_Title.TabIndex = 5;
            LB_Title.Text = "フリー HSP逆コンパイラ　Ver 1.20";
            // 
            // LB_Copyright
            // 
            LB_Copyright.AutoSize = true;
            LB_Copyright.Location = new Point(66, 37);
            LB_Copyright.Name = "LB_Copyright";
            LB_Copyright.Size = new Size(11, 12);
            LB_Copyright.TabIndex = 6;
            LB_Copyright.Text = "-";
            // 
            // AboutDialog
            // 
            AutoScaleBaseSize = new Size(5, 12);
            ClientSize = new Size(240, 63);
            Controls.Add(LB_Copyright);
            Controls.Add(LB_Title);
            Controls.Add(pictureBox1);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            ImeMode = ImeMode.Off;
            MaximizeBox = false;
            MaximumSize = new Size(246, 90);
            MinimizeBox = false;
            MinimumSize = new Size(246, 90);
            Name = "AboutDialog";
            ShowIcon = false;
            ShowInTaskbar = false;
            Text = "バージョン情報";
            TopMost = true;
            Load += new System.EventHandler(RegistDialog_Load);
            ((System.ComponentModel.ISupportInitialize)(pictureBox1)).EndInit();
            ResumeLayout(false);
            PerformLayout();

        }
        #endregion

        private void buttonOK_Click(object sender, System.EventArgs e)
        {
        }

        private void buttonCancel_Click(object sender, System.EventArgs e)
        {
            Close();
        }


        private void RegistDialog_Load(object sender, System.EventArgs e)
        {
            Location = initialLocation;
        }

        private Point initialLocation;

        internal void SetInitialLocation(Form frm)
        {
            initialLocation = frm.Location;
            initialLocation.Offset(frm.Width / 2, frm.Height / 2);
            initialLocation.Offset(-Width / 2, -Height / 2);
        }
    }
}
