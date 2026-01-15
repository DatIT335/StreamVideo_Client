namespace StreamVideo_Client.WinForms
{
    partial class FormConnect
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblIP;
        private System.Windows.Forms.Label lblPort;
        private System.Windows.Forms.Label lblUser;
        private System.Windows.Forms.Label lblPass;

        private System.Windows.Forms.TextBox txtIP;
        private System.Windows.Forms.TextBox txtPort;
        private System.Windows.Forms.TextBox txtUser;
        private System.Windows.Forms.TextBox txtPass;

        private System.Windows.Forms.Button btnLogin;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();

            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            lblTitle = new System.Windows.Forms.Label();
            lblIP = new System.Windows.Forms.Label();
            lblPort = new System.Windows.Forms.Label();
            lblUser = new System.Windows.Forms.Label();
            lblPass = new System.Windows.Forms.Label();

            txtIP = new System.Windows.Forms.TextBox();
            txtPort = new System.Windows.Forms.TextBox();
            txtUser = new System.Windows.Forms.TextBox();
            txtPass = new System.Windows.Forms.TextBox();

            btnLogin = new System.Windows.Forms.Button();

            SuspendLayout();

            // lblTitle
            lblTitle.AutoSize = true;
            lblTitle.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            lblTitle.Location = new System.Drawing.Point(80, 20);
            lblTitle.Text = "CLIENT ĐĂNG NHẬP";

            // lblIP
            lblIP.Location = new System.Drawing.Point(30, 70);
            lblIP.Text = "IP Server:";

            // txtIP
            txtIP.Location = new System.Drawing.Point(150, 70);
            txtIP.Size = new System.Drawing.Size(180, 23);
            txtIP.Text = "127.0.0.1";

            // lblPort
            lblPort.Location = new System.Drawing.Point(30, 110);
            lblPort.Text = "Port:";

            // txtPort
            txtPort.Location = new System.Drawing.Point(150, 110);
            txtPort.Size = new System.Drawing.Size(180, 23);
            txtPort.Text = "9000";

            // lblUser
            lblUser.Location = new System.Drawing.Point(30, 150);
            lblUser.Text = "Tên đăng nhập:";

            // txtUser
            txtUser.Location = new System.Drawing.Point(150, 150);
            txtUser.Size = new System.Drawing.Size(180, 23);

            // lblPass
            lblPass.Location = new System.Drawing.Point(30, 190);
            lblPass.Text = "Mật khẩu:";

            // txtPass
            txtPass.Location = new System.Drawing.Point(150, 190);
            txtPass.Size = new System.Drawing.Size(180, 23);
            txtPass.UseSystemPasswordChar = true;

            // btnLogin
            btnLogin.Location = new System.Drawing.Point(150, 235);
            btnLogin.Size = new System.Drawing.Size(100, 30);
            btnLogin.Text = "Đăng nhập";
            btnLogin.Click += btnLogin_Click;

            // FormConnect
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(380, 300);
            Controls.Add(lblTitle);
            Controls.Add(lblIP);
            Controls.Add(txtIP);
            Controls.Add(lblPort);
            Controls.Add(txtPort);
            Controls.Add(lblUser);
            Controls.Add(txtUser);
            Controls.Add(lblPass);
            Controls.Add(txtPass);
            Controls.Add(btnLogin);

            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "Client - Đăng nhập";

            ResumeLayout(false);
            PerformLayout();
        }
    }
}
