using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace tts3
{
    public class GuiHandler
    {

        [DllImport("user32.dll")]
        static extern int GetForegroundWindow();

        [DllImport("user32.dll")]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        ContextMenu contextMenu1;

        private static int index, prevWindowHandle;

        public GuiHandler() {
            contextMenu1 = new ContextMenu();
            index = 0;
        }
        public void CreateTaskBarIcon() {

            NotifyIcon notifyIcon1 = new NotifyIcon(new System.ComponentModel.Container())
            {
                Icon = Properties.Resources.icon,
                ContextMenu = contextMenu1,
                Text = "ttsvoice is running...",
                Visible = true
            };
            
        }

        public void AddMenuOption(string name, EventHandler lambda) {
            MenuItem menuItem1 = new MenuItem
            {
                Index = index,
                Text = name,
            };

            menuItem1.Click += new EventHandler(lambda);

            contextMenu1.MenuItems.AddRange(new MenuItem[] { menuItem1 });

            index++;
        }
        public static string GetUserInput() {
            if (Application.OpenForms.Count > 0)
                return "";

            (Form prompt, TextBox text) = createForm();

            return prompt.ShowDialog() == DialogResult.OK ? text.Text : "";
        }

        public static (Form, TextBox) createForm() {
            Form prompt = new Form()
            {
                Width = 300,
                Height = 40,
                FormBorderStyle = FormBorderStyle.None,
                StartPosition = FormStartPosition.CenterScreen,
                BackColor = Color.Gray,
                Icon = Properties.Resources.icon
            };

            TextBox textBox = new TextBox()
            {
                Left = 1,
                Top = 1,
                Width = 298,
                BackColor = Color.Black,
                ForeColor = Color.Beige,
                Font = new Font(prompt.Font.FontFamily, 24)
            };


            prompt.Activated += (sender, e) =>
            {
                prevWindowHandle = GetForegroundWindow();
                prompt.WindowState = FormWindowState.Normal;
                textBox.Focus();
            };

            prompt.Deactivate += (sender, e) =>
            {
                SetForegroundWindow((IntPtr)prevWindowHandle);
            };

            prompt.Height = textBox.Height + 2;

            Button confirmation = new Button()
            {
                Text = "Ok",
                Left = 0,
                Width = 0,
                Top = 0,
                DialogResult = DialogResult.OK,
                Height = 0
            };

            confirmation.Click += (sender, e) => prompt.Close();
            prompt.Controls.Add(textBox);
            prompt.Controls.Add(confirmation);
            prompt.AcceptButton = prompt.CancelButton = confirmation;

            textBox.KeyPress += (sender, e) => {
                int txtw = TextRenderer.MeasureText(textBox.Text, textBox.Font).Width;
                if (txtw > textBox.Width)
                {
                    prompt.Width = txtw + 2;
                    textBox.Width = txtw;
                    prompt.Location = new Point((Screen.PrimaryScreen.WorkingArea.Width - prompt.Width) / 2,
                          (Screen.PrimaryScreen.WorkingArea.Height - prompt.Height) / 2);
                }
            };

            return (prompt, textBox);
        }
    }
}
