using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Threading.Tasks;


namespace CartiHub
{
    public partial class MacroForm : Form
    {
        private bool isRecording = false;
        private List<string> recordedKeys = new List<string>();
        private DateTime startTime;
        private bool isPlayingMacro = false;


        public MacroForm()
        {
            InitializeComponent();
            this.KeyPreview = true;
            this.KeyDown += new KeyEventHandler(Form1_KeyDown);
        }


        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void siticoneRoundedButton3_Click(object sender, EventArgs e)
        {
            if (!isRecording)
            {
                isRecording = true;
                recordedKeys.Clear(); 
                startTime = DateTime.Now;
                RecordMacroButton.Text = "Recording Macro. Click to Stop";
                MessageBox.Show("Recording started. Press 'Stop' to save.");
            }
            else
            {
                isRecording = false;
                SaveToFile();
                RecordMacroButton.Text = "Record Macro";
                MessageBox.Show("Recording stopped. Keys saved to file.");
            }
        }

        private List<Keys> recordedMacro = new List<Keys>();

        private async void siticoneRoundedButton1_Click(object sender, EventArgs e)
        {
            if (!isPlayingMacro)
            {
                isPlayingMacro = true;
                StartMacroButton.Text = "Playing Macro. Click to Stop";

                if (recordedMacro.Count > 0)
                {
                    await Task.Run(() =>
                    {
                        foreach (Keys key in recordedMacro)
                        {
                            if (!isPlayingMacro)
                            {
                                break; 
                            }

                            SendKeys.SendWait(key.ToString());
                            System.Threading.Thread.Sleep(100);
                        }
                    });
                }
                else
                {
                    MessageBox.Show("No macro recorded yet.");
                }

                isPlayingMacro = false; 
                StartMacroButton.Text = "Click to Play Macro";
            }
            else
            {
                isPlayingMacro = false;
                StartMacroButton.Text = "Click to Play Macro";
            }
        }


        private void siticoneRoundedButton2_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text Files (*.txt)|*.txt";
            openFileDialog.Title = "Open Recorded Keys File";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string[] lines = File.ReadAllLines(openFileDialog.FileName);

                    foreach (string line in lines)
                    {

                        string[] parts = line.Split(new string[] { " - Time: " }, StringSplitOptions.None);
                        string keyString = parts[0]; 

                        if (Enum.TryParse(keyString, out Keys key))
                        {
                            recordedMacro.Add(key);
                        }
                    }

                    if (recordedMacro.Count > 0)
                    {
                        foreach (Keys key in recordedMacro)
                        {
                            SendKeys.SendWait(key.ToString());
                            System.Threading.Thread.Sleep(100);
                        }
                    }
                    else
                    {
                        MessageBox.Show("No macro recorded in the file.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error opening file: " + ex.Message);
                }
            }
        }


        private void siticoneRoundedButton4_Click(object sender, EventArgs e)
        {
          this.WindowState = FormWindowState.Minimized;
        }

        private void siticoneRoundedButton5_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void CurrentMacroLabel_Click(object sender, EventArgs e)
        {

        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (isRecording)
            {
                TimeSpan elapsedTime = DateTime.Now - startTime;
                string keyInfo = $"{e.KeyCode.ToString()} - Time: {elapsedTime.TotalSeconds} seconds";
                recordedKeys.Add(keyInfo);
            }
        }

        private void SaveToFile()
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "Text Files (*.txt)|*.txt";
                saveFileDialog.Title = "Save Recorded Keys";
                saveFileDialog.ShowDialog();

                if (saveFileDialog.FileName != "")
                {
                    try
                    {
                        using (StreamWriter writer = new StreamWriter(saveFileDialog.FileName))
                        {
                            foreach (string keyInfo in recordedKeys)
                            {
                                writer.WriteLine(keyInfo);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error saving file: " + ex.Message);
                    }
                }
            }
        }
    }
}
