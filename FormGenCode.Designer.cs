namespace BWM
{
    partial class FormGenCode
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.btnGen = new System.Windows.Forms.Button();
            this.tbSource = new System.Windows.Forms.TextBox();
            this.tbResultClass = new System.Windows.Forms.TextBox();
            this.tbResultSQL = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.tbResultStore = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Source:";
            // 
            // btnGen
            // 
            this.btnGen.Location = new System.Drawing.Point(991, 4);
            this.btnGen.Name = "btnGen";
            this.btnGen.Size = new System.Drawing.Size(75, 23);
            this.btnGen.TabIndex = 1;
            this.btnGen.Text = "Gen";
            this.btnGen.UseVisualStyleBackColor = true;
            this.btnGen.Click += new System.EventHandler(this.btnGen_Click);
            // 
            // tbSource
            // 
            this.tbSource.Location = new System.Drawing.Point(12, 33);
            this.tbSource.Multiline = true;
            this.tbSource.Name = "tbSource";
            this.tbSource.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tbSource.Size = new System.Drawing.Size(335, 498);
            this.tbSource.TabIndex = 2;
            // 
            // tbResultClass
            // 
            this.tbResultClass.Location = new System.Drawing.Point(388, 33);
            this.tbResultClass.Multiline = true;
            this.tbResultClass.Name = "tbResultClass";
            this.tbResultClass.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tbResultClass.Size = new System.Drawing.Size(330, 498);
            this.tbResultClass.TabIndex = 3;
            // 
            // tbResultSQL
            // 
            this.tbResultSQL.Location = new System.Drawing.Point(724, 33);
            this.tbResultSQL.Multiline = true;
            this.tbResultSQL.Name = "tbResultSQL";
            this.tbResultSQL.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tbResultSQL.Size = new System.Drawing.Size(330, 243);
            this.tbResultSQL.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(385, 14);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(46, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Classes:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(721, 14);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(46, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Queries:";
            // 
            // tbResultStore
            // 
            this.tbResultStore.Location = new System.Drawing.Point(724, 288);
            this.tbResultStore.Multiline = true;
            this.tbResultStore.Name = "tbResultStore";
            this.tbResultStore.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tbResultStore.Size = new System.Drawing.Size(330, 243);
            this.tbResultStore.TabIndex = 7;
            // 
            // FormGenCode
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1078, 543);
            this.Controls.Add(this.tbResultStore);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.tbResultSQL);
            this.Controls.Add(this.tbResultClass);
            this.Controls.Add(this.tbSource);
            this.Controls.Add(this.btnGen);
            this.Controls.Add(this.label1);
            this.Name = "FormGenCode";
            this.Text = "FormGenCode";
            this.Load += new System.EventHandler(this.FormGenCode_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnGen;
        private System.Windows.Forms.TextBox tbSource;
        private System.Windows.Forms.TextBox tbResultClass;
        private System.Windows.Forms.TextBox tbResultSQL;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbResultStore;
    }
}