using System.Windows.Forms;
using System.Drawing;
using System;

namespace BWM.LIB
{
    class InputBox
    {
        public static DialogResult BoxTwoButton(string title, string promptText, string firstName, string secondName, ref string value)
        {
            Form form = new Form();
            Label label = new Label();
            TextBox textBox = new TextBox();
            Button btnFirst = new Button();
            Button btnSecond = new Button();

            form.Text = title;
            label.Text = promptText;
            textBox.Text = "";
            textBox.UseSystemPasswordChar = true;
            textBox.PasswordChar = '*';

            btnFirst.Text = firstName;
            btnSecond.Text = secondName;
            btnFirst.DialogResult = DialogResult.OK;
            btnSecond.DialogResult = DialogResult.Cancel;

            label.SetBounds(9, 20, 372, 13);
            textBox.SetBounds(12, 36, 372, 20);
            btnFirst.SetBounds(228, 72, 75, 23);
            btnSecond.SetBounds(309, 72, 75, 23);

            label.AutoSize = true;
            textBox.Anchor = textBox.Anchor | AnchorStyles.Right;
            btnFirst.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnSecond.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

            form.ClientSize = new Size(396, 107);
            form.Controls.AddRange(new Control[] { label, textBox, btnFirst, btnSecond });
            form.ClientSize = new Size(Math.Max(300, label.Right + 10), form.ClientSize.Height);
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.StartPosition = FormStartPosition.CenterScreen;
            form.MinimizeBox = false;
            form.MaximizeBox = false;
            form.AcceptButton = btnFirst;
            form.CancelButton = btnSecond;

            DialogResult dialogResult = form.ShowDialog();
            value = textBox.Text;
            return dialogResult;
        }
    }
}
