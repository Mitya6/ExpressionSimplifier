using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using ExpressionSimplifier;
using ExpressionSimplifier.Parse;

namespace WpfGui
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<ExpressionSimplifier.Expression> expressions;

        public MainWindow()
        {
            InitializeComponent();
            this.expressions = new List<ExpressionSimplifier.Expression>();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                String path = "Input.txt";
                List<String> lines = LineByLineReader.ReadInput(path);
                foreach (String line in lines)
                {
                    this.expressions.Add(new ExpressionSimplifier.Expression(
                        line, ExpressionParser.BuildTree(line)));
                }
                lbExpressions.ItemsSource = this.expressions;

                FillTransformationsComboBox();
            }
            catch (Exception ex)
            {
                // Workaround
                // 64bit machines are unable to properly throw the errors during a Window_Loaded event.
                // http://stackoverflow.com/questions/4807122/wpf-showdialog-swallowing-exceptions-during-window-load
                BackgroundWorker loaderExceptionWorker = new BackgroundWorker();
                loaderExceptionWorker.DoWork += ((exceptionWorkerSender, runWorkerCompletedEventArgs) => { runWorkerCompletedEventArgs.Result = runWorkerCompletedEventArgs.Argument; });
                loaderExceptionWorker.RunWorkerCompleted += ((exceptionWorkerSender, runWorkerCompletedEventArgs) => { throw (Exception)runWorkerCompletedEventArgs.Result; });
                loaderExceptionWorker.RunWorkerAsync(ex);
            }
        }

        private void lbExpressions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateTreeView();
        }

        private void UpdateTreeView()
        {
            ExpressionSimplifier.Expression expr = lbExpressions.SelectedItem
                as ExpressionSimplifier.Expression;

            if (expr != null)
            {
                ResetControls();
                if (expr.Root != null)
                {
                    this.tvTree.Items.Add(PopulateTreeView(expr.Root));
                }
                else
                {
                    this.tbError.Text = "Error!";
                }
            }
        }

        private void FillTransformationsComboBox()
        {
            MethodInfo[] methodInfos = typeof(ExpressionSimplifier.Expression).GetMethods(
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            List<String> methodNames = new List<String>();
            foreach (MethodInfo mi in methodInfos)
            {
                methodNames.Add(mi.Name);
            }
            methodNames = methodNames.SkipWhile(item => item != "DeleteZeroOrEmptyAddition").ToList();

            cbTransformations.ItemsSource = methodNames;
        }

        private void ResetControls()
        {
            this.tvTree.Items.Clear();
            this.tbError.Text = "";
        }

        private TreeViewItem PopulateTreeView(TreeNode treeNode)
        {
            TreeViewItem tvItem = new TreeViewItem();

            Button button = new Button() { Content = treeNode };
            button.Click += (object sender, RoutedEventArgs e) =>
                {
                    TreeNode clickedNode = (TreeNode)(((Button)sender).Content);
                    PerformTransformation(clickedNode);

                    UpdateTreeView();
                };
            tvItem.Header = button;
            
            for (int i = 0; i < treeNode.ChildrenCount(); i++)
            {
                tvItem.Items.Add(PopulateTreeView(treeNode.GetChild(i)));
            }

            tvItem.ExpandSubtree();
            return tvItem;
        }

        private void PerformTransformation(TreeNode clickedNode)
        {
            if (cbTransformations.SelectedItem == null) return;

            MethodInfo mi = typeof(ExpressionSimplifier.Expression).GetTypeInfo().GetDeclaredMethod(
                cbTransformations.SelectedItem.ToString());
            mi.Invoke(clickedNode.Expr, new Object[] { clickedNode });
        }
    }
}
