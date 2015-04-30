using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Runtime.InteropServices;

namespace ClipboardMonitor
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class Form1 : System.Windows.Forms.Form
	{

        ///
        /// 使用系統 kernel32.dll 進行轉換
        ///
        private const int LocaleSystemDefault = 0x0800;
        private const int LcmapSimplifiedChinese = 0x02000000;
        private NotifyIcon myNotify;
        private const int LcmapTraditionalChinese = 0x04000000;

        [DllImport("kernel32", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int LCMapString(int locale, int dwMapFlags, string lpSrcStr, int cchSrc,
                                              [Out] string lpDestStr, int cchDest);

        public static string ToSimplified(string argSource)
        {
            var t = new String(' ', argSource.Length);
            LCMapString(LocaleSystemDefault, LcmapSimplifiedChinese, argSource, argSource.Length, t, argSource.Length);
            return t;
        }

        public static string ToTraditional(string argSource)
        {
            var t = new String(' ', argSource.Length);
            LCMapString(LocaleSystemDefault, LcmapTraditionalChinese, argSource, argSource.Length, t, argSource.Length);
            return t;
        }



















		[DllImport("User32.dll")]
		protected static extern int SetClipboardViewer(int hWndNewViewer);

		[DllImport("User32.dll", CharSet=CharSet.Auto)]
		public static extern bool ChangeClipboardChain(IntPtr hWndRemove, IntPtr hWndNewNext);

		[DllImport("user32.dll", CharSet=CharSet.Auto)]
		public static extern int SendMessage(IntPtr hwnd, int wMsg, IntPtr wParam, IntPtr lParam);

		private System.Windows.Forms.RichTextBox richTextBox1;


        IntPtr nextClipboardViewer;
        private IContainer components;

		public Form1()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			nextClipboardViewer = (IntPtr)SetClipboardViewer((int) this.Handle);
            richTextBox1.Clear();
			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			ChangeClipboardChain(this.Handle, nextClipboardViewer);
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.myNotify = new System.Windows.Forms.NotifyIcon(this.components);
            this.SuspendLayout();
            // 
            // richTextBox1
            // 
            this.richTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.richTextBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBox1.Location = new System.Drawing.Point(0, 0);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(510, 421);
            this.richTextBox1.TabIndex = 0;
            this.richTextBox1.Text = "richTextBox1";
            this.richTextBox1.WordWrap = false;
            // 
            // myNotify
            // 
            this.myNotify.Icon = ((System.Drawing.Icon)(resources.GetObject("myNotify.Icon")));
            this.myNotify.Text = "ClipboardChineseTranslator";
            this.myNotify.Visible = true;
            this.myNotify.DoubleClick += new System.EventHandler(this.myNotify_DoubleClick);
            // 
            // Form1
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(6, 18);
            this.ClientSize = new System.Drawing.Size(510, 421);
            this.Controls.Add(this.richTextBox1);
            this.Name = "Form1";
            this.Text = "Clipboard Monitor Example";
            this.Resize += new System.EventHandler(this.Form1_Resize);
            this.ResumeLayout(false);

		}
		#endregion

		/// <summary>

		/// The main entry point for the application.
        /// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new Form1());
		}

		protected override void WndProc(ref System.Windows.Forms.Message m)
		{
			// defined in winuser.h
			const int WM_DRAWCLIPBOARD = 0x308;
			const int WM_CHANGECBCHAIN = 0x030D;

			switch(m.Msg)
			{
				case WM_DRAWCLIPBOARD:
					DisplayClipboardData();
					SendMessage(nextClipboardViewer, m.Msg, m.WParam, m.LParam);
					break;

				case WM_CHANGECBCHAIN:
					if (m.WParam == nextClipboardViewer)
						nextClipboardViewer = m.LParam;
					else
						SendMessage(nextClipboardViewer, m.Msg, m.WParam, m.LParam);
					break;

				default:
					base.WndProc(ref m);
					break;
			}	
		}

		void DisplayClipboardData()		
		{
			try
			{
                //richTextBox1.LoadFile(@"C:\users\nherre\desktop\test.rtf");
                //IDataObject iData = new DataObject();  
				//iData = Clipboard.GetDataObject();

                string original = Clipboard.GetText();
                Clipboard.SetText(ToTraditional(original));
                richTextBox1.Clear();
                richTextBox1.AppendText("\r\n");
                richTextBox1.Paste();
                //richTextBox1.SaveFile(@"C:\users\nherre\desktop\test.rtf");

                #region IF Logic
                //If logic
                //string[] formatsReturned = Clipboard.GetDataObject().GetFormats();
                //for (int i = 0; i < formatsReturned.Length; i++)
                //{
                //    richTextBox1.AppendText(formatsReturned[i]+"\r\n");
                //}

                //if (iData.GetDataPresent(DataFormats.Rtf))
                //{
                //    richTextBox1.Paste (DataFormats.GetFormat(DataFormats.Rtf));
                //}
                //else if (iData.GetDataPresent(DataFormats.Text))
                //{
                //    richTextBox1.Paste(DataFormats.GetFormat(DataFormats.Text));
                //}
                //else if (iData.GetDataPresent(DataFormats.Bitmap))
                //{
                //    richTextBox1.Paste();//DataFormats.GetFormat(DataFormats.Bitmap));
                //}
                //else if (iData.GetDataPresent(DataFormats.FileDrop))
                //{
                //    //richTextBox1
                //    richTextBox1.Paste(); //(DataFormats.GetFormat(DataFormats.FileDrop));
                //}
                //else
                //{
                //    richTextBox1.Text = "[Clipboard data is not RTF or ASCII Text or a Bitmap]";
                //}
#endregion
            }
			catch(Exception e)
			{
				MessageBox.Show(e.ToString());
			}
		}

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (FormWindowState.Minimized == WindowState)
                Hide();
        }

        private void myNotify_DoubleClick(object sender, EventArgs e)
        {
            Show();
            WindowState = FormWindowState.Normal;
        }
	}
}
