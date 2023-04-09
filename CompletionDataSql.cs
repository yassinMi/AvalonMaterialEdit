using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace AvalonMaterialEdit
{
   
    public class CompletionDataSql : ICompletionData
    {

        public static Func<object, CompletionDataSql> GetCompletionDataSqlFromFieldDescriptor { get; set; }

        /// <summary>
        /// use this ctro to make a sqlite keyword item
        /// </summary>
        /// <param name="text"></param>
        public CompletionDataSql(string text)
        {
            this.Text = text;
            TextBlock tb = new TextBlock(new Run(this.Text));
            tb.Margin = new Thickness(4, 4, 4, 4);

            TextOptions.SetTextFormattingMode(tb, TextFormattingMode.Display);
            //tb.Foreground = (SolidColorBrush)App.Current.FindResource("PrimaryHueMidBrush");
            tb.FontFamily = new FontFamily("consolas");
            this.Content = tb;
        }
        public static CompletionDataSql fromToken(object obj)
        {
            if (obj is string)
            {
                return new CompletionDataSql((string)obj);
            }
            else if (obj is object)
            {
                return GetCompletionDataSqlFromFieldDescriptor?.Invoke(obj)??new CompletionDataSql(obj) { };

            }
            return null;

        }
        /// <summary>
        /// use this ctro to make a table field item as a fall back when the consumer supplied delegate is not specified
        /// </summary>
        /// <param name="text"></param>
        public CompletionDataSql(object f)
        {
            this.Text = f.ToString();
            this.Content = f.ToString();
            //old code: to move 
            /*
            this.Text = f.PropName;
            StackPanel sp = new StackPanel();
            TextBlock tb = new TextBlock(new Run(this.Text));
            tb.Margin = new Thickness(4, 4, 4, 4);
            TextOptions.SetTextFormattingMode(tb, TextFormattingMode.Display);
            TextOptions.SetTextFormattingMode(sp, TextFormattingMode.Display);

            Image i = new System.Windows.Controls.Image();
            i.Source = (BitmapImage)new Converters.FieldTypeToIconConverter().Convert(f.FieldType, typeof(ImageSource), null, System.Globalization.CultureInfo.InvariantCulture);
            this.Image = (BitmapImage)new Converters.FieldTypeToIconConverter().Convert(f.FieldType, typeof(ImageSource), null, System.Globalization.CultureInfo.InvariantCulture);

            i.VerticalAlignment = VerticalAlignment.Center;
            tb.VerticalAlignment = VerticalAlignment.Center;
            i.Height = 16;
            i.Width = 16;
            sp.Orientation = Orientation.Horizontal;
            //sp.Children.Add(i);
            sp.Children.Add(tb);
            this.Content = sp;

            this.Description = $"{AppModelHelper.Types.FirstOrDefault(t => AppModelHelper.GetFieldDescriptorsByType(t).Contains(f)).Name}.{f.PropName}";
        
          */
        }

        public System.Windows.Media.ImageSource Image
        {
            get; set;
        }

        public string Text { get; private set; }

        // Use this property if you want to show a fancy UIElement in the list.
        public object Content
        {
            get; set;
        }

        public object Description
        {
            get; set;
        }

        public double Priority
        {
            get
            {
                return 1;
            }
        }


        public void Complete(TextArea textArea, ISegment completionSegment,
            EventArgs insertionRequestEventArgs)
        {

            textArea.Document.Replace(completionSegment, this.Text);
        }


    }
}
