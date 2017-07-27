using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MinimalStudio
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Code_TextBox_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                Code_TextBox.FontSize += Code_TextBox.FontSize <= 1 ? 1 : e.Delta > 0 ? 1 : -1;
            }
        }

        private void Compile_Button_Click(object sender, RoutedEventArgs e)
        {
            CompileAndRunCode(Code_TextBox.Text);
        }

        private void CompileAndRunCode(string code)
        {
            CSharpCodeProvider provider = new CSharpCodeProvider();
            CompilerParameters parameters = new CompilerParameters();

            string output = "SimpleStudioGo.exe";

            // Add some libs
            parameters.ReferencedAssemblies.Add("mscorlib.dll");
            parameters.ReferencedAssemblies.Add("System.dll");
            parameters.ReferencedAssemblies.Add("System.Data.dll");
            parameters.ReferencedAssemblies.Add("System.Core.dll");
            parameters.ReferencedAssemblies.Add("System.Drawing.dll");
            parameters.ReferencedAssemblies.Add("System.Web.dll");
            parameters.ReferencedAssemblies.Add("System.Xml.dll");
            parameters.ReferencedAssemblies.Add("System.Windows.Forms.dll");

            // True - memory generation, false - external file generation
            parameters.GenerateInMemory = false;
            // True - exe file generation, false - dll file generation
            parameters.GenerateExecutable = true;

            parameters.OutputAssembly = output;

            CompilerResults results = provider.CompileAssemblyFromSource(parameters, code);

            if (results.Errors.HasErrors)
            {
                StringBuilder sb = new StringBuilder();

                foreach (CompilerError error in results.Errors)
                {
                    sb.AppendLine(String.Format("Error ({0}): {1}", error.ErrorNumber, error.ErrorText));
                }
                
                MessageBox.Show(sb.ToString());
            }
            else
            {
                Process.Start(output);
            }

        }

        private void Code_TextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.Enter) CompileAndRunCode(Code_TextBox.Text);
        }
    }

    public class BetterTextBox : TextBox
    {
        public BetterTextBox()
        {
            //Defaults to 4
            TabSize = 4;
        }

        public int TabSize
        {
            get;
            set;
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Tab)
            {
                String tab = new String(' ', TabSize);

                if (base.SelectedText != string.Empty)
                {
                    if (Keyboard.Modifiers == ModifierKeys.Shift)
                    {
                        if (base.SelectedText.StartsWith(tab)) base.SelectedText = base.SelectedText.Substring(TabSize);
                        base.SelectedText = base.SelectedText.Replace(System.Environment.NewLine + tab, System.Environment.NewLine);
                    }
                    else
                    {
                        if (base.SelectionStart == 0) base.SelectedText = tab + base.SelectedText;
                        base.SelectedText = base.SelectedText.Replace(System.Environment.NewLine, System.Environment.NewLine + tab);
                    }

                    e.Handled = true;
                }
                else
                {
                    int caretPosition = base.CaretIndex;

                    if (Keyboard.Modifiers == ModifierKeys.Shift)
                    {
                        if (base.Text.Substring(0, caretPosition).EndsWith(tab))
                        {
                            base.Text = base.Text.Remove(caretPosition - TabSize, TabSize);
                            base.CaretIndex = caretPosition - TabSize;
                        }
                    }
                    else
                    {
                        base.Text = base.Text.Insert(caretPosition, tab);
                        base.CaretIndex = caretPosition + TabSize;
                    }
                    
                    e.Handled = true;
                }
            }
        }
    }
}
