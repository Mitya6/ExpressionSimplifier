using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using ExpressionSimplifier;
using ExpressionSimplifier.Parse;
using ExpressionSimplifier.Pattern;

namespace WpfGui
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<ExpressionNode> expressions = new List<ExpressionNode>();

        public MainWindow()
        {
            InitializeComponent();

            ToolTipService.ShowDurationProperty.OverrideMetadata(typeof(DependencyObject), new FrameworkPropertyMetadata(Int32.MaxValue));
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                String path = "Input.txt";
                List<String> lines = LineByLineReader.ReadInput(path);
                foreach (String line in lines)
                {
                    expressions.Add(ExpressionParser.BuildTree(line));
                }

                this.lbExpressions.ItemsSource = this.expressions;

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

        private void FillTransformationsComboBox()
        {
            MethodInfo[] methodInfos = typeof(ExpressionSimplifier.ExpressionNode).GetMethods(
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            List<String> methodNames = new List<String>();
            foreach (MethodInfo mi in methodInfos)
            {
                methodNames.Add(mi.Name);
            }
            methodNames = methodNames.SkipWhile(item => item != "Raise").ToList();

            cbTransformations.ItemsSource = methodNames;
        }

        private void lbExpressions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.lbExpressions.SelectedIndex >= 0)
            {
                RefreshExpressionDetails(); 
            }
        }

        private void RefreshListbox(ExpressionNode newRoot)
        {
            if (this.lbExpressions.SelectedIndex >= 0)
            {
                int selectedIndex = this.lbExpressions.SelectedIndex;

                if (newRoot != null)
                {
                    ExpressionNode oldRoot = (ExpressionNode)this.lbExpressions.SelectedItem;
                    expressions.Insert(expressions.IndexOf(oldRoot), newRoot);
                    expressions.Remove(oldRoot);
                }

                this.lbExpressions.Items.Refresh();
                this.lbExpressions.SelectedIndex = selectedIndex;
            }
        }

        private void RefreshExpressionDetails()
        {
            ExpressionNode expr = lbExpressions.SelectedItem as ExpressionNode;

            if (expr != null)
            {
                ResetControls();

                this.tvTree.Items.Add(PopulateTreeView(expr));
                try
                {
                    this.tbDimension.Text = "Dimension: " + expr.GetDimension().ToString();
                    this.tbCost.Text = "Cost: " + expr.Cost().ToString();
                }
                catch (ApplicationException appEx)
                {
                    this.tbError.Text = appEx.Message;
                }
            }
        }

        private void ResetControls()
        {
            this.tvTree.Items.Clear();
            this.tbError.Text = "";
            this.tbDimension.Text = "";
            this.tbCost.Text = "";
        }

        private TreeViewItem PopulateTreeView(ExpressionNode expNode)
        {
            TreeViewItem tvItem = new TreeViewItem();

            // Create button to handle on click transformations for the node.
            Button button = new Button() { Content = expNode.DisplayName, Tag = expNode, ToolTip = expNode.ToString() };
            button.Click += (object sender, RoutedEventArgs e) =>
                {
                    ExpressionNode clickedNode = (ExpressionNode)(((Button)sender).Tag);
                    PerformTransformation(clickedNode);
                };
            tvItem.Header = button;

            // Populate child nodes.
            for (int i = 0; i < expNode.ChildrenCount(); i++)
            {
                tvItem.Items.Add(PopulateTreeView((ExpressionNode)(expNode.GetChild(i))));
            }

            tvItem.ExpandSubtree();
            return tvItem;
        }

        private void PerformTransformation(ExpressionNode clickedNode)
        {
            if (cbTransformations.SelectedItem == null) return;

            MethodInfo mi = typeof(ExpressionSimplifier.ExpressionNode).GetTypeInfo().GetDeclaredMethod(
                cbTransformations.SelectedItem.ToString());
            Object newRoot = mi.Invoke(clickedNode, null);

            RefreshListbox((ExpressionNode)newRoot);

            RefreshExpressionDetails();
        }

        private void btnApplyFirst_Click(object sender, RoutedEventArgs e)
        {
            if (cbTransformations.SelectedItem == null) return;

            ExpressionNode expr = (ExpressionNode)(lbExpressions.SelectedItem);

            String transformation = cbTransformations.SelectedItem.ToString();

            if (expr != null)
            {
                MethodInfo[] methodInfos = typeof(Pattern).GetMethods(
                BindingFlags.Public | BindingFlags.Static);
                MethodInfo methodInfo = methodInfos.FirstOrDefault(mi => mi.Name.Contains(transformation));

                if (methodInfo != null)
                {
                    Object newRoot = methodInfo.Invoke(null, new Object[] { expr });

                    RefreshListbox((ExpressionNode)newRoot);

                    RefreshExpressionDetails();
                }
            }
        }
    }
}
