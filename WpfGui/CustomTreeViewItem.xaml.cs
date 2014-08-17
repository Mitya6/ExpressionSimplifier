using System;
using System.Collections.Generic;
using System.Linq;
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

namespace WpfGui
{
    /// <summary>
    /// Interaction logic for CustomTreeViewItem.xaml
    /// </summary>
    public partial class CustomTreeViewItem : TreeViewItem
    {
        public CustomTreeViewItem()
        {
            InitializeComponent();
        }

        protected override void OnRender(DrawingContext dc)
        {
            Rectangle verLn = (Rectangle)this.Template.FindName("VerLn", this);

            if (this.Items.Count > 0)
            {
                CustomTreeViewItem lastItem = (CustomTreeViewItem)this.Items[this.Items.Count - 1];

                Point p1 = lastItem.TransformToVisual(this)
                          .Transform(new Point(0, 0));
                verLn.Height = p1.Y - 19/2.0 - 1;
            }

            base.OnRender(dc);            
        } 
    }
}
